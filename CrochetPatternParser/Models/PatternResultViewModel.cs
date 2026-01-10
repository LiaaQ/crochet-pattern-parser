namespace CrochetPatternParser.Models
{
    public class RoundViewModel
    {
        public int RoundIndex { get; set; }
        public int StitchCount { get; set; }
        public int? ExpectedStitchConsumed { get; set; }
        public string? Error { get; set; }
    }

    public class PatternViewModel
    {
        public string PatternText { get; set; } = "";
        public List<RoundViewModel> Rounds { get; set; } = new();
    }
}
