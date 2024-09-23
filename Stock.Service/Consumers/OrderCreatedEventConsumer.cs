using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.Service.Models.Contexts;
using Stock.Service.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumer(StockApiDbContext stockApiDbContext) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            await stockApiDbContext.OrderInboxes.AddAsync(new Models.Entities.OrderInbox()
            {
                Processed = false,
                Payload = JsonSerializer.Serialize(context.Message)
            });

            await stockApiDbContext.SaveChangesAsync();

            List<OrderInbox> orderInboxes = await stockApiDbContext.OrderInboxes.Where(i => i.Processed == false).ToListAsync();

            foreach (var orderInbox in orderInboxes)
            {
                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);

                await Console.Out.WriteLineAsync($"{orderCreatedEvent.OrderId} The Stock Transactions Of The Order With The Value Have Been Completed Successfully!");

                orderInbox.Processed = true;

                await stockApiDbContext.SaveChangesAsync();
            }
        }
    }
}
