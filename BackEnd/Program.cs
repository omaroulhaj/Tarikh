using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.Extentions;
using static TarikhMaghribi.Extentions.Mail;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TarikhDB")));
builder.Services.AddTransient<ISenderEmail, EmailService>();
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
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


var app = builder.Build();
app.UseCors("AllowAngularLocalhost");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();