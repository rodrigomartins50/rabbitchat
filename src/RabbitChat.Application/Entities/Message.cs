using System;

namespace RabbitChat.Domain.Entities
{
    public class Message: Entity
    {
        public string Text { get; set; }

        public DateTime DateRegister { get; set; }

        public String FromUser { get; set; }
    }
}
