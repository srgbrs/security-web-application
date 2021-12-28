using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using web_storage.Areas.Identity;
using web_storage.Data;

namespace web_storage
{
    using System.IO;
    using Areas;
    using Azure.Identity;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using Sodium;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<WebStorageUser>(options => { options.SignIn.RequireConfirmedAccount = true; })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddPasswordValidator<LoginPasswordMatchingValidator<WebStorageUser>>()
                .AddPasswordValidator<CommonPasswordUsageValidator<WebStorageUser>>()
                .AddUserStore<EncodedUserStore<WebStorageUser>>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;

                options.SignIn.RequireConfirmedEmail = true;
            });
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider,
                RevalidatingIdentityAuthenticationStateProvider<WebStorageUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddSingleton<WeatherForecastService>();

            services.AddScoped<IPasswordHasher<WebStorageUser>, ArgonSha3PasswordHasher<WebStorageUser>>();
            services.Configure<ArgonSha3PasswordHasherOptions>(options =>
            {
                options.ArgonStrength = PasswordHash.StrengthArgon.Moderate;
                options.Sha3BitLength = 512;
            });

            var connectionString =
                "BlobEndpoint=https://kpisadsstorage.blob.core.windows.net/;QueueEndpoint=https://kpisadsstorage.queue.core.windows.net/;FileEndpoint=https://kpisadsstorage.file.core.windows.net/;TableEndpoint=https://kpisadsstorage.table.core.windows.net/;SharedAccessSignature=sv=2020-08-04&ss=bfqt&srt=sco&sp=rwdlacupitfx&se=2021-12-28T17:15:57Z&st=2021-12-28T09:15:57Z&sip=0.0.0.0-255.255.255.255&spr=https,http&sig=rzwaSevbsGzRdi6vBBSynL%2BtYMh%2FVETsDoACe4X81OI%3D";
            var containerName = "key-web-storage-container";
            var blobName = "database-key-blob";

            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(connectionString, containerName, blobName)
                .ProtectKeysWithAzureKeyVault(
                    new Uri(
                        "https://kpi-sads-key-vault.vault.azure.net/keys/sads-vault-key-rsa/502643d5b0784121ac64140bb05ab94f"),
                    new DefaultAzureCredential())
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}