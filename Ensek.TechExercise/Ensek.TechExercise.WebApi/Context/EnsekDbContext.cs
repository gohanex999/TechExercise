using Ensek.TechExercise.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ensek.TechExercise.WebApi.Context
{
    public class EnsekDbContext : DbContext
    {
        public EnsekDbContext(DbContextOptions<EnsekDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeterReading>().HasOne(x => x.Account).WithMany();
        }
    }
}
