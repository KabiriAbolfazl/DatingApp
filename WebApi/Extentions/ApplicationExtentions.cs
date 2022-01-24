using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Services;

namespace WebApi.Extentions
{
    public static class ApplicationExtentions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(option =>
            {
                option.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });
            return services;
        }
    }
}
