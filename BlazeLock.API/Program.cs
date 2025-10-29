using BlazeLock.API.Controllers;
using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);


string? corsFrontEndpoint = builder.Configuration.GetValue<string>("CorsFrontEndpoint");

//CORS permet d'autoriser le front à appeler l'API.
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

builder.Services.AddDbContext<BlazeLockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//Indique la mise en place de la police CORS créée précédement.
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

app.UseAuthorization();

app.MapControllers();

app.MapUtilisateurEndpoints();

app.MapCoffreEndpoints();

app.MapEntreeEndpoints();

app.MapHistoriqueEntreeEndpoints();

app.MapLogEndpoints();

app.MapPartageEndpoints();

app.Run();
