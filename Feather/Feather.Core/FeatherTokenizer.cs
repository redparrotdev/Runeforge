using Feather.Core.Structure;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Feather.Core;

public static class FeatherTokenizer
{
    public static readonly Tokenizer<FeatherTokenType> Instance = new TokenizerImpl();

    private sealed class TokenizerImpl : Tokenizer<FeatherTokenType>
    {
        private static readonly Dictionary<string, FeatherTokenType> _keywords = new(StringComparer.Ordinal)
        {
            { "true", FeatherTokenType.True },
            { "false", FeatherTokenType.False },
            { "var", FeatherTokenType.Var },
            { "set", FeatherTokenType.Set },
            { "goto", FeatherTokenType.Goto },
            { "call", FeatherTokenType.Call },
            { "if", FeatherTokenType.If },
            { "else", FeatherTokenType.Else },
            { "and", FeatherTokenType.And },
            { "or", FeatherTokenType.Or },
            { "not", FeatherTokenType.Not },
            { "null", FeatherTokenType.Null },
            { "START", FeatherTokenType.Start },
            { "END", FeatherTokenType.End }
        };

        private static readonly Dictionary<char, FeatherTokenType> _singleCharTokens = new()
        {
            { '=', FeatherTokenType.Assign },
            { '#', FeatherTokenType.Hash },
            { '.', FeatherTokenType.Dot },
            { '@', FeatherTokenType.AtSign },
            { '{', FeatherTokenType.OpenBrace },
            { '}', FeatherTokenType.CloseBrace },
            { '<', FeatherTokenType.Less },
            { '>', FeatherTokenType.Greater },
            { '*', FeatherTokenType.Star }
        };

        protected override IEnumerable<Result<FeatherTokenType>> Tokenize(TextSpan span)
        {
            var next = span;

            while (!next.IsAtEnd)
            {
                if (IsWhitespace(next, ref next))
                {
                    continue;
                }

                if (IsComment(next, ref next))
                {
                    continue;
                }

                var ch = Character.AnyChar(next);
                if (!ch.HasValue)
                {
                    yield break;
                }

                if (IsSignleCharToken(ch.Value, out var charTokenType))
                {
                    yield return Result.Value(charTokenType, next, ch.Remainder);
                    next = ch.Remainder;
                    continue;
                }

                var identifier = Identifier.CStyle(next);
                if (IsIdentifierOrKeyword(identifier, out var idTokenType))
                {
                    yield return Result.Value(idTokenType, next, identifier.Remainder);
                    next = identifier.Remainder;
                    continue;
                }

                var number = Numerics.Decimal(next);
                if (number.HasValue)
                {
                    yield return Result.Value(FeatherTokenType.Number, next, number.Remainder);
                    next = number.Remainder;
                    continue;
                }

                var str = QuotedString.CStyle(next);
                if (str.HasValue)
                {
                    yield return Result.Value(FeatherTokenType.String, next, str.Remainder);
                    next = str.Remainder;
                    continue;
                }

                yield return Result.Empty<FeatherTokenType>(next, $"Unexpected character '{ch.Value}'");
                yield break;
            }
        }

        private static bool IsWhitespace(TextSpan span, ref TextSpan next)
        {
            var ws = Span.WhiteSpace(span);
            if (ws.HasValue)
            {
                next = ws.Remainder;
                return true;
            }

            return false;
        }

        private static bool IsComment(TextSpan span, ref TextSpan next)
        {
            var comment = Comment.CPlusPlusStyle(span);
            if (comment.HasValue)
            {
                next = comment.Remainder;
                return true;
            }

            return false;
        }

        private static bool IsSignleCharToken(char ch, out FeatherTokenType tokenType)
        {
            if (_singleCharTokens.TryGetValue(ch, out tokenType))
            {
                return true;
            }

            return false;
        }

        private static bool IsIdentifierOrKeyword(Result<TextSpan> identifier, out FeatherTokenType tokenType)
        {
            tokenType = default;
            if (!identifier.HasValue)
            {
                return false;
            }

            tokenType = _keywords.TryGetValue(identifier.Value.ToStringValue(), out tokenType) ? tokenType : FeatherTokenType.Identifier;

            return true;
        }
    }
}
