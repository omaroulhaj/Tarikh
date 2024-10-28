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
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomJwtAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
