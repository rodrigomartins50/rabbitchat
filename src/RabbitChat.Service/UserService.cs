using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Repositories;
using RabbitChat.Domain.Services;

namespace RabbitChat.Service
{
    public class UserService : Service<User>, IUserService
    {
        public UserService(IUserRepository repository) : base(repository)
        {
        }
    }
}
