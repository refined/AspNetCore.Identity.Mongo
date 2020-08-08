using System;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Mongo.Jwt
{
    public static class IdentityDependencyInjection
    {
        public static IdentityCookiesBuilder AddDefaultJwtSetup(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddJwtAuthorization(configuration)
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = IdentityConstants.ApplicationScheme;
                    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies(o =>
                {
                    o.ApplicationCookie.Configure(c =>
                    {
                        c.SlidingExpiration = true;
                        c.ExpireTimeSpan = TimeSpan.FromDays(1);
#pragma warning disable 1998
                        c.Events.OnRedirectToLogin = async context
                            => context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        c.Events.OnRedirectToAccessDenied = async context
                            => context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
#pragma warning restore 1998
                    });
                });
        }
    }
}
