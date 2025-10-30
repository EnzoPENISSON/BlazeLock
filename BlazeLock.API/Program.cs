using BlazeLock.API.Controllers;
using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<BlazeLockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

builder.Services.AddScoped<IPartageRepository, PartageRepository>();
builder.Services.AddScoped<IPartageService, PartageService>();

builder.Services.AddScoped<ICoffreRepository, CoffreRepository>();
builder.Services.AddScoped<ICoffreService, CoffreService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();


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

app.MapEntreeEndpoints();

app.MapHistoriqueEntreeEndpoints();

app.Run();
