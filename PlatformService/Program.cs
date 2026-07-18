var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. THIS REPLACES Startup.ConfigureServices
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 2. THIS REPLACES Startup.Configure
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();