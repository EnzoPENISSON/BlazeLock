using BlazeLock.FRONT;
using BlazeLock.FRONT.Core;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient simple compatible WASM
string apiEndpoint = builder.Configuration.GetValue<string>("WebAPI:Endpoint") ?? throw new InvalidOperationException("WebAPI is not configured");

// Auth Entra ID
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
}).AddAccountClaimsPrincipalFactory<CustomAccountClaimsPrincipalFactory>();


//Ajout d'un service WebAPIClient qui dépend d'un HttpClient
builder.Services.AddHttpClient<UserAPIService>(client => client.BaseAddress = new Uri(apiEndpoint))
    //Le HttpClient précise l'utilisation d'un MessageHandler qui se charge de transférer le jeton d'identification de l'utilisateur connecté.
    .AddHttpMessageHandler(sp =>
    {
        AuthorizationMessageHandler handler = sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(authorizedUrls: [apiEndpoint]);

        return handler;
    });

await builder.Build().RunAsync();
