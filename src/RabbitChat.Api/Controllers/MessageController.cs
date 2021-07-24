using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitChat.Application.App.Command;
using RabbitChat.Application.SignalR;
using RabbitChat.Infra.AmqpAdapters.Rpc;
using System;

namespace RabbitChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
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
        }

        [HttpPost("LoadMessages")]
        public void ReadMessage(LoadMessagesCommand dto, [FromServices] SimpleAmqpRpc simpleAmqpRpc)
        {
            simpleAmqpRpc.FireAndForget<Object>(
                exchangeName: "",
                routingKey: "rabbit_chat_load_messages_queue",
                requestModel: dto
           );
        }

    }
}
