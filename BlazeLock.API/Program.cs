using BlazeLock.API.Controllers;
using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using static BlazeLock.API.Services.UtilisateurService;

var builder = WebApplication.CreateBuilder(args);


string? corsFrontEndpoint = builder.Configuration.GetValue<string>("CorsFrontEndpoint");

//CORS permet d'autoriser le front � appeler l'API.
if (string.IsNullOrWhiteSpace(corsFrontEndpoint) == false)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WebAssemblyOrigin", policy =>
        {
            policy
                .WithOrigins(corsFrontEndpoint)
                .AllowAnyMethod()
                .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-custom-header")
                .AllowCredentials();
        });
    });
}


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ROLE_ADMIN", policy =>
//        policy.RequireClaim("scp", "ROLE_ADMIN"));

//    options.AddPolicy("ROLE_USER", policy =>
//        policy.RequireClaim("scp", "ROLE_USER"));
//});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContextFactory<BlazeLockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

builder.Services.AddScoped<IPartageRepository, PartageRepository>();
builder.Services.AddScoped<IPartageService, PartageService>();

builder.Services.AddScoped<ICoffreRepository, CoffreRepository>();
builder.Services.AddScoped<ICoffreService, CoffreService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<IEntreeRepository, EntreeRepository>();
//builder.Services.AddScoped<IEntreeService, EntreeService>();


var app = builder.Build();

//Indique la mise en place de la police CORS cr��e pr�c�dement.
if (string.IsNullOrWhiteSpace(corsFrontEndpoint) == false)
{
    app.UseCors("WebAssemblyOrigin");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHistoriqueEntreeEndpoints();

app.Run();
