using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Repositories;

namespace RabbitChat.Data.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(DbContext context) : base(context)
        {
        }
    }
}
