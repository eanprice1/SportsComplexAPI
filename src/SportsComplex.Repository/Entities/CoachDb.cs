using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Coach", Schema = "dbo")]
    public class CoachDb
    {
        [Key]
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        [Column(TypeName = "char(10)")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "Bit")]
        public bool IsHeadCoach { get; set; }

        public TeamDb? Team { get; set; }
    }
}