using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Repositories;

namespace RabbitChat.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}
