namespace CrochetPatternParser.Data
{
    public class RoundEntity
    {
        public int Id { get; set; }

        public int RoundNumber { get; set; }

        public string Text { get; set; } = "";

        public int SectionId { get; set; }
        public SectionEntity Section { get; set; } = null!;
    }
}
