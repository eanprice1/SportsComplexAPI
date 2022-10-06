using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("EmergencyContact", Schema = "dbo")]
    public class EmergencyContactDb
    {
        [Key]
        public int Id { get; set; }
        public int GuardianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        [Column(TypeName = "char(10)")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? OtherAddress { get; set; }
        [Column(TypeName = "char(10)")]
        public string? OtherPhoneNumber { get; set; }
        public GuardianDb Guardian { get; set; }
    }
}