using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.Infrastructure.Entities;

namespace Sample.Infrastructure.EntityFramework;

public class OutboxConsumerDbContext(DbContextOptions<OutboxConsumerDbContext> options)
    : DbContext(options), IOutboxDbContext
{
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxEvent>().HasKey(oe => oe.Id); 
    }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}