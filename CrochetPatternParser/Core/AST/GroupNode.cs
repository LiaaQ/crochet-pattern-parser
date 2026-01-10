using System.ComponentModel;

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

    public int GetProducedStitches()
    {
        int total = 0;
        foreach (var stitch in Stitches)
        {
            total += stitch.GetProducedStitches();
        }
        return total * RepeatCount;
    }

    public int GetConsumedStitches()
    {
        int total = 0;
        foreach (var stitch in Stitches)
        {
            total += stitch.GetConsumedStitches();
        }
        return total * RepeatCount;
    }
}
