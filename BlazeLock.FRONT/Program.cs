using BlazeLock.FRONT;
using BlazeLock.FRONT.Core;
using BlazeLock.FRONT.Services;
using BlazeLock.FRONT.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string apiEndpoint = builder.Configuration.GetValue<string>("WebAPI:Endpoint")
    ?? throw new InvalidOperationException("WebAPI:Endpoint is not configured");

string apiClientId = builder.Configuration["AzureAd:ClientId"]!;
string apiScope = $"{apiClientId}/access_as_user";

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped<VaultKeyStore>();
// View Models
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<CoffreDetailViewModel>();

builder.Services.AddHttpClient<UserAPIService>(client =>
    client.BaseAddress = new Uri(apiEndpoint))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IEntreeAPIService, EntreeAPIService>(client =>
    client.BaseAddress = new Uri(apiEndpoint))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";

    options.ProviderOptions.DefaultAccessTokenScopes.Add(apiScope);
})
.AddAccountClaimsPrincipalFactory<CustomAccountClaimsPrincipalFactory>();

await builder.Build().RunAsync();