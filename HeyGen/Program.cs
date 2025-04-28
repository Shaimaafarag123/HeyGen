// Program.cs
using HeyGen.Data;
using HeyGen.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

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
    var baseUrl = builder.Configuration["HeyGen:BaseUrl"];
    var apiKey = builder.Configuration["HeyGen:ApiKey"];

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);
    client.Timeout = TimeSpan.FromSeconds(30);
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