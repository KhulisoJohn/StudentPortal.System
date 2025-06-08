using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models; // ✅ Adjust if ApplicationUser is in a different namespace
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


// 🔐 Load environment variables from .env in project root
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// ✅ Read connection details from .env file
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD");

// ✅ Check if values exist
if (string.IsNullOrEmpty(dbServer) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
{
    throw new Exception("One or more required database environment variables are missing.");
}

// ✅ Build connection string
var connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

// 🧠 Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🧩 Identity with Entity Framework
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentPortalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// 🌐 Configure the HTTP request pipeline
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

// 🏠 Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
