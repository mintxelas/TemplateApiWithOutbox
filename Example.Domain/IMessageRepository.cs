using System.Threading.Tasks;

namespace Example.Domain
{
    public interface IMessageRepository
    {
        Task<Message> GetById(int id);
        Task<int> Save(Message message);
    }
}
