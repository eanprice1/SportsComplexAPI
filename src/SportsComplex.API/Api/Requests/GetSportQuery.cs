namespace SportsComplex.API.Api.Requests;

public class GetSportQuery
{
    public List<int> Ids { get; set; } = new();
    public string? Name { get; set; }
    public DateTime? StartRange { get; set; }
    public DateTime? EndRange { get; set; }
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}