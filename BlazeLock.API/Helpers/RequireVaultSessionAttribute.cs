using BlazeLock.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Helpers
{
    public class RequireVaultSessionAttribute : TypeFilterAttribute
    {
        public RequireVaultSessionAttribute() : base(typeof(VaultSessionFilter))
        {
        }
    }
}
