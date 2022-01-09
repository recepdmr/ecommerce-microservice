using BasketService.Api.Application.Repositories;
using BasketService.Api.Application.Services;
using BasketService.Api.EventHandlers;
using BasketService.Api.Events;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repositories;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();

builder.Services.AddSingleton<IEventBus>((sp) =>
{
    var eventBusConfig = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ,
        EventNameSuffix = "IntegrationEvent",
        Connection = new ConnectionFactory(),
        DeleteEventSuffix = true,
    };
    return EventBusFactory.Create(eventBusConfig, sp);
});
builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureRedis(configuration);
builder.Services.ConfigureConsul(configuration);
builder.Services.ConfigureJwt(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime);

var eventBus = app.Services.GetRequiredService<IEventBus>();

await eventBus.SubscibeAsync<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
