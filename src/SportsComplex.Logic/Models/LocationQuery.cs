namespace SportsComplex.Logic.Models;

public class LocationQuery
{
    public List<int> Ids { get; init; } = new();
    public List<int> SportIds { get; init; } = new();
    public string? Name { get; init; }
    public string? Address { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}