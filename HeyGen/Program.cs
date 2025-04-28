// Program.cs
using HeyGen.Data;
using HeyGen.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for HeyGen API
builder.Services.AddHttpClient();

// Register the HeyGen service
builder.Services.AddScoped<IHeyGenService, HeyGenService>();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IHeyGenService, HeyGenService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HeyGen:BaseUrl"] ?? "https://api.heygen.com/v2");
    client.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["HeyGen:ApiKey"]);
});
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();