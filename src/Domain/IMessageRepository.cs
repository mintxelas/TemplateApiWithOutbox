using System;
using System.Threading.Tasks;

namespace Template.Domain
{
    public interface IMessageRepository
    {
        Task<Message[]> GetAll();
        Task<Message> GetById(Guid id);
        Task<Message> Save(Message message);
    }
}
