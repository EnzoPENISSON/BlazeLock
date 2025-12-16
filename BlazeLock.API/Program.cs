using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var tenantId = builder.Configuration["AzureAd:TenantId"];
var clientId = builder.Configuration["AzureAd:ClientId"];
var scope = $"{clientId}/.default";
var frontendUrl = builder.Configuration["Front:Url"];

string? corsFrontEndpoint = builder.Configuration.GetValue<string>("CorsFrontEndpoint");

if (!string.IsNullOrWhiteSpace(corsFrontEndpoint))
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

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"];
    options.TokenValidationParameters.ValidAudiences = new[]
    {
        clientId, 
        $"api://{clientId}"
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlazeLock API", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { scope, "Access API as User" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { scope }
        }
    });
});

builder.Services.AddDbContextFactory<BlazeLockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

builder.Services.AddScoped<IPartageRepository, PartageRepository>();
builder.Services.AddScoped<IPartageService, PartageService>();

builder.Services.AddScoped<ICoffreRepository, CoffreRepository>();
builder.Services.AddScoped<ICoffreService, CoffreService>();

builder.Services.AddScoped<IEncryptService, EncryptService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<IEntreeRepository, EntreeRepository>();
builder.Services.AddScoped<IHistoriqueEntreeRepository, HistoriqueEntreeRepository>();
builder.Services.AddScoped<IEntreeService, EntreeService>();
builder.Services.AddScoped<IDossierRepository, DossierRepository>();



var app = builder.Build();

if (!string.IsNullOrWhiteSpace(corsFrontEndpoint))
{
    app.UseCors("WebAssemblyOrigin");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(clientId);
        c.OAuthUsePkce();
        c.OAuthScopeSeparator(" ");
    });
}

app.UseHttpsRedirection();

app.UseCors(policy => policy
    .WithOrigins(frontendUrl)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
