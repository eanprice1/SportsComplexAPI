namespace SportsComplex.API.Api.Requests;

public class MatchRequest
{
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int? LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}