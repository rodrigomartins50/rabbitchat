using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Services;
using System.Threading.Tasks;

namespace RabbitChat.ConsumerSendMessage
{
    public class Execution
    {
        private IMessageService _messageService;

        public Execution(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task Execute(Message message)
        {
            _messageService.Insert(message);
        }
    }
}
