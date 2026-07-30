namespace CrochetPatternParser.Data
{
    public class PatternEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = "Untitled Pattern";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImagePath { get; set; }

        public string UserId { get; set; } = "";
        public ApplicationUserEntity User { get; set; } = null!;

        // Navigation property for sections
        public ICollection<SectionEntity> Sections { get; set; } = new List<SectionEntity>();
    }
}
