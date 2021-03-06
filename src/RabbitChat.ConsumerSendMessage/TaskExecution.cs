using MediatR;
using RabbitChat.Application.App.Command;
using System;
using System.Threading.Tasks;

namespace RabbitChat.ConsumerSendMessage
{
    public class TaskExecution
    {
        private readonly IMediator _mediator;

        public TaskExecution(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(SendMessageCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response)
            {
                throw new Exception("Erro!");
            }
        }
    }
}
