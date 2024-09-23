using MassTransit;
using Order.Outbox.Table.Publisher.Service.Entities;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Order.Outbox.Table.Publisher.Service.Jobs
{
    public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            OrderOutboxSingletonDatabase.DataReaderBusy();

            if (OrderOutboxSingletonDatabase.DataReaderState)
            {
                List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM OrderOutboxes WHERE ProcessedDate IS NULL ORDER BY OccuredOn ASC")).ToList();

                foreach (var orderOutbox in orderOutboxes)
                {
                    if (orderOutbox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);

                        if (orderCreatedEvent != null)
                        {
                            await publishEndpoint.Publish(orderCreatedEvent);
                            OrderOutboxSingletonDatabase.ExecuteAsync($@"UPDATE OrderOutboxes SET ProcessedDate = GETDATE() WHERE Id = '{orderOutbox.Id}'");
                        }
                    }
                }

                OrderOutboxSingletonDatabase.DataReaderReady();
                await Console.Out.WriteLineAsync("Order Outbox Table Checked!");
            }
        }
    }
}
