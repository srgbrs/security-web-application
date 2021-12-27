namespace web_storage.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public class CommonPasswordUsageValidator<TUser> : IPasswordValidator<TUser> where TUser : IdentityUser
    {
        private string DataSourceUrl => "https://raw.githubusercontent.com/danielmiessler/SecLists/master/Passwords/xato-net-10-million-passwords-10000.txt";

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var commonlyUsedPasswords = await GetCommonlyUsedPasswordsList();
            if (commonlyUsedPasswords.Contains(password))
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "CommonPasswordUsage",
                    Description = "Usage of commonly used password is forbidden. See list of commonly used passwords at OWASP SecList"
                });
            }

            return IdentityResult.Success;
        }

        private Task<List<string>> GetCommonlyUsedPasswordsList()
        {
            var tcs = new TaskCompletionSource<List<string>>();
            using (var webCl = new WebClient())
            {
                webCl.DownloadStringAsync(new Uri(DataSourceUrl));
                webCl.DownloadStringCompleted += (sender, args) => tcs.SetResult(args.Result.Split().ToList());
            }

            return tcs.Task;
        }
    }
}