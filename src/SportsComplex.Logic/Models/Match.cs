using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models;

public class Match : IModel
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int? LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}