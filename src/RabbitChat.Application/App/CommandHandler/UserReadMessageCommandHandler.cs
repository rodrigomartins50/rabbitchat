using MediatR;
using RabbitChat.Application.App.Command;
using RabbitChat.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Application.App.CommandHandler
{
    public class UserReadMessageCommandHandler : IRequestHandler<UserReadMessageCommand, bool>
    {
        private MessageService _messageService;

        public UserReadMessageCommandHandler(MessageService messageService)
        {
            _messageService = messageService;
        }

        public Task<bool> Handle(UserReadMessageCommand request, CancellationToken cancellationToken)
        {
            _messageService.ReadMessage(request.MessageId, request.UserId);

            return Task.FromResult(true);
        }
    }
}
