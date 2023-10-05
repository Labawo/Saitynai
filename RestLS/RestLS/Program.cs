using RestLS.Data;
using RestLS.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ORM - Microsoft.EntityFrameworkCore.SqlServer
// Microsoft.EntityFrameworkCore.Tools
// Orm managment - dotnet tool install --global dotnet-ef

builder.Services.AddControllers();

builder.Services.AddDbContext<LS_DbContext>();
//adding repost
builder.Services.AddTransient<IDoctorsRepository, DoctorsRepository>();
builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();
builder.Services.AddTransient<IRecomendationsRepository, RecomendationsRepository>();
builder.Services.AddTransient<IAdminsRepository, AdminsRepository>();
builder.Services.AddTransient<IPatientsRepository, PatientsRepository>();
builder.Services.AddTransient<IGroupSessionsRepository, GroupSessionsRepository>();
//builder.Services.AddTransient<IUsersRepository, UsersRepository>();

//migration after changing entities  
// dotnet ef migrations add 'comment'
// dotnet ef database update

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();