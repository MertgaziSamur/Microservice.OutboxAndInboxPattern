using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Context;
using Order.API.ViewModels;
using Shared;
using Shared.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddDbContext<OrderApiDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

var app = builder.Build();


app.MapPost("/create-order", async (CreateOrderVM model, OrderApiDbContext _context, ISendEndpointProvider sendEndpointProvider) =>
{
    Order.API.Models.Order order = new Order.API.Models.Order()
    {
        BuyerId = Guid.Parse(model.BuyerId),
        OrderItems = model.OrderItems.Select(oi => new Order.API.Models.OrderItem()
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = oi.ProductId

        }).ToList(),
        CreatedDate = DateTime.UtcNow,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count),
    };

    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage()
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = oi.ProductId

        }).ToList()
    };

    #region With Not OutboxPattern
    //var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEvent}"));

    //await sendEndpoint.Send<OrderCreatedEvent>(orderCreatedEvent);
    #endregion

    #region With OutboxPattern
    OrderOutbox orderOutbox = new OrderOutbox()
    {
        OccuredOn = DateTime.UtcNow,
        ProcessedDate = null,
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        Type = nameof(OrderCreatedEvent)
    };

    await _context.OrderOutboxes.AddAsync(orderOutbox);
    await _context.SaveChangesAsync();
    #endregion

});

app.UseSwagger();
app.UseSwaggerUI();


app.Run();
