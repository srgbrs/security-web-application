namespace web_storage.Areas.Identity
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class EncryptedUserStore<TUser> : UserOnlyStore<TUser> where TUser : WebStorageUser, new()
    {
        public EncryptedUserStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            return base.GetPhoneNumberAsync(user, cancellationToken);
        }
    }
}