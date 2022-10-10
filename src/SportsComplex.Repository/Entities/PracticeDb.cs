using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsComplex.Repository.Entities
{
    [Table("Practice", Schema = "dbo")]
    public class PracticeDb
    {
        [Key]
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int? LocationId { get; set; }
        public string DayOfWeek { get; set; }
        [Column(TypeName = "Time")]
        public TimeSpan StartTime { get; set; }
        [Column(TypeName = "Time")]
        public TimeSpan EndTime { get; set; }
        public TeamDb Team { get; set; }
        public LocationDb Location { get; set; }
    }
}