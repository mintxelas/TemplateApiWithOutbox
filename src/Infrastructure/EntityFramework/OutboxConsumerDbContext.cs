using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.Infrastructure.Entities;

namespace Sample.Infrastructure.EntityFramework
{
    public class OutboxConsumerDbContext : DbContext, IOutboxDbContext
    {
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        public OutboxConsumerDbContext(DbContextOptions<OutboxConsumerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>().HasKey(oe => oe.Id); 
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}