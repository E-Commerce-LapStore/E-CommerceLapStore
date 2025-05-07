using LapStore.BLL.DependencyInjections;

namespace LapStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddServiceDependencyInjection(builder)
                            .AddRepositoryDependencyInjection()
                            .AddDbContextDependencyInjection(builder.Configuration)
                            .AddIdentityDependencyInjection(builder.Configuration)
                            .AddGeneralDependencyInjection();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
