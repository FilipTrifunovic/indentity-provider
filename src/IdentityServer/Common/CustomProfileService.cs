
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer.Common
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var claims = await _userManager.GetClaimsAsync(user);

            // Add additional claims from ApplicationUser properties
            claims.Add(new Claim("firstName", user.FirstName ?? string.Empty));
            claims.Add(new Claim("lastName", user.LastName ?? string.Empty));
            claims.Add(new Claim("address", user.Address ?? string.Empty));
            claims.Add(new Claim("country", user.Country ?? string.Empty));

            context.IssuedClaims.AddRange(claims);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = _userManager.GetUserAsync(context.Subject).Result;
            context.IsActive = user != null;
            return Task.CompletedTask;
        }
    }
}
