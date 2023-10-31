using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestLS.Auth;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ORM - Microsoft.EntityFrameworkCore.SqlServer
// Microsoft.EntityFrameworkCore.Tools
// Orm managment - dotnet tool install --global dotnet-ef
// Microsoft.AspNetCore.Identity
// Microsoft.AspNetCore.Identity.EntityFrameworkCore
// Microsoft.AspNetCore.Authentication.JwtBearer


builder.Services.AddControllers();

builder.Services.AddIdentity<ClinicUser, IdentityRole>()
    .AddEntityFrameworkStores<LS_DbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
    });
    
builder.Services.AddDbContext<LS_DbContext>();
//adding repost
builder.Services.AddTransient<ITherapiesRepository, TherapiesRepository>();
builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();
builder.Services.AddTransient<IRecomendationsRepository, RecomendationsRepository>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

var dbSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();

//migration after changing entities  
// dotnet ef migrations add 'comment'
// dotnet ef database update