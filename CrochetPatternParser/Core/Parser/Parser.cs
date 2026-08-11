using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Ast;

namespace CrochetPatternParser.Core.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public PatternNode Parse()
    {
        var pattern = new PatternNode();

        while (!IsAtEnd())
        {
            pattern.Rounds.Add(ParseRound());
        }

        return pattern;
    }

    private RoundNode ParseRound()
    {
        var round = new RoundNode();

        while (!IsAtEnd() && Peek().Type != TokenType.Semicolon)
        {
            round.Elements.Add(ParseElement());
        }

        Match(TokenType.Semicolon);
        return round;
    }

    private IAstNode ParseElement()
    {
        return Peek().Type switch
        {
            TokenType.Number or TokenType.Stitch => ParseStitch(),
            TokenType.LParen => ParseGroup(),
            TokenType.LBracket => ParseIncrease(),
            TokenType.Color => ParseColor(),
            TokenType.FastenOff => ParseFastenOff(),
            TokenType.Unknown => throw new Exception(Peek().Lexeme),
            _ => throw new Exception($"Unexpected token: {Peek().Type}.")
        };
    }

    private IAstNode ParseFastenOff()
    {
        Advance(); // consumes FO
        return new FastenOffNode();
    }

    private IAstNode ParseColor()
    {
        var token = Advance(); // consumes color token
        return new ColorNode(token.Lexeme);
    }

    private GroupNode ParseGroup()
    {
        Advance(); // '('

        var stitches = new List<StitchNode>
        {
            ParseStitch()
        };

        while (Peek().Type == TokenType.Number || Peek().Type == TokenType.Stitch)
            stitches.Add(ParseStitch());

        Expect(TokenType.RParen);

        if (Peek().Type != TokenType.Number)
            throw new Exception("Expected repeat count after group.");

        int repeat = int.Parse(Advance().Lexeme);

        return new GroupNode(stitches, repeat);
    }

    private IncreaseNode ParseIncrease()
    {
        Advance(); // '['

        var stitches = new List<StitchNode>
        {
            ParseStitch()
        };

        while (Peek().Type == TokenType.Number || Peek().Type == TokenType.Stitch)
            stitches.Add(ParseStitch());

        Expect(TokenType.RBracket);

        return new IncreaseNode(stitches);
    }

    private StitchNode ParseStitch()
    {
        int count = 1;

        if (Peek().Type == TokenType.Number)
        {
            count = int.Parse(Advance().Lexeme);
        }

        var stitchToken = Expect(TokenType.Stitch);

        return new StitchNode(stitchToken.Lexeme, count);
    }


    private bool IsAtEnd()
        => Peek().Type == TokenType.EndOfInput;

    private Token Peek()
        => _tokens[_current];

    private Token Advance()
        => _tokens[_current++];

    private bool Match(TokenType type)
    {
        if (Peek().Type != type)
            return false;

        Advance();
        return true;
    }

    private Token Expect(TokenType type)
    {
        if (Peek().Type != type)
            throw new Exception(
                $"Expected {type}, got {Peek().Type} ('{Peek().Lexeme}')."
            );

        return Advance();
    }
}