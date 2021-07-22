using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitChat.Api.Hubs;
using RabbitChat.Application.App.Command;
using RabbitChat.Infra.AmqpAdapters.Rpc;
using System;

namespace RabbitChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private IHubContext<RabbitChatHub> _hub;

        public MessageController(ILogger<MessageController> logger, IHubContext<RabbitChatHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        [HttpPost]
        public void Post(SendMessageCommand dto, [FromServices] SimpleAmqpRpc simpleAmqpRpc)
        {
            dto.DateRegister = DateTime.Now;

            simpleAmqpRpc.FireAndForget<Object>(
                exchangeName: "",
                routingKey: "rabbit_chat_message_queue",
                requestModel: dto
           );

            _hub.Clients.All.SendAsync("ReceiveNewMessage", dto.Text);
        }

        [HttpPost("ReadMessage")]
        public void ReadMessage(UserReadMessageCommand dto, [FromServices] SimpleAmqpRpc simpleAmqpRpc)
        {
            simpleAmqpRpc.FireAndForget<Object>(
                exchangeName: "",
                routingKey: "rabbit_chat_read_message_queue",
                requestModel: dto
           );
        }

    }
}
