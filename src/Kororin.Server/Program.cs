using Korirn.Server.Model.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<RoomContextRepository>();

// Add services to the container.
builder.Services.AddMagicOnion();
builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapMagicOnionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();