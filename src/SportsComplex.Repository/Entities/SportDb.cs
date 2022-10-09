using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Sport", Schema = "dbo")]
    public class SportDb
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinTeamSize { get; set; }
        public int MaxTeamSize { get; set; }
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime EndDate { get; set; }
        public List<TeamDb> Teams { get; set; }
        public List<LocationDb> Locations { get; set; }
    }
}