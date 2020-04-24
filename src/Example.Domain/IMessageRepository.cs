using System;
using System.Threading.Tasks;

namespace Example.Domain
{
    public interface IMessageRepository
    {
        Task<Message> GetById(Guid id);
        Task Save(Message message);
    }
}
