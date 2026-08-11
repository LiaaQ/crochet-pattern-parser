namespace CrochetPatternParser.Core.Ast;

public class IncreaseNode : IAstNode
{
    public List<StitchNode> Stitches { get; }

    public IncreaseNode(List<StitchNode> stitches)
    {
        Stitches = stitches;
    }

    public bool ContainsIncOrDec()
    {
        foreach (var stitch in Stitches)
        {
            if (stitch.StitchType is "inc" or "dec")
                return true;
        }

        return false;
    }

    public int GetProducedStitches()
    {
        int total = 0;
        foreach (var stitch in Stitches)
        {
            total += stitch.GetProducedStitches();
        }

        return total;
    }

    public int GetConsumedStitches()
    {
        return 1;
    }
}