namespace CrochetPatternParser.Data
{
    public class PatternEntity
    {
        public int Id { get; set; }

        public string RawText { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = "";
        public ApplicationUserEntity User { get; set; } = null!;
    }
}
