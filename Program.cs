using Microsoft.EntityFrameworkCore;
using StudentPortal.Data; // ✅ Change if needed
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Load .env file first (MUST be before using env vars)
Env.Load();

// ✅ Build DB connection string from env
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

// 🔧 Inject the connection string into EF Core
builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🧩 MVC controllers
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🛡️ Error handling & HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 🔁 Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
