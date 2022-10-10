using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Models;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository
{
    public class SportsComplexDbContext : DbContext
    {
        public SportsComplexDbContext(DbContextOptions<SportsComplexDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchDb>()
                
                .HasOne(x => x.AwayTeam)
                .WithMany(x => x.AwayMatches)
                .HasForeignKey(x => x.AwayTeamId)
                .IsRequired();

            modelBuilder.Entity<MatchDb>()
                .HasOne(x => x.HomeTeam)
                .WithMany(x => x.HomeMatches)
                .HasForeignKey(x => x.HomeTeamId)
                .IsRequired();

            modelBuilder.Entity<GuardianDb>()
                .HasMany(x => x.Players)
                .WithOne(x => x.Guardian)
                .HasForeignKey(x => x.GuardianId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GuardianDb>()
                .HasMany(x => x.EmergencyContacts)
                .WithOne(x => x.Guardian)
                .HasForeignKey(x => x.GuardianId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SportDb>()
                .HasMany(x => x.Teams)
                .WithOne(x => x.Sport)
                .HasForeignKey(x => x.SportId);

            modelBuilder.Entity<SportDb>()
                .HasMany(x => x.Locations)
                .WithOne(x => x.Sport)
                .HasForeignKey(x => x.SportId);

            modelBuilder.Entity<TeamDb>()
                .HasMany(x => x.Practices)
                .WithOne(x => x.Team)
                .HasForeignKey(x => x.TeamId)
                .IsRequired();

            modelBuilder.Entity<TeamDb>()
                .HasMany(x => x.Players)
                .WithOne(x => x.Team)
                .HasForeignKey(x => x.TeamId);
        }

        public DbSet<GuardianDb> Guardian { get; set; }
        public DbSet<EmergencyContactDb> EmergencyContact { get; set; }
        public DbSet<PlayerDb> Player { get; set; }
        public DbSet<CoachDb> Coach { get; set; }
        public DbSet<TeamDb> Team { get; set; }
        public DbSet<SportDb> Sport { get; set; }
        public DbSet<MatchDb> Match { get; set; }
        public DbSet<PracticeDb> Practice { get; set; }
        public DbSet<LocationDb> Location { get; set; }
    }
}