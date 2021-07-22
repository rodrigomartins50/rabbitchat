using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

    }
}
