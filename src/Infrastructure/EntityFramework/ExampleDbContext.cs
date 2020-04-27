using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure.EntityFramework
{
    public class ExampleDbContext: DbContext
    {
        public DbSet<MessageRecord> MessageRecords { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }
        
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageRecord>().HasKey(m => m.Id);
            modelBuilder.Entity<OutboxEvent>().HasKey(oe => oe.Id);
        }
    }
}
