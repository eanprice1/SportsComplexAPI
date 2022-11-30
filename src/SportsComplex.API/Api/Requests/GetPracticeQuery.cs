namespace SportsComplex.API.Api.Requests;

public class GetPracticeQuery
{
    public List<int> Ids { get; set; } = new();
    public List<int> TeamIds { get; set; } = new();
    public List<int> LocationIds { get; set; } = new();
    public DateTime? StartRange { get; set; }
    public DateTime? EndRange { get; set; }
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}