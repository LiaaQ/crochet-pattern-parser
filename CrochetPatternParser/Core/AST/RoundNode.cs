namespace CrochetPatternParser.Core.Ast;

public class RoundNode : IAstNode
{
    public List<IAstNode> Elements { get; } = new();

    public int GetProducedStitches()
    {
        int total = 0;

        foreach (var element in Elements)
        {
            total += element switch
            {
                StitchNode s => s.GetProducedStitches(),
                GroupNode g => g.GetProducedStitches(),
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
                _ => 0
            };
        }

        return total;
    }

}
