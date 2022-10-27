namespace SportsComplex.Logic.Models;

public class PlayerQuery
{
    public List<int> Ids { get; init; } = new();
    public List<int> TeamIds { get; init; } = new();
    public List<int> GuardianIds { get; init; } = new();
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool OnlyUnassignedPlayers { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}