using MediatR;
using RabbitChat.Application.App.Command;
using RabbitChat.Domain.Entities;
using RabbitChat.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Application.App.CommandHandler
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, bool>
    {
        private MessageService _messageService;

        public SendMessageCommandHandler(MessageService messageService)
        {
            _messageService = messageService;
        }

        public Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message();

            message.Text = request.Text;
            message.DateRegister = request.DateRegister;
            //message.FromUser = new User
            //{
            //    Id = request.FromUserId
            //};
            //message.ToUser = new User
            //{
            //    Id = request.ToUserId
            //};

            _messageService.Insert(message);

            return Task.FromResult(true);
        }
    }
}
