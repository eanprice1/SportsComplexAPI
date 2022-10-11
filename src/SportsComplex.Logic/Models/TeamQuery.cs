namespace SportsComplex.Logic.Models;

public class TeamQuery
{
    public List<int> Ids { get; set; } = new();
    public List<int> SportIds { get; set; } = new();
    public string? Name { get; set; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}