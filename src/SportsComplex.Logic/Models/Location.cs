using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models;

public class Location : IModel
{
    public int Id { get; set; }
    public int SportId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
}