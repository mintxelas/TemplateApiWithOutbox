using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure.SqLite
{
    public class ExampleDbContext: DbContext
    {
        public DbSet<MessageRecord> MessageRecords { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        public ExampleDbContext()
        {
            
        }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {
                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageRecord>().ToTable("MessageRecords").HasKey(m => m.Id);
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvents").HasKey(oe => oe.Id);
        }
    }
}
