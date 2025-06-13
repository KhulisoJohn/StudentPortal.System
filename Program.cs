using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrEmpty(dbServer) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
{
    throw new Exception("One or more required database environment variables are missing.");
}

var connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentPortalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAndAdminUser(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
