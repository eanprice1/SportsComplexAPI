namespace SportsComplex.API.Api.Requests;

public class PracticeRequest
{
    public int TeamId { get; set; }
    public int? LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}