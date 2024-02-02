using Ecom.Common.MassTransit;
using Ecom.Common.MongoDB;
using Ecom.Common.Settings;
using Ecom.ProductService.Entities;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var serviceSettings = builder.Configuration.GetSection("ServiceSettings").Get<ServiceSettings>();

builder.Services.AddMongo()
                .AddMongoRepository<Item>("items")
                .AddMassTransitWithRabbitMq();


builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

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
