using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Team", Schema = "dbo")]
    public class TeamDb
    {
        [Key]
        public int Id { get; set; }
        public int SportId { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public SportDb Sport { get; set; }
        public List<MatchDb> HomeMatches { get; set; }
        public List<MatchDb> AwayMatches { get; set; }
        public List<PracticeDb> Practices { get; set; }
    }
}