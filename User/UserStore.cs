using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Test;

namespace IdentityServer.User
{
    public class UserStore
    {
        private readonly List<TestUser> _users;

        protected readonly CISDI_TEST20180829Context dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Test.UserStore"/> class.
        /// </summary>
        /// <param name="_dbContext">The users.</param>
        public UserStore(CISDI_TEST20180829Context _dbContext)
        {
            dbContext = _dbContext;
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            var queryable = dbContext.Popedom.Select(c => c.LoginName == username && c.Pws == password);
            if (queryable.Any())
            {
                return true;
            }
            var user = FindByUsername(username);
            if (user != null)
            {
                return user.Password.Equals(password);
            }

            return false;
        }

        /// <summary>
        /// Finds the user by subject identifier.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public TestUser FindBySubjectId(string subjectId)
        {
            TestUser testUser = new TestUser();
            Popedom popedom = dbContext.Popedom.Where(c => c.Userid == subjectId).FirstOrDefault();
            if (popedom != null)
            {
                testUser.SubjectId = popedom.Userid;
                testUser.Claims=new List<Claim>()
                {
                    new Claim("name", popedom.Username),
                    new Claim("sysId",popedom.Userid),
                    new Claim("nationality","中国")
                };
            }
            return testUser;
        }

        /// <summary>
        /// Finds the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public TestUser FindByUsername(string username)
        {
            //dbContext.Popedom.AsEnumerable().(c => c.LoginName == username);
            TestUser testUser = new TestUser();
            var queryable = dbContext.Popedom.Select(c => c.LoginName == username);
            Popedom popedom = dbContext.Popedom.FirstOrDefault(c => c.Username == username);
            if (popedom != null)
            {
                testUser.SubjectId = popedom.Userid;
                testUser.Claims = new List<Claim>()
                {
                    new Claim("name", popedom.Username),
                    new Claim("sysId",popedom.Userid),
                    new Claim("nationality","中国")
                };
            }
            return testUser;
        }

        /// <summary>
        /// Finds the user by external provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public TestUser FindByExternalProvider(string provider, string userId)
        {
            return _users.FirstOrDefault(x =>
                x.ProviderName == provider &&
                x.ProviderSubjectId == userId);
        }

        /// <summary>
        /// Automatically provisions a user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public TestUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId();

            // check if a display name is available, otherwise fallback to subject id
            var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

            // create new user
            var user = new TestUser
            {
                SubjectId = sub,
                Username = name,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = filtered
            };

            // add user to in-memory store
            _users.Add(user);

            return user;
        }
    }
}
