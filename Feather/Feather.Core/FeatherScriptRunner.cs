using Feather.Core.Exceptions.BinaryOperationsExceptions;
using Feather.Core.Helpers;
using Feather.Core.Models;
using Feather.Core.Structure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Feather.Core;

public sealed class FeatherScriptRunner
{
    private readonly Dictionary<string, object?> _scriptVariables = new(StringComparer.Ordinal);

    private FeatherScript _script = null!;
    private List<FeatherStatement> _topLevelStatements = [];
    private Dictionary<string, FeatherStatement.LabelDeclarationStatement> _labelsMap = [];
    private string _currentLabel = string.Empty;

    private LookaheadEnumerator<FeatherStatement>? _currentBlockStatementsEnumerator = null;
    private IEnumerable<FeatherStatement.ChoiceStatement> _currentBlockChoices = [];
    private readonly Dictionary<DialogLine.Choice, FeatherStatement.BlockStatement?> _currentLineChoicesBlocksMap = [];

    private bool _isFinished = false;
    private bool _waitingForChoiceSelection = false;

    public DialogLine? CurrentLine { get; private set; } = null;

    public void Initialize(FeatherScript script)
    {
        Debug.Assert(script != null);
        ArgumentNullException.ThrowIfNull(script);
        _script = script;
        _scriptVariables.Clear();
        CurrentLine = null;
        _isFinished = false;
        _waitingForChoiceSelection = false;
        _currentLineChoicesBlocksMap.Clear();
        InitializeCollections();
        InitializeTopLevelStatements();
        if (_script.StartLabel is { } startLabel)
        {
            _currentLabel = startLabel.Label.Label;
        }

        JumpToLabel(_currentLabel);
    }

    public bool NextDialogLine(DialogLine.Choice? selectedChoice = null)
    {
        if (_isFinished) return false;

        if (selectedChoice is not null && _waitingForChoiceSelection)
        {
            RunChoiceBlock(selectedChoice);
        }

        if (_waitingForChoiceSelection) return false;
        if (_currentBlockStatementsEnumerator is null) return false;

        var canContinue = RunLabelBlock();

        // In case we reached END statement
        if (_isFinished) return false;
        if (canContinue) return true;

        var choicesCount = _currentBlockChoices.TryGetNonEnumeratedCount(out var count) ? count : _currentBlockChoices.Count();
        if (choicesCount == 0)
        {
            // No choices and block end reached means we are finished the script
            _isFinished = true;
            return true;
        }

        SetChoices();
        _waitingForChoiceSelection = true;
        return true;
    }

    private void InitializeCollections()
    {
        _topLevelStatements = [];
        _labelsMap = new(StringComparer.Ordinal);

        foreach (var stmt in _script.TopLevelStatements)
        {
            if (stmt is FeatherStatement.LabelDeclarationStatement labelDecl)
            {
                if (string.IsNullOrWhiteSpace(_currentLabel))
                {
                    _currentLabel = labelDecl.Label;
                }

                _labelsMap[labelDecl.Label] = labelDecl;
                continue;
            }

            _topLevelStatements.Add(stmt);
        }
    }

    private void InitializeTopLevelStatements()
    {
        foreach (var stmt in _topLevelStatements)
        {
            EvaluateStatement(stmt);
        }
    }

    private void JumpToLabel(string label)
    {
        if (!_labelsMap.TryGetValue(label, out var labelDecl))
        {
            // TODO: Make own exception type
            throw new InvalidOperationException($"Label '{label}' is not defined.");
        }

        _currentLabel = label;
        _currentBlockStatementsEnumerator = new LookaheadEnumerator<FeatherStatement>(labelDecl.Block.Statements);
        _currentBlockChoices = labelDecl.Choices;
    }

    private bool RunLabelBlock()
    {
        if (_currentBlockStatementsEnumerator is null) return false;

        while(_currentBlockStatementsEnumerator.MoveNext())
        {
            var stmt = _currentBlockStatementsEnumerator.Current;
            EvaluateStatement(stmt);

            if (stmt is FeatherStatement.DialogLineStatement)
            {
                if (!_currentBlockStatementsEnumerator.HasNext) return false;

                return true;
            }

            // End statement reached
            if (_isFinished) return false;
        }

        return false;
    }

    private void RunChoiceBlock(DialogLine.Choice choice)
    {
        _waitingForChoiceSelection = false;

        if (!_currentLineChoicesBlocksMap.TryGetValue(choice, out var choiceBlock))
        {
            return;
        }

        if (choiceBlock is null || !choiceBlock.Statements.Any())
        {
            _isFinished = true;
            return;
        }

        var blockEnumerator = choiceBlock.Statements.GetEnumerator();

        while (blockEnumerator.MoveNext())
        {
            var stmt = blockEnumerator.Current;
            EvaluateStatement(stmt);

            if (_isFinished) return;
            if (stmt is FeatherStatement.GotoStatement)
            {
                return;
            }
        }

        _isFinished = true;
    }

    private void SetChoices()
    {
        Debug.Assert(CurrentLine != null);

        _currentLineChoicesBlocksMap.Clear();
        foreach (var choiceStmt in _currentBlockChoices)
        {
            var choiceText = choiceStmt.Text;
            var choice = new DialogLine.Choice(choiceText);

            CurrentLine.Choices.Add(choice);
            _currentLineChoicesBlocksMap[choice] = choiceStmt.Block;
        }
    }

    private void EvaluateStatement(FeatherStatement stmt)
    {
        switch (stmt)
        {
            case FeatherStatement.DialogLineStatement dialogLine:
                EvaluateDialogLineStatement(dialogLine);
                break;
            case FeatherStatement.VariableDeclarationStatement varDecl:
                EvaluateVariableDeclarationStatement(varDecl);
                break;
            case FeatherStatement.SetVariableStatement setVar:
                EvaluateSetVariableStatement(setVar);
                break;
            case FeatherStatement.GotoStatement gotoStatement:
                EvaluateGotoStatement(gotoStatement);
                break;
            case FeatherStatement.EndStatement:
                _isFinished = true;
                break;
            default:
                _isFinished = false;
                throw new InvalidOperationException($"Unsupported statement type: {stmt.GetType().FullName}");
        }
    }

    private object? EvaluateExpression(FeatherExpression expr)
    {
        switch (expr)
        {
            case FeatherExpression.StringExpressing strExpr:
                return new StrongBox<string>(strExpr.Value);
            case FeatherExpression.NumberExpressing numExpr:
                return new StrongBox<float>(numExpr.Value);
            case FeatherExpression.BooleanExpression boolExpr:
                return new StrongBox<bool>(boolExpr.Value);
            case FeatherExpression.NullExpression:
                return null;
            case FeatherExpression.IdentifierExpressing idExpr:
            {
                if (_scriptVariables.TryGetValue(idExpr.Name, out var value))
                {
                    return value;
                }

                // TODO: Make own exception type
                throw new InvalidOperationException($"Variable '{idExpr.Name}' is not defined.");
            }
            case FeatherExpression.LabelIdentifierExpression labelExpression:
                return new StrongBox<string>(labelExpression.Label);
            case FeatherExpression.CharacterNameExpression charNameExpression:
                return EvaluateExpression(charNameExpression.Value);
            case FeatherExpression.BinaryExpression binaryExpr:
                return EvaluateBinaryExpression(binaryExpr);
            default:
                throw new InvalidOperationException($"Unsupported expression type: {expr.GetType().FullName}");
        }
    }

    private object? EvaluateBinaryExpression(FeatherExpression.BinaryExpression binaryExpression)
    {
        var left = binaryExpression.Left;
        var right = binaryExpression.Right;
        var op = binaryExpression.Operator;

        var leftValue = EvaluateExpression(left);
        var rightValue = EvaluateExpression(right);

        switch (op)
        {
            case BinaryOperatorType.Plus:
            {
                return BinaryExpressionEvaluationHelper.EvaluatePlusOperatorTypeBinaryExpression(leftValue, rightValue);
            }
            case BinaryOperatorType.Minus:
            {
                return BinaryExpressionEvaluationHelper.EvaluateMinusOperatorTypeBinaryExpression(leftValue, rightValue);
            }
            case BinaryOperatorType.Multiply:
            {
                return BinaryExpressionEvaluationHelper.EvaluateMultiplyOperatorTypeBinaryExpression(leftValue, rightValue);
            }
            case BinaryOperatorType.Divide:
            {
                return BinaryExpressionEvaluationHelper.EvaluateDivideOperatorTypeBinaryExpression(leftValue, rightValue);
            }
            default:
                throw new UnsupportedBinaryOperatorException(op);
        }
    }

    private void EvaluateDialogLineStatement(FeatherStatement.DialogLineStatement dialogLine)
    {
        var characterName = EvaluateExpression(dialogLine.Character) switch
        {
            StrongBox<string> str => str.Value!,
            _ => throw new InvalidOperationException($"Unsupported character name expression type: {dialogLine.Character.GetType().FullName}")
        };

        // TODO: Add text processing through expressions in text
        var text = dialogLine.Text;

        CurrentLine = new DialogLine(characterName, text);
    }

    private void EvaluateVariableDeclarationStatement(FeatherStatement.VariableDeclarationStatement varDecl)
    {
        var initialValue = EvaluateExpression(varDecl.Initializer);
        _scriptVariables[varDecl.Name] = initialValue;
    }

    private void EvaluateSetVariableStatement(FeatherStatement.SetVariableStatement stmt)
    {
        if (!_scriptVariables.ContainsKey(stmt.Variable))
        {
            // TODO: Make own exception type
            throw new InvalidOperationException($"Variable '{stmt.Variable}' is not defined.");
        }

        var newValue = EvaluateExpression(stmt.Value);
        _scriptVariables[stmt.Variable] = newValue;
    }

    private void EvaluateGotoStatement(FeatherStatement.GotoStatement stmt)
    {
        var label = EvaluateExpression(stmt.Label) switch
        {
            StrongBox<string> str => str.Value!,
            _ => throw new InvalidOperationException($"Unsupported label expression type: {stmt.Label.GetType().FullName}")
        };

        JumpToLabel(label);
    }
}
