using System;

namespace RabbitChat.Domain.Entities
{
    public class Message: Entity
    {
        public string Text { get; set; }

        public DateTime DateRegister { get; set; }

        public User FromUser { get; set; }

        public User ToUser { get; set; }

        public bool Received { get; set; }

        public bool Read { get; set; }
    }
}
