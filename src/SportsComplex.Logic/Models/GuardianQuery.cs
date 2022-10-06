namespace SportsComplex.Logic.Models
{
    public class GuardianQuery
    {
        public int? Count { get; set; }
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}
