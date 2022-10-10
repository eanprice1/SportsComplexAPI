namespace SportsComplex.API.Api.Requests;

public class SportRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int MinTeamSize { get; set; }
    public int MaxTeamSize { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}