namespace web_storage.Areas.Identity
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public class LoginPasswordMatchingValidator<TUser> : IPasswordValidator<TUser> where TUser : IdentityUser
    {
        private const StringComparison StringComparisonMode = StringComparison.OrdinalIgnoreCase;
            
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (string.Equals(user.UserName, password, StringComparisonMode) ||
                string.Equals(user.Email, password, StringComparisonMode))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError()
                {
                    Code = "LoginPasswordMatch",
                    Description = "Neither username, nor password can't be used as password"
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}