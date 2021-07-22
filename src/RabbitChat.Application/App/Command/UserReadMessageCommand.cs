using MediatR;
using System;

namespace RabbitChat.Application.App.Command
{
    public class UserReadMessageCommand : IRequest<bool>
    {
        public Guid MessageId { get; set; }
    }
}
