namespace SportsComplex.Logic.Models;

public class CoachQuery
{
    public List<int> Ids { get; init; } = new();
    public List<int> TeamIds { get; init; } = new();
    public bool OnlyUnassignedCoaches { get; init; }
    public bool OnlyHeadCoaches { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}