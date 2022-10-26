namespace SportsComplex.API.Api.Requests;

public class GetCoachQuery
{
    public List<int> Ids { get; } = new();
    public List<int> TeamIds { get; } = new();
    public bool OnlyUnassignedCoaches { get; set; } = false;
    public bool OnlyHeadCoaches { get; set; } = false;
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}