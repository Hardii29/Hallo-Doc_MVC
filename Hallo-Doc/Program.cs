using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using System.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Hallo_Doc.Repository.Repository.Interface;
using Hallo_Doc.Repository.Repository.Implementation;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));
builder.Services.AddScoped<IAdminDashboard, AdminDashboardRepo>();
builder.Services.AddScoped<ILogin, LoginRepo>();
builder.Services.AddScoped<IPatientUser, PatientUserRepo>();
builder.Services.AddScoped<IPatientReq, PatientReqRepo>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAdminLogin, AdminLoginRepo>();
builder.Services.AddScoped<IProvider, ProviderRepo>();
builder.Services.AddScoped<IAdminNavbar, AdminNavbarRepo>();
builder.Services.AddScoped<IEmail_SMS, Email_SMSservices>();
builder.Services.AddScoped<IPhysician, PhysicianRepo>();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseNotyf();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
