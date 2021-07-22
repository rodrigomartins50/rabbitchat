using MediatR;
using System;

namespace RabbitChat.Application.App.Command
{
    public class SendMessageCommand: IRequest<bool>
    {
        public string Text { get; set; }

        public Guid FromUserId { get; set; }

        public Guid ToUserId { get; set; }

        public DateTime DateRegister { get; set; }
    }
}
