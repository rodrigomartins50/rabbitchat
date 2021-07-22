using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain.Entities;

namespace RabbitChat.Data.Repositories
{
    public class MessageRepository : BaseRepository<Message>
    {
        public MessageRepository(DbContext context) : base(context)
        {
        }
    }
}
