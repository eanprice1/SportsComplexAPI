namespace SportsComplex.Logic.Models;

public class MatchQuery
{
    public List<int> Ids { get; init; } = new();
    public List<int> TeamIds { get; init; } = new();
    public List<int> LocationIds { get; init; } = new();
    public DateTime? StartRange { get; init; }
    public DateTime? EndRange { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}