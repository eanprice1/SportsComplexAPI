using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models
{
    public class Team : IModel
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
    }
}
