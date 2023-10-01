using RestLS.Data;
using RestLS.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ORM - Microsoft.EntityFrameworkCore.SqlServer
// Microsoft.EntityFrameworkCore.Tools
// Orm managment - dotnet tool install --global dotnet-ef

builder.Services.AddControllers();

builder.Services.AddDbContext<LS_DbContext>();
builder.Services.AddTransient<IDoctorsRepository, DoctorsRepository>();
builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();