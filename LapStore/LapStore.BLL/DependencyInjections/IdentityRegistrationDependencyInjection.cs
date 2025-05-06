using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace LapStore.BLL.DependencyInjections
{
    public static class IdentityRegisterationDependencyInjection
    {
        public static IServiceCollection AddIdentityDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Identity services
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<LapStoreDbContext>()
            .AddDefaultTokenProviders();

            // Configure cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "mahmoud";
                option.DefaultChallengeScheme = "mahmoud";
            }).AddJwtBearer("mahmoud", options =>
            {
                var securitykeystring = configuration.GetSection("SecretKey").Value;
                var securtykeyByte = Encoding.ASCII.GetBytes(securitykeystring);
                var securityKey = new SymmetricSecurityKey(securtykeyByte);

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = securityKey,
                    //ValidAudience = "url" ,
                    //ValidIssuer = "url",
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}