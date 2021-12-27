namespace web_storage.Areas.Identity
{
    using System;
    using System.Text;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Org.BouncyCastle.Crypto.Digests;
    using Sodium;

    public class ArgonSha3PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private readonly ArgonSha3PasswordHasherOptions options;
        private readonly Sha3Digest sha3;

        public ArgonSha3PasswordHasher(IOptions<ArgonSha3PasswordHasherOptions> optionsAccessor = null)
        {
            options = optionsAccessor?.Value ?? new ArgonSha3PasswordHasherOptions();
            sha3 = new Sha3Digest(options.Sha3BitLength);
        }
        
        public string HashPassword(TUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            
            return PasswordHash.ArgonHashString(GetDigest(password), options.ArgonStrength).TrimEnd('\0');
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword)) throw new ArgumentNullException(nameof(hashedPassword));
            if (string.IsNullOrWhiteSpace(providedPassword)) throw new ArgumentNullException(nameof(providedPassword));

            var digest = GetDigest(providedPassword);
            
            var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, digest);
            if (isValid && PasswordHash.ArgonPasswordNeedsRehash(hashedPassword, options.ArgonStrength))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private string GetDigest(string line)
        {
            var input = Encoding.UTF8.GetBytes(line);
            sha3.BlockUpdate(input, 0, input.Length);

            var result = new byte[options.Sha3BitLength];
            sha3.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }
    }
}