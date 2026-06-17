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

    private IEnumerator<FeatherStatement>? _currentBlockStatementsEnumerator = null;
    private IEnumerable<FeatherStatement.ChoiceStatement> _currentBlockChoices = [];

    private bool _isFinished = false;

    public DialogLine? CurrentLine { get; private set; } = null;

    public void Initialize(FeatherScript script)
    {
        Debug.Assert(script != null);
        ArgumentNullException.ThrowIfNull(script);
        _script = script;
        CurrentLine = null;
        _isFinished = false;
        InitializeCollections();
        InitializeTopLevelStatements();
        if (_script.StartLabel is { } startLabel)
        {
            _currentLabel = startLabel.Label.Label;
        }

        JumpToLabel(_currentLabel);
    }

    public bool NextDialogLine()
    {
        if (_isFinished) return false;
        if (_currentBlockStatementsEnumerator is null) return false;

        var canContinue = RunBlock(_currentBlockStatementsEnumerator);

        if (canContinue) return true;

        // TODO: Handle choices

        return false;
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
        _currentBlockStatementsEnumerator = labelDecl.Block.Statements.GetEnumerator();
        _currentBlockChoices = labelDecl.Choices;
    }

    private bool RunBlock(IEnumerator<FeatherStatement> block)
    {
        while(block.MoveNext())
        {
            var stmt = block.Current;
            EvaluateStatement(stmt);

            if (stmt is FeatherStatement.DialogLineStatement)
            {
                return true;
            }
        }

        return false;
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
            default:
                throw new InvalidOperationException($"Unsupported expression type: {expr.GetType().FullName}");
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
}
