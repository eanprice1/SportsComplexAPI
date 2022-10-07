namespace SportsComplex.API.Api.Requests;

public class GetPlayerQuery
{
    public List<int> Ids { get; } = new();
    public List<int> TeamIds { get; } = new();
    public List<int> GuardianIds { get; } = new();
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}