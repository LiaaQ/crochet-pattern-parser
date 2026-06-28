namespace CrochetPatternParser.Data
{
    public class SectionEntity
    {
        public int Id { get; set; }

        public int SectionNumber { get; set; }

        public int PatternId { get; set; }

        public PatternEntity Pattern { get; set; } = null!;

        // Navigation property for rounds
        public ICollection<RoundEntity> Rounds { get; set; } = new List<RoundEntity>();
    }
}