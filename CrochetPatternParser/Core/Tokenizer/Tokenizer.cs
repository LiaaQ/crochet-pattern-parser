using System.Text;

namespace CrochetPatternParser.Core.Tokenizer;

public class Tokenizer
{
    private readonly string _input;
    private int _position;

    public Tokenizer(string input)
    {
        _input = input;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (!IsAtEnd())
        {
            SkipWhitespace();

            if (IsAtEnd())
                break;

            char c = Peek();

            if (c == '@')
            {
                tokens.Add(ReadColor());
            }
            else if (char.IsDigit(c))
            {
                tokens.Add(ReadNumber());
            }
            else if (char.IsLetter(c))
            {
                var token = ReadWord();
                if (token != null)
                    tokens.Add(token);
            }
            else
            {
                var token = ReadSymbol();
                if (token != null)
                    tokens.Add(token);
            }
        }

        tokens.Add(new Token(TokenType.EndOfInput, ""));
        return tokens;
    }

    private Token ReadNumber()
    {
        var sb = new StringBuilder();

        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            sb.Append(Advance());
        }

        return new Token(TokenType.Number, sb.ToString());
    }

    private Token? ReadWord()
    {
        var sb = new StringBuilder();

        while (!IsAtEnd() && char.IsLetter(Peek()))
        {
            sb.Append(Advance());
        }

        string word = sb.ToString();

        return word switch
        {
            "sc" or "hdc" or "dc" or "tr" or "inc" or "dec" or "mr" or "slst"
                => new Token(TokenType.Stitch, word),

            "FO"
                => new Token(TokenType.FastenOff, word),

            _ => new Token(TokenType.Unknown, $"Unknown keyword: {word}")
        };
    }

    private Token? ReadSymbol()
    {
        char c = Advance();

        return c switch
        {
            '(' => new Token(TokenType.LParen, "("),
            ')' => new Token(TokenType.RParen, ")"),
            '[' => new Token(TokenType.LBracket, "["),
            ']' => new Token(TokenType.RBracket, "]"),
            ';' => new Token(TokenType.Semicolon, ";"),
            ',' => null,
            _ => new Token(TokenType.Unknown, $"Unexpected character: {c}")
        };
    }

    private Token ReadColor()
    {
        Advance(); // consume '@'

        var sb = new StringBuilder();

        while (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
        {
            sb.Append(Advance());
        }

        if (sb.Length == 0)
            return new Token(TokenType.Unknown, "Color name expected after '@'");

        return new Token(TokenType.Color, sb.ToString());
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
            Advance();
    }

    private char Peek() => _input[_position];
    private char Advance() => _input[_position++];
    private bool IsAtEnd() => _position >= _input.Length;
}
