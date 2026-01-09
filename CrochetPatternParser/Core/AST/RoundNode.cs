namespace CrochetPatternParser.Core.Ast;

public class RoundNode : IAstNode
{
    public List<IAstNode> Elements { get; } = new();
}
