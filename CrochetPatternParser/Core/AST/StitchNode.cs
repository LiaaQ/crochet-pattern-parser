namespace CrochetPatternParser.Core.Ast;

public class StitchNode : IAstNode
{
    public string StitchType { get; }
    public int Count { get; }

    public StitchNode(string stitchType, int count)
    {
        StitchType = stitchType;
        Count = count;
    }
}
