namespace Feather.Core.Structure;

public abstract record FeatherStatement
{
    public record ExpressionStatement(FeatherExpression Expression) : FeatherStatement;
    public record DialogLineStatement(FeatherExpression.CharacterNameExpression Character, string Text) : FeatherStatement;
    public record ChoiceStatement(string Text, FeatherExpression? Condition, BlockStatement? Block) : FeatherStatement;
    public record VariableDeclarationStatement(string Name, FeatherExpression Initializer) : FeatherStatement;
    public record SetVariableStatement(string Variable, FeatherExpression Value) : FeatherStatement;
    public record FunctionCallStatement(string FunctionName, IEnumerable<FeatherExpression> Arguments) : FeatherStatement;
    public record BlockStatement(IEnumerable<FeatherStatement> Statements) : FeatherStatement;
    public record LabelDeclarationStatement(string Label, BlockStatement Block, IEnumerable<ChoiceStatement> Choices) : FeatherStatement;
    public record StartStatement(FeatherExpression.LabelIdentifierExpression Label) : FeatherStatement;
    public record GotoStatement(FeatherExpression.LabelIdentifierExpression Label) : FeatherStatement;
    public record EndStatement() : FeatherStatement;
}
