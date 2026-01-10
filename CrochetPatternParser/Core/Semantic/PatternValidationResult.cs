public class PatternValidationResult
{
    public List<RoundValidationResult> Rounds { get; } = new();
    public bool HasError => Rounds.Any(r => r.Error != null);
}
