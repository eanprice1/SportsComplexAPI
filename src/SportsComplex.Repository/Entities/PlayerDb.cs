using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Player", Schema = "dbo")]
    public class PlayerDb
    {
        [Key]
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public int GuardianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public GuardianDb Guardian { get; set; }
        public TeamDb? Team { get; set; }
    }
}