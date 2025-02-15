using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.Infrastructure.Entities;

namespace Sample.Infrastructure.EntityFramework
{
    public interface IOutboxDbContext
    {
        DbSet<OutboxEvent> OutboxEvents { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}