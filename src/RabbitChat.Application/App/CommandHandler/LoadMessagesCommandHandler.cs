using MediatR;
using Microsoft.AspNetCore.SignalR;
using RabbitChat.Application.App.Command;
using RabbitChat.Application.SignalR;
using RabbitChat.Data.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Application.App.CommandHandler
{
    public class LoadMessagesCommandHandler : IRequestHandler<LoadMessagesCommand, bool>
    {
        private MessageRepository _messageRepository;
        private IHubContext<RabbitChatHub> _hub;

        public LoadMessagesCommandHandler(MessageRepository messageRepository, IHubContext<RabbitChatHub> hub)
        {
            _messageRepository = messageRepository;
        }

        public Task<bool> Handle(LoadMessagesCommand request, CancellationToken cancellationToken)
        {
            var listMessages = this._messageRepository.GetList(
                m => m.DateRegister < request.DateRegisterLastMessage,
                o => o.OrderByDescending(x => x.DateRegister),
                0, 30);

            _hub.Clients.All.SendAsync("LoadMessages", listMessages);

            return Task.FromResult(true);
        }
    }
}
