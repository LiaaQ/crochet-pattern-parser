public class RoundValidationResult
{
    public int RoundIndex { get; init; }
    public int StitchCount { get; init; }
    public int? ExpectedStitchConsumed { get; set; }
    public string? Error { get; set; }
}
