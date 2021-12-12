using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.Api.EventHandlers;
using PaymentService.Api.Events;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
builder.Services.AddSingleton<IEventBus>((sp) =>
{
    var eventBusConfig = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "PaymentService",
        EventBusType = EventBusType.RabbitMQ,
        EventNameSuffix = "IntegrationEvent",
        Connection = new ConnectionFactory(),
        DeleteEventSuffix = true,
    };
    return EventBusFactory.Create(eventBusConfig, sp);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var eventBus = app.Services.GetRequiredService<IEventBus>();

await eventBus.SubscibeAsync<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();

await app.RunAsync();
