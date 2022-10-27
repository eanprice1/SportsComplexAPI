namespace SportsComplex.API.Api.Requests;

public class GetCoachQuery
{
    public List<int> Ids { get; } = new();
    public List<int> TeamIds { get; } = new();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? IsHeadCoach { get; set; }
    public bool OnlyUnassignedCoaches { get; set; } = false;
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}