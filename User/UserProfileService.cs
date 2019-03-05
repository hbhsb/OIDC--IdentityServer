using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;

namespace IdentityServer.User
{
    public class UserProfileService: IProfileService
    {
        protected readonly UserStore Users;

        public UserProfileService(UserStore users)
        {
            Users = users;
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.RequestedClaimTypes.Any())
            {
                TestUser testUser = Users.FindBySubjectId(context.Subject.GetSubjectId());
                if (testUser != null)
                {
                    context.AddRequestedClaims(testUser.Claims);
                }
            }
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
