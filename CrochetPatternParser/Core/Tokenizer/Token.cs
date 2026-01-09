namespace CrochetPatternParser.Core.Tokenizer;

public enum TokenType
{
    Number,
    Stitch,
    Color,
    LParen,
    RParen,
    Semicolon,
    FastenOff,  // FO
    EndOfInput
}

public record Token(TokenType Type, string Lexeme);
