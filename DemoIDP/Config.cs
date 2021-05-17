using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoIDP
{
    public static class Config
    {
        #region Users
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "main st.",
                    locality = "Riyadh",
                    postal_code = 13515,
                    country = "Saudi Arabia"
                };

                return new List<TestUser>
        {
            new TestUser
          {
            SubjectId = "666666",
            Username = "assaf",
            Password = "assaf",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Ahmed Assaf"),
              new Claim(JwtClaimTypes.GivenName, "Ahmed"),
              new Claim(JwtClaimTypes.FamilyName, "Assaf"),
              new Claim(JwtClaimTypes.Email, "AhmedAssaf@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://assaf.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "818727",
            Username = "alice",
            Password = "alice",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Alice Smith"),
              new Claim(JwtClaimTypes.GivenName, "Alice"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "88421113",
            Username = "bob",
            Password = "bob",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Bob Smith"),
              new Claim(JwtClaimTypes.GivenName, "Bob"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "user"),
              new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          }
        };
            }
        }
        #endregion

        public static IEnumerable<IdentityResource> IdentityResources => new[]
        {
            new IdentityResources.OpenId(),

            //new IdentityResource(
            //name: "openid",
            //userClaims: new[] { "sub" },
            //displayName: "Your user identifier"),

            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            },

            new IdentityResource(
                name: "profile",
                userClaims: new[] { "name", "email", "website","given_name","family_name","address"},
                displayName: "Your profile data"){ Enabled=true, ShowInDiscoveryDocument=true},
            //new IdentityResources.Profile(),
        };

        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("weatherapi.read"),
            new ApiScope("weatherapi.write","write",new[]{ "role"}),
        };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
          new ApiResource("weatherapi")
          {
            Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
            ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
            UserClaims = new List<string> {"role"}
          }
        };

        public static IEnumerable<Client> Clients => new[]
          {
            // m2m client credentials flow client
            new Client
            {
              ClientId = "m2m.client",
              ClientName = "Client Credentials Client",

              AllowedGrantTypes = GrantTypes.ClientCredentials,
              ClientSecrets = {new Secret("secret".Sha256())},
              //ClientSecrets = {new Secret(){
              //    Type = IdentityServerConstants.SecretTypes.JsonWebKey,
              //    Value = ""
              //} },

              AllowedScopes = { "weatherapi.read", "weatherapi.write"}
            },

            // interactive client using code flow + pkce
            new Client
            {
              ClientId = "interactive",
              ClientSecrets = {new Secret("secret".Sha256())},

              AllowedGrantTypes = GrantTypes.Code,

              RedirectUris = {"https://localhost:5444/signin-oidc"},
              FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
              PostLogoutRedirectUris = {"https://localhost:5444/signout-callback-oidc"},

              AllowOfflineAccess = true,
              AllowedScopes = {"openid", "profile", "weatherapi.read"},
              RequirePkce = true,
              RequireConsent = true,
              AllowPlainTextPkce = false
            },
          };
    }
}
