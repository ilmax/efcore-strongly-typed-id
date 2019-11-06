using System;

namespace StronglyTypedId
{
    public class Order
    {
        /// <summary>
        /// The PK of this order
        /// </summary>
        public OrderId Id { get; private set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}