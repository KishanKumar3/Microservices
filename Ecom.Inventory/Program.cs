using Ecom.Common.MassTransit;
using Ecom.Common.MongoDB;
using Ecom.Common.Settings;
using Ecom.Inventory.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// var allowedOriginsPolicy = "simpleSpecification";
var serviceSettings = builder.Configuration.GetSection("ServiceSettings").Get<ServiceSettings>();
builder.Services.AddMongo().AddMongoRepository<InventoryItem>("InventoryItems").AddMassTransitWithRabbitMq();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: allowedOriginsPolicy, builder =>
//     {
//         builder.WithOrigins("https://localhost:5001").AllowAnyHeader().AllowAnyMethod();
//     });
// });
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

// app.UseCors(allowedOriginsPolicy);
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
