using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_DATABASE");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrEmpty(dbServer) ||
    string.IsNullOrEmpty(dbPort) ||
    string.IsNullOrEmpty(dbName) ||
    string.IsNullOrEmpty(dbUser) ||
    string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("One or more required environment variables are missing.");
}

var connectionString = $"Server={dbServer};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";

// Add DB context
builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentPortalDbContext>()
    .AddDefaultTokenProviders();

// Identity Cookie Settings (AccessDeniedPath FIX)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";            
    options.AccessDeniedPath = "/Account/AccessDenied";   
});

// MVC Support
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
