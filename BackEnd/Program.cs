using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.Models.Entities;
using TarikhMaghribi.Utils.Extentions;

var builder = WebApplication.CreateBuilder(args);
//services
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TarikhDB")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenJwtAuth();
builder.Services.AddCustomJwtAuth(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocalhost",
        builder =>
        {
            builder.WithOrigins("http://13.61.126.240", "http://tarikhmaghribi.com", "https://tarikhmaghribi.com", "http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});



var app = builder.Build();
app.UseCors("AllowAngularLocalhost");


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();




app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
