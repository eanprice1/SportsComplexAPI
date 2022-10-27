namespace SportsComplex.Logic.Models
{
    public class GuardianQuery
    {
        public List<int> Ids { get; init; } = new();
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public int? Count { get; init; }
        public string? OrderBy { get; init; }
        public bool Descending { get; init; }
    }
}
