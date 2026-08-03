namespace CrochetPatternParser.Core.Tokenizer;

public enum TokenType
{
    Number,
    Stitch,
    Color,
    Unknown,
    LParen,
    RParen,
    Semicolon,
    FastenOff,  // FO
    EndOfInput
}

public record Token(TokenType Type, string Lexeme);
