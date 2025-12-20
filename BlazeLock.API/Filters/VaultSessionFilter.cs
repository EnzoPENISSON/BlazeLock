using BlazeLock.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BlazeLock.API.Filters
{

    public class VaultSessionFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache _cache;

        public VaultSessionFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var (userId, errorResult) = user.GetCurrentUserId();

            if (!context.RouteData.Values.TryGetValue("idCoffre", out var coffreIdObj) ||
                !Guid.TryParse(coffreIdObj.ToString(), out Guid coffreId))
            {
                context.Result = new BadRequestObjectResult("Vault ID is missing or invalid in the route.");
                return;
            }

            string sessionKey = $"Session_{userId}_{coffreId}";

            Console.WriteLine($"[FILTER] Checking Cache for: '{sessionKey}'");

            if (!_cache.TryGetValue(sessionKey, out byte[] encryptionKey))
            {
                Console.WriteLine($"[FILTER] ❌ Key not found!");

                context.Result = new ObjectResult("Vault is locked...") { StatusCode = 403 };
                return;
            }

            context.HttpContext.Items["VaultKey"] = encryptionKey;

            await next();
        }
    }
}
