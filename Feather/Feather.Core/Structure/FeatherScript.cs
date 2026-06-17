namespace Feather.Core.Structure;

public sealed record FeatherScript(FeatherStatement.StartStatement? StartLabel, IEnumerable<FeatherStatement> TopLevelStatements);
