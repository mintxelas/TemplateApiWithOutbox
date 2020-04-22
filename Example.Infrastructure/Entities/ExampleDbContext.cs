using Example.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure
{
    public class ExampleDbContext: DbContext
    {
        public DbSet<MessageRecord> Messages { get; set; }
        public DbSet<OutboxEvent> Outbox { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageRecord>().ToTable("Messages").HasKey(m => m.Id);
            modelBuilder.Entity<OutboxEvent>().ToTable("Outbox").HasNoKey();
        }
    }
}
