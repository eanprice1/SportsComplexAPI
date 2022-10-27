namespace SportsComplex.API.Api.Requests;

public class LocationRequest
{
    public int SportId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
}