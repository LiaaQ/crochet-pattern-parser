namespace CrochetPatternParser.Core.Ast;

public class PatternNode : IAstNode
{
    public List<RoundNode> Rounds { get; } = new();
}
