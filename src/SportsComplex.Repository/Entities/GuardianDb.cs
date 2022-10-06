using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Guardian", Schema = "dbo")]
    public class GuardianDb
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        [Column(TypeName = "char(10)")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? OtherAddress { get; set; }

        public List<EmergencyContactDb> EmergencyContacts { get; set; }
        public List<PlayerDb> Players { get; set; }
    }
}
