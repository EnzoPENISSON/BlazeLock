using BlazeLock.API.Controllers;
using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.API.Services;
using Microsoft.EntityFrameworkCore;
using static BlazeLock.API.Services.UtilisateurService;

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

app.MapCoffreEndpoints();

app.MapEntreeEndpoints();

app.MapHistoriqueEntreeEndpoints();

app.MapLogEndpoints();

app.Run();
