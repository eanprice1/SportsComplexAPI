namespace SportsComplex.API.Api.Requests;

public class GetTeamQuery
{
    public List<int> Ids { get; set; } = new();
    public List<int> SportIds { get; set; } = new();
    public string? Name { get; set; }
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}