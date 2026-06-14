using Feather.Core.Structure;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Feather.Core;

public static class FeatherTokenizer
{
    private static readonly Tokenizer<FeatherTokenType> _tokenizer = new TokenizerBuilder<FeatherTokenType>()
        .Ignore(Span.WhiteSpace)
        .Ignore(Comment.CPlusPlusStyle)
        // Single character tokens
        .Match(Span.EqualTo('='), FeatherTokenType.Assign)
        .Match(Span.EqualTo('#'), FeatherTokenType.Hash)
        .Match(Span.EqualTo('.'), FeatherTokenType.Dot)
        .Match(Span.EqualTo('@'), FeatherTokenType.AtSign)
        .Match(Span.EqualTo('{'), FeatherTokenType.OpenBrace)
        .Match(Span.EqualTo('}'), FeatherTokenType.CloseBrace)
        .Match(Span.EqualTo('<'), FeatherTokenType.Less)
        .Match(Span.EqualTo('>'), FeatherTokenType.Greater)
        // Keywords
        .Match(Span.EqualTo("true"), FeatherTokenType.True)
        .Match(Span.EqualTo("false"), FeatherTokenType.False)
        .Match(Span.EqualTo("var"), FeatherTokenType.Var)
        .Match(Span.EqualTo("set"), FeatherTokenType.Set)
        .Match(Span.EqualTo("goto"), FeatherTokenType.Goto)
        .Match(Span.EqualTo("call"), FeatherTokenType.Call)
        .Match(Span.EqualTo("if"), FeatherTokenType.If)
        .Match(Span.EqualTo("else"), FeatherTokenType.Else)
        .Match(Span.EqualTo("and"), FeatherTokenType.And)
        .Match(Span.EqualTo("or"), FeatherTokenType.Or)
        .Match(Span.EqualTo("not"), FeatherTokenType.Not)
        .Match(Span.EqualTo("null"), FeatherTokenType.Null)
        .Match(Span.EqualTo("START"), FeatherTokenType.Start)
        .Match(Span.EqualTo("END"), FeatherTokenType.End)
        // Literals
        .Match(Identifier.CStyle, FeatherTokenType.Identifier)
        .Match(Numerics.Decimal, FeatherTokenType.Number)
        .Match(QuotedString.CStyle, FeatherTokenType.String)
        .Build();

    public static readonly Tokenizer<FeatherTokenType> Instance = _tokenizer;
}
