using BlazeLock.API.Controllers;
using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication("Bearer")
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ROLE_ADMIN", policy =>
        policy.RequireClaim("scp", "ROLE_ADMIN"));

    options.AddPolicy("ROLE_USER", policy =>
        policy.RequireClaim("scp", "ROLE_USER"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BlazeLockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapUtilisateurEndpoints();

app.MapCoffreEndpoints();

app.MapEntreeEndpoints();

app.MapHistoriqueEntreeEndpoints();

app.MapLogEndpoints();

app.MapPartageEndpoints();

app.Run();
