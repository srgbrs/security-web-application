namespace web_storage.Areas.Identity
{
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    
    public class EncodedUserStore<TUser> : UserStore<TUser> where TUser : IdentityUser, new()
    {
        public EncodedUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}