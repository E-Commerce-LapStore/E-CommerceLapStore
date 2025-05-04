using LapStore.BLL.DependencyInjections;

namespace LapStore.API
{
    public class Program
    {
        public static void Main(string[] args)
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
            #endregion



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
