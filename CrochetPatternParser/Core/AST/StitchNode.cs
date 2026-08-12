using System.Reflection.Metadata.Ecma335;

namespace CrochetPatternParser.Core.Ast;

public class StitchNode : IAstNode
{
    public string StitchType { get; }

    // Number of times to perform the stitch
    public int Count { get; }

    public StitchNode(string stitchType, int count)
    {
        StitchType = stitchType;
        Count = count;
    }



    public int GetProducedStitches()
    {
        return StitchType switch
        {
            "inc" => Count * 2,
            "slst" => 0,
            "sk" => 0,
            _ => Count
        };
    }

    public int GetConsumedStitches()
    {
        return StitchType switch
        {
            "dec" => 2 * Count,
            "slst" => 0,
            _ => Count
        };
    }
}
