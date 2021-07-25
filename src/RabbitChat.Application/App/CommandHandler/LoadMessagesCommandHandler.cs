using MediatR;
using Microsoft.AspNetCore.SignalR;
using RabbitChat.Application.App.Command;
using RabbitChat.Application.SignalR;
using RabbitChat.Data.Repositories;
using RabbitChat.Domain;
using RabbitChat.Domain.Entities;
using System.Collections.Generic;
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
            _hub = hub;
        }

        public Task<bool> Handle(LoadMessagesCommand request, CancellationToken cancellationToken)
        {
            PagedList<Message> listMessages = null;

            if (request.DateRegisterLastMessage != null)
            {

                listMessages = this._messageRepository.GetList(
                    m => m.DateRegister < request.DateRegisterLastMessage,
                    o => o.OrderByDescending(x => x.DateRegister),
                    0, 30);
            }
            else
            {
                listMessages = this._messageRepository.GetList(
                    m => m.DateRegister != null,
                    o => o.OrderByDescending(x => x.DateRegister),
                    0, 30);
            }            

            _hub.Clients.Client(request.ConnectionId).SendAsync("LoadMessages", listMessages.ToList());

            return Task.FromResult(true);
        }
    }
}
