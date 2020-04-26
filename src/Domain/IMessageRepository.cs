using System;
using System.Threading.Tasks;

namespace Template.Domain
{
    public interface IMessageRepository
    {
        Task<Message> GetById(Guid id);
        Task<Message> Save(Message message);
    }
}
