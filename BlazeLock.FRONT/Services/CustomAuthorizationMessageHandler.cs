using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazeLock.FRONT.Services;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigation, IConfiguration config)
        : base(provider, navigation)
    {
        var apiEndpoint = config["WebAPI:Endpoint"] ?? throw new Exception("WebAPI:Endpoint missing");
        var apiClientId = config["AzureAd:apiClientId"];

        var scope = $"api://{apiClientId}/.default";

        ConfigureHandler(
            authorizedUrls: new[] { apiEndpoint },
            scopes: new[] { scope }
        );
    }
}