using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure.SqLite
{
    public class OutboxConsumerDbContext : DbContext, IOutboxDbContext
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
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(@"Data Source=MessagesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvent").HasKey(oe => oe.Id); 
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}