namespace web_storage.Areas.Identity
{
    using Microsoft.AspNetCore.Identity;

    public class WebStorageUser : IdentityUser
    {
        public string EncryptedPhoneNumber { get; set; }
    }
}