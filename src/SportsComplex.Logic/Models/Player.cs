using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models
{
    public class Player : IModel
    {
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public int GuardianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
    }
}
