using System;
using System.Net;
using AspNetCore.Identity.Mongo.Entities;
using AspNetCore.Identity.Mongo.Repository;
using AspNetCore.Identity.Mongo.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Mongo
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentitySettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOptions()
                .Configure<MongoDbSettings>(configuration.GetSection("MongoDb"))
                .Configure<IdentitySettings>(configuration.GetSection("IdentitySettings"));
        }

        public static IdentityBuilder AddIdentitySetup(this IServiceCollection services, IdentitySettings settings)
        {
            return services
                .AddIdentityCore<IdentityUserEntity>(options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;
                    
                    // User settings.
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-@.&_";

                    options.SignIn.RequireConfirmedPhoneNumber = settings.RequireConfirmedPhoneNumber;
                    options.SignIn.RequireConfirmedEmail = settings.RequireConfirmedEmail;
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
                    options.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultPhoneProvider;
                    options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.User.RequireUniqueEmail = true;
                })
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore>()
                .AddUserManager<UserManager<IdentityUserEntity>>()
                .AddSignInManager();
        }

        public static IdentityBuilder AddDefaultIdentitySetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentitySettings(configuration);
            var identitySettings = configuration.GetSection("IdentitySettings").Get<IdentitySettings>();
            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = IdentityConstants.ApplicationScheme;
                    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
            .AddIdentityCookies(o => { });

            return services
                .AddRepositories()
                .AddTransient<IIdentityRepository<IdentityUserEntity>, IdentityUserRepository>()
                .AddTransient<ILookupNormalizer, UpperInvariantLookupNormalizer>()
                .AddTransient<IUserStore<IdentityUserEntity>, UserStore>()
                .AddIdentitySetup(identitySettings)
                .AddDefaultTokenProviders();
        }
    }
}
