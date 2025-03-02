using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Idsrv4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources([
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email()
                ])
                .AddInMemoryApiScopes(new List<ApiScope>
                {
                    new ApiScope("internal-api")
                })
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource("internal-api", "Internal API")
                    {
                        Scopes = { "internal-api" }
                    }
                })
                .AddInMemoryClients([
                    new Client
                     {
                        ClientId = "internal-api",
                        ClientName = "Interactive client with short token lifetime (Code with PKCE)",

                        RedirectUris = { "https://localhost:5200/signin-oidc" },

                        ClientSecrets = { new Secret("secret".Sha256()) },
                        RequireConsent = false,

                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true,
                        AllowedScopes = { "openid", "profile", "email", "internal-api" },

                        AllowOfflineAccess = true,
                        AccessTokenType = AccessTokenType.Jwt,

                        RefreshTokenUsage = TokenUsage.ReUse,
                        RefreshTokenExpiration = TokenExpiration.Sliding,

                        AccessTokenLifetime = 20

                     }
                ])
                .AddTestUsers([
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "admin",
                        Password = "admin",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Ronda Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Ronda"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "RondaSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "https://rondasmith.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                            {
                                street_address = "One Hacker Way",
                                locality = "Heidelberg",
                                postal_code = 69118,
                                country = "Germany"
                            }), IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    }
                ])
                .AddDeveloperSigningCredential(persistKey: false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            
            app.UseHttpsRedirection();
        }
    }
}
