namespace CrochetPatternParser.Core.Ast;

public class RoundNode : IAstNode
{
    public List<IAstNode> Elements { get; } = new();

    public bool HasIncOrDec()
    {
        foreach (var element in Elements)
        {
            if (HasIncOrDec(element))
                return true;
        }

        return false;
    }

    public bool HasInvalidStitchesInIncreaseGroup()
    {
        foreach (var element in Elements)
        {
            if (element is IncreaseNode increase && increase.Stitches.Exists(stitch => stitch.StitchType == "dec" || stitch.StitchType == "inc"))
                return true;
        }

        return false;
    }

    private static bool HasIncOrDec(IAstNode element)
    {
        return element switch
        {
            StitchNode stitch => stitch.StitchType is "inc" or "dec",
            GroupNode group => group.Stitches.Exists(stitch => stitch.StitchType is "inc" or "dec"),
            IncreaseNode increase => true,
            _ => false
        };
    }

    public int GetProducedStitches()
    {
        int total = 0;

        foreach (var element in Elements)
        {
            total += element switch
            {
                StitchNode s => s.GetProducedStitches(),
                GroupNode g => g.GetProducedStitches(),
                IncreaseNode i => i.GetProducedStitches(),
                _ => 0
            };
        }

        return total;
    }

    public int GetConsumedStitches()
    {
        int total = 0;

        foreach (var element in Elements)
        {
            total += element switch
            {
                StitchNode s => s.GetConsumedStitches(),
                GroupNode g => g.GetConsumedStitches(),
                IncreaseNode i => i.GetConsumedStitches(),
                _ => 0
            };
        }

        return total;
    }

}
