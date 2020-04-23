using Example.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure.SqLite
{
    public class OutboxConsumerDbContext : DbContext
    {
        public DbSet<OutboxEvent> OutboxEvent { get; set; }

        public OutboxConsumerDbContext()
        {
            
        }

        public OutboxConsumerDbContext(DbContextOptions<OutboxConsumerDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvent").HasKey(oe => oe.Id); 
        }
    }
}