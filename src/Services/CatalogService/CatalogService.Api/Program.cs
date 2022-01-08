using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureDbContext(configuration);
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

var seeder = app.Services.GetRequiredService<CatalogDbContextSeeder>();
seeder.SeedAsync().Wait();
app.RegisterWithConsul(app.Lifetime);

await app.RunAsync();
