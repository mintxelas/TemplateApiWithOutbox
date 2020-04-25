using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure
{
    public interface IOutboxDbContext
    {
        DbSet<OutboxEvent> OutboxEvent { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}