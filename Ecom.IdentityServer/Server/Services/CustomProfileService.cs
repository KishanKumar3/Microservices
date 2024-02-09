using IdentityModel;
using IdentityServer4.Extensions;
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
            var claims = new List<Claim>
        { //adding user-id claim
            new Claim("user-id", user.Id)
        };
            context.IssuedClaims.AddRange(claims);
            
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
