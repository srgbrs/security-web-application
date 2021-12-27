namespace web_storage.Areas.Identity
{
    using Sodium;

    public class ArgonSha3PasswordHasherOptions
    {
        public int Sha3BitLength = 256;
        
        public PasswordHash.StrengthArgon ArgonStrength = PasswordHash.StrengthArgon.Interactive;
    }
}