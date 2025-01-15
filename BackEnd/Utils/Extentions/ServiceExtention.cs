using TarikhMaghribi.Services.Implementations;
using TarikhMaghribi.Services.Interfaces;
using static TarikhMaghribi.Services.Mail;

namespace TarikhMaghribi.Utils.Extentions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IJourFerieService, JourFerieService>();
            services.AddTransient<ISenderEmail, EmailService>();
            services.AddTransient<ITokenService, TokenService>();

        }
    }

}
