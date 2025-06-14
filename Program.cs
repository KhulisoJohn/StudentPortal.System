using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Force TLS 1.2
//System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

 DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));


var serveName = Environment.GetEnvironmentVariable("DB_SERVER");

var passWord = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Use MySQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server={serverName};Port=3306;Database=StudentPortalDb;User=root;Password={passWord};";

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentPortalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Seed roles/admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleSeeder.SeedRolesAndAdminUser(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seeding failed");
    }
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
