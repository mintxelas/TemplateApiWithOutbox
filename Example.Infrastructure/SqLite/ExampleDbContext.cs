using Example.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure.SqLite
{
    public class ExampleDbContext: DbContext
    {
        public DbSet<MessageRecord> MessageRecord { get; set; }
        public DbSet<OutboxEvent> OutboxEvent { get; set; }

        public ExampleDbContext()
        {
            
        }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {
                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageRecord>().ToTable("MessageRecord").HasKey(m => m.Id);
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvent").HasNoKey();
        }
    }

    public class OutboxConsumerDbContext : DbContext
    {
        public DbSet<OutboxEvent> OutboxEvent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvent").HasNoKey();
        }
    }
}
