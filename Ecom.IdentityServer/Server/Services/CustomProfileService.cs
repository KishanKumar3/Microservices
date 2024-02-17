using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Server.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CustomProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public  async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var allClaims = await _userManager.GetClaimsAsync(user);
            allClaims.Add(new Claim("user-id", user.Id));

            var requiredClaims = allClaims.Where(claim => claim.Type == "user-id" || claim.Type == "role").ToList();

            context.IssuedClaims.AddRange(requiredClaims);

        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
