using System;

namespace RabbitChat.Domain
{
    /// <summary>
    /// Basic representation of a database entity.
    /// </summary>
    [Serializable]
    public class Entity
    {
        /// <summary>
        /// Databse record id.
        /// </summary>
        public Guid Id { get; set; }
    }
}
