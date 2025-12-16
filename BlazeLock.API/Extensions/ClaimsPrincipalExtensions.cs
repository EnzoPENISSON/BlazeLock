using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Extensions
{
    /// <summary>
    /// Fournit des méthodes d'extension pour la classe <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Récupère l'ID de l'utilisateur connecté à partir des claims.
        /// Cette méthode gère plusieurs types de claims pour assurer la compatibilité
        /// avec différents fournisseurs d'identité (par exemple, Azure AD B2C).
        /// </summary>
        /// <param name="user">L'objet ClaimsPrincipal représentant l'utilisateur actuel.</param>
        /// <returns>
        /// Un tuple contenant l'ID de l'utilisateur sous forme de Guid et un IActionResult.
        /// Si la récupération réussit, l'IActionResult est null.
        /// En cas d'échec (claim manquant ou invalide), l'ID est Guid.Empty et l'IActionResult contient l'erreur appropriée (Unauthorized ou BadRequest).
        /// </returns>
        public static (Guid userId, IActionResult? error) GetCurrentUserId(this ClaimsPrincipal user)
        {
            // Tente de récupérer l'ID utilisateur à partir du claim 'objectidentifier' utilisé par Azure AD.
            var userIdClaim = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            // Si le premier claim n'est pas trouvé, essaie avec 'oid', un autre claim courant pour l'ID d'objet.
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = user.FindFirstValue("oid");
            }

            // En dernier recours, essaie avec 'NameIdentifier', un claim standard.
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // Si aucun des claims pertinents n'est trouvé, l'utilisateur ne peut pas être identifié.
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return (Guid.Empty, new UnauthorizedObjectResult("Impossible de récupérer l'ID utilisateur (Claims 'oid' ou 'NameIdentifier' manquants)."));
            }

            // Tente de convertir la valeur du claim en Guid.
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                // Log l'erreur pour le débogage si la conversion échoue.
                Console.WriteLine($"[AUTH ERROR] Valeur reçue non-GUID : {userIdClaim}");
                return (Guid.Empty, new BadRequestObjectResult($"L'ID utilisateur reçu n'est pas un GUID valide. Valeur reçue : {userIdClaim}"));
            }

            // Si tout réussit, retourne l'ID de l'utilisateur et une erreur nulle.
            return (userId, null);
        }
    }
}
