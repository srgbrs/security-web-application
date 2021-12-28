namespace web_storage.Areas.Identity
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
    using Microsoft.AspNetCore.DataProtection.KeyManagement;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    
    public class EncodedUserStore<TUser> : UserStore<TUser> where TUser : IdentityUser, new()
    {
        private readonly IAuthenticatedEncryptor encryptor;
        
        public EncodedUserStore(IKeyManager keyManager, ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            encryptor = keyManager.GetAllKeys().First().CreateEncryptor();
        }

        public override async Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrEmpty(user.PhoneNumber)) return user.PhoneNumber;
            
            var encryptedBytes = Convert.FromBase64String(user.PhoneNumber);
            var rawBytes = encryptor.Decrypt( encryptedBytes, ArraySegment<byte>.Empty);
            return Encoding.UTF8.GetString(rawBytes);
        }

        public override async Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                await base.SetPhoneNumberAsync(user, phoneNumber, cancellationToken);
                return;
            }
            
            var rawBytes = Encoding.UTF8.GetBytes(phoneNumber);
            var encryptedBytes = encryptor.Encrypt(rawBytes, ArraySegment<byte>.Empty);
            user.PhoneNumber = Convert.ToBase64String(encryptedBytes);
        }
    }
}