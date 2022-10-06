using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Match", Schema = "dbo")]
    public class MatchDb
    {
        [Key]
        public int Id { get; set; }
        public int SportId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int LocationId { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime StartDateTime { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime EndDateTime { get; set; }
        public SportDb Sport { get; set; }
        public TeamDb HomeTeam { get; set; }
        public TeamDb AwayTeam { get; set; }
        public LocationDb Location { get; set; }
    }
}