namespace BlazeLock.FRONT.Components.Services
{
    using Microsoft.AspNetCore.Components.Authorization;

    public interface IUserService
    {
        Task<Guid?> GetUserIdAsync();
    }

    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Guid?> GetUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var objectIdClaim = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
                return objectIdClaim != null && Guid.TryParse(objectIdClaim.Value, out Guid userId) ? userId : null;
            }

            return null;
        }
    }


}
