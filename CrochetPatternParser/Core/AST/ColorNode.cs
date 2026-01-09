namespace CrochetPatternParser.Core.Ast;

public class ColorNode : IAstNode
{
    public string ColorName { get; }

    public ColorNode(string colorName)
    {
        ColorName = colorName;
    }
}
