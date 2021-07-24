using MediatR;
using System;

namespace RabbitChat.Application.App.Command
{
    public class LoadMessagesCommand : IRequest<bool>
    {

        public DateTime? DateRegisterLastMessage { get; set; }

    }
}
