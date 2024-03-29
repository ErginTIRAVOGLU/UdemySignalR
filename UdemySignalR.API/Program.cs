using Microsoft.EntityFrameworkCore;
using UdemySignalR.API.Hubs;
using UdemySignalR.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        //builder.AllowAnyOrigin();
        builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true);
        /*builder.WithOrigins("https://localhost:7119/").AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true);
         */
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.MapHub<MyHub>("/MyHub");

app.UseHttpsRedirection();



app.UseAuthorization();

app.MapControllers();

app.Run();
