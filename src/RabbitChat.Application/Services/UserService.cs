using RabbitChat.Data.Repositories;
using RabbitChat.Domain.Entities;

namespace RabbitChat.Service
{
    public class UserService : BaseService<User>
    {
        public UserService(UserRepository repository) : base(repository)
        {
        }
    }
}
