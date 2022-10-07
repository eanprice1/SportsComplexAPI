namespace SportsComplex.Logic.Models
{
    public class GuardianQuery
    {
        public List<int> Ids { get; init; } = new();
        public int? Count { get; init; }
        public string? OrderBy { get; init; }
        public bool Descending { get; init; }
    }
}
