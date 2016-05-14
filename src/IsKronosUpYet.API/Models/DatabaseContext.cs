using Microsoft.Data.Entity;

namespace IsKronosUpYet.API.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerStatus> ServerStatus { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IP required
            modelBuilder.Entity<Server>()
                .Property(b => b.IP)
                .IsRequired();
        }
    }
}