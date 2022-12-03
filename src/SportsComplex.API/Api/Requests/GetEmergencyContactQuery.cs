namespace SportsComplex.API.Api.Requests;

public class GetEmergencyContactQuery
{
    public List<int> Ids { get; set; } = new();
    public List<int> GuardianIds { get; set; } = new();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Count { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; } = false;
}