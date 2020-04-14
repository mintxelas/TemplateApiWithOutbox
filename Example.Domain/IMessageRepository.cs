namespace Example.Model
{
    public interface IMessageRepository
    {
        Message GetById(int id);
        void Save(Message message);
    }
}
