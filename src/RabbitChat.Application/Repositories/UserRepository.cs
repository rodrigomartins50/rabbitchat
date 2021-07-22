using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain.Entities;

namespace RabbitChat.Data.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}
