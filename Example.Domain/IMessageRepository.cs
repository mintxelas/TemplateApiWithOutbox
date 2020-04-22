using System.Threading.Tasks;

namespace Example.Domain
{
    public interface IMessageRepository
    {
        Task<Message> GetById(int id);
        Task Save(Message message);
    }
}
