using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Location", Schema = "dbo")]
    public class LocationDb
    {
        [Key]
        public int Id { get; set; }
        public int SportId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public SportDb Sport { get; set; }
        public List<MatchDb> Matches { get; set; }
        public List<PracticeDb> Practices { get; set; }
    }
}