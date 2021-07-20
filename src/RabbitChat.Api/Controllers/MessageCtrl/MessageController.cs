using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitChat.Api.Controllers.MessageCtrl;
using RabbitChat.Infra.AmqpAdapters.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public void Post(CreateMessageDto dto, [FromServices] SimpleAmqpRpc simpleAmqpRpc)
        {
            simpleAmqpRpc.FireAndForget<Object>(
                exchangeName: "",
                routingKey: "rabbit_chat_message_queue",
                requestModel: dto
           );
        }

    }
}
