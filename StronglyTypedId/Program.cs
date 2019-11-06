using System;
using System.Linq;

namespace StronglyTypedId
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (var context = new SampleContext())
            {
                context.Database.EnsureCreated();

                var order = new Order();
                context.Add(order);
                context.SaveChanges();

                Console.WriteLine(order.Id);
            }

            using (var context = new SampleContext())
            {
                var firstOrderId = new OrderId(1);
                var order = context.Orders.SingleOrDefault(o => o.Id == firstOrderId);

                Console.WriteLine($"Order found: {(order == null ? "no" : "yes")}");
            }
        }
    }
}
