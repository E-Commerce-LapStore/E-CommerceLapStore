using LapStore.BLL.DependencyInjections;

namespace LapStore.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Register DependencyInjection
            // Add services to the container.
            // Add configuration services first
            // Add other services
            
            builder.Services.AddServiceDependencyInjection()
                            .AddRepositoryDependencyInjection()
                            .AddGeneralDependencyInjection(builder.Configuration)
                            .AddIdentityDependencyInjection();

            // Add security headers
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            #endregion

            

            var app = builder.Build();


            // Configure security headers
            app.UseHsts();
            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty; // This makes Swagger UI accessible at the root URL
                });
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Add static assets mapping within endpoints
                endpoints.MapStaticAssets();
            });


            await app.RunAsync();
        }
    }
}
