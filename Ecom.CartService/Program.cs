using Ecom.CartService.Clients;
using Ecom.CartService.Entities;
using Ecom.Common.MassTransit;
using Ecom.Common.MongoDB;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
                .AddMongoRepository<CartItem>("cartitems")
                .AddMongoRepository<CatalogItem>("catalogitems")
                .AddMongoRepository<InventoryItem>("inventoryItems")
                .AddMassTransitWithRabbitMq();

AddCatalogClient(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();

static void AddCatalogClient(WebApplicationBuilder builder)
{
    Random jitterer = new Random();

    builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5001");
    })
    .AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        5,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                        + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000))
        ))
    .AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        3,
        TimeSpan.FromSeconds(15)
        ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}