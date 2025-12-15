using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static (Guid userId, IActionResult? error) GetCurrentUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = user.FindFirstValue("oid");
            }

            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return (Guid.Empty, new UnauthorizedObjectResult("Impossible de récupérer l'ID utilisateur (Claims 'oid' ou 'NameIdentifier' manquants)."));
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                Console.WriteLine($"[AUTH ERROR] Valeur reçue non-GUID : {userIdClaim}");
                return (Guid.Empty, new BadRequestObjectResult($"L'ID utilisateur reçu n'est pas un GUID valide. Valeur reçue : {userIdClaim}"));
            }

            return (userId, null);
        }
    }
}
