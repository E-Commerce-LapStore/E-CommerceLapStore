namespace LapStore.Web.DependencyInjection
{
    public static class GeneralRegisterationDependencyInjection
    {
        public static IServiceCollection AddGeneralDependencyInjection(this IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Remove the singleton HttpClient registration
            // services.AddSingleton<HttpClient>(sp => {...});

            // Add HttpClient factory with named client and pre-configured base address
            services.AddHttpClient("ApiClient", (sp, client) => {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["ApiSettings:ApiBaseUrl"]);
            });

            // Add Session support
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}