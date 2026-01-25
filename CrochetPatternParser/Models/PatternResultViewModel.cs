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
        public string Title { get; set; } = "Untitled Pattern";
        public List<string> RoundTexts { get; set; } = new();
        public List<RoundViewModel> Rounds { get; set; } = new();
        public bool Saved { get; set; } = false;
        public string? ImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
