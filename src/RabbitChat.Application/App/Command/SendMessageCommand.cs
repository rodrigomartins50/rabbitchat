using MediatR;
using System;

namespace RabbitChat.Application.App.Command
{
    public class SendMessageCommand: IRequest<bool>
    {
        public string Text { get; set; }

        public String FromUser { get; set; }

        public DateTime DateRegister { get; set; }
    }
}
