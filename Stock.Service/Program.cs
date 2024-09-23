
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Service.Consumers;
using Stock.Service.Models.Contexts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);

        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEvent, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<StockApiDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

var host = builder.Build();

host.Run();
