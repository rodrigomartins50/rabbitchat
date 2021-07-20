using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Services;
using System.Threading.Tasks;

namespace RabbitChat.ConsumerSendMessage2
{
    public class Execution
    {
        private IMessageService _messageService;

        public Execution(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public Task Execute(Message message)
        {
            _messageService.Insert(message);

            return Task.CompletedTask;
        }
    }
}
