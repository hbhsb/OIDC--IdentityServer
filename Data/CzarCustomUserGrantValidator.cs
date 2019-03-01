using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.Data
{
    public class CzarCustomUserGrantValidator: IExtensionGrantValidator
    {
        public string GrantType => "CzarCustomUser";

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userName = context.Request.Raw.Get("czar_name");
            var userPassword = context.Request.Raw.Get("czar_password");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
            //校验登录
            var result = userName == userPassword;
            if (result != true)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
            //添加指定的claims
            GrantValidationResult grantValidationResult = new GrantValidationResult(
                subject: userName,
                authenticationMethod: GrantType,
                claims: new List<Claim>()
                {
                    new Claim("pwd",userPassword)
                });

            context.Result = grantValidationResult;
            return Task.CompletedTask;
        }
    }
}
