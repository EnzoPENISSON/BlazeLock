namespace BlazeLock.FRONT.Core
{

    using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
    using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
    using System.Security.Claims;
    using System.Text.Json;

    /*
     * Cette classe permet de corriger un problème dans la structure des Claims de MSAL.
     * MSAL retourne le rôle de l'utilisateur sous un nom de rôle "roles" au lieu de "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".
     * Ceci ne permet pas d'utiliser [Authorize( Roles = "nom du rôle")] ou authenticationState.User.IsInRole(Roles.User)
     * Cette classe va être appelée par MSAL lors de la création d'un utilisateur (notion de Factory).
     * A ce moment, on recréé un Claims avec le bon nom pour les rôles.
     */
    public class CustomAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
    {
        #region Methods

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            ClaimsPrincipal user = await base.CreateUserAsync(account, options);

            if ((user.Identity?.IsAuthenticated ?? false) == false)
            {
                return user;
            }

            if (user.Identity is not ClaimsIdentity identity)
            {
                return user;
            }

            List<Claim> roleClaims = identity.FindAll("roles").ToList();

            if (roleClaims is null || roleClaims.Count == 0)
            {
                return user;
            }

            foreach (Claim roleClaim in roleClaims)
            {
                try
                {
                    string[] roleNames = JsonSerializer.Deserialize<string[]>(roleClaim.Value) ?? [];

                    foreach (string roleName in roleNames)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }
                }
                catch (Exception)
                {

                }
            }

            return user;
        }

        #endregion
    }

}
