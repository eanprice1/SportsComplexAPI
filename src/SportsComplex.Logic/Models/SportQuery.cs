namespace SportsComplex.Logic.Models;

public class SportQuery
{
    public List<int> Ids { get; set; } = new();
    public string? Name { get; set; }
    public DateTime? StartRange { get; set; }
    public DateTime? EndRange { get; set; }
    public bool FilterByEndDateFlag { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}