namespace Feather.Core.Structure;

public enum FeatherTokenType
{
    None = 0,
    // Literals
    Identifier, // e.g. variable names, function names
    String, // e.g. "Hello, World!"
    Number, // e.g. 123, 3.14

    // Keywords
    True, // true
    False, // false
    Var, // var
    Set, // set
    Goto, // goto
    Call, // call
    If, // if
    Else, // else
    And, // and
    Or, // or
    Not, // not
    Null, // null
    Start, // start
    End, // end

    // Single character tokens
    Assign, // =
    Hash, // #
    Dot, // .
    Star, // *
    AtSign, // @
    OpenBrace, // {
    CloseBrace, // }
    Less, // <
    Greater, // >
    Plus, // +
    Minus, // -
    Slash, // /
    OpenParen, // (
    CloseParen, // )
}
