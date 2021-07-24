using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain.Entities;

namespace RabbitChat.Data
{
    public class RabbitChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public RabbitChatContext(DbContextOptions options) : base(options)
        {
        }
    }
}
