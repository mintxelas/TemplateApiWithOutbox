using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure.EntityFramework
{
    public interface IOutboxDbContext
    {
        DbSet<OutboxEvent> OutboxEvents { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}