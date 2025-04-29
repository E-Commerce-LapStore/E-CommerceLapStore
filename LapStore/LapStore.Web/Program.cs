using LapStore.BLL.DependencyInjections;
using LapStore.BLL.Services;
using LapStore.BLL.Services.Interfaces;
using LapStore.DAL.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace LapStore.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Register DependencyInjection
            // Add configuration services first
            builder.Services.AddConfigurationDependencyInjection(builder.Configuration);

            // Add other services
            builder.Services.AddServiceDependencyInjection()
                          .AddRepositoryDependencyInjection()
                          .AddGeneralDependencyInjection(builder.Configuration)
                          .AddIdentityDependencyInjection()
                          .AddScoped<IEmailService, EmailService>();

            // Add security headers
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            #endregion

            #region Middleware
            var app = builder.Build();

            // Configure security headers
            app.UseHsts();
            app.UseHttpsRedirection();

            // Add security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Add("Content-Security-Policy", 
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self'; " +
                    "frame-ancestors 'none';");
                await next();
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            await app.RunAsync();
            #endregion
        }
    }
}