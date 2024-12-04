using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TarikhMaghribi.Utils.Extentions
{
    public static class CustomJwtAuthExtention
    {
        public static void AddCustomJwtAuth(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
                };
            });
        }
        public static void AddSwaggerGenJwtAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "test api",
                    Description = "TarikhMghribi",
                    Contact = new OpenApiContact()
                    {
                        Name = "By : Oulhaj Omar && El Hassan Hamdaoui Alaoui",
                        Email = "omaroulhaj19@gmail.com ",
                        Url = new Uri("http://3000/")
                    }
                });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter the JWT Key"
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement() {
             {
                 new OpenApiSecurityScheme()
                 {
                     Reference = new OpenApiReference()
                     {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                     },
                     Name = "Bearer",
                     In = ParameterLocation.Header
                 },
                 new List<string>()
             }
         });
            });
        }
    }
}
