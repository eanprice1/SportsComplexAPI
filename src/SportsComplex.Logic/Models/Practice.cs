using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models;

public class Practice : IModel
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int? LocationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}