using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.Data
{
    public class MyCrapGrantValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var rhubarb = context.Request.Raw.Get("rhubarb");
            var custard = context.Request.Raw.Get("custard");
            var music = context.Request.Raw.Get("music");

            if (string.IsNullOrWhiteSpace(rhubarb) || string.IsNullOrWhiteSpace(custard) ||
                string.IsNullOrWhiteSpace(music))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return Task.FromResult(false);
            }

            if (bool.Parse(rhubarb) && bool.Parse(custard) && music == "ska")
            {
                var sub = "ThisIsNotGoodSub";
                context.Result = new GrantValidationResult(sub,"my_crap_grant");
                Task.FromResult(0);
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient);
            return Task.FromResult(false);
        }

        public string GrantType => "my_crap_grant";
    }
}
