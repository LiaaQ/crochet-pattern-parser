namespace CrochetPatternParser.Core.Ast;

public class GroupNode : IAstNode
{
    public List<StitchNode> Stitches { get; }
    public int RepeatCount { get; }

    public GroupNode(List<StitchNode> stitches, int repeatCount)
    {
        Stitches = stitches;
        RepeatCount = repeatCount;
    }
}
