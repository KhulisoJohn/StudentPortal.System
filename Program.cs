using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentPortalSystem.Data;
using StudentPortalSystem.Models;
using Supabase;
using DotNetEnv;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// Detect if running EF Core design-time commands
var isDesignTime = AppDomain.CurrentDomain.FriendlyName.Contains("ef");

// -----------------------------
// EF Core Database Connection
// -----------------------------
var dbHost = Environment.GetEnvironmentVariable("DB_SERVER") ?? "aws-1-us-east-2.pooler.supabase.com";
var dbPort = isDesignTime ? "5432" : Environment.GetEnvironmentVariable("DB_PORT") ?? "6543"; // direct for migrations, pooled for runtime
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres.utmaatwqelfqdhcwvbgr";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Khuljohn-studi0";

if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("Missing one or more required database environment variables.");
}

var dbConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<StudentPortalDbContext>(options =>
    options.UseNpgsql(dbConnectionString));

// -----------------------------
// Identity
// -----------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StudentPortalDbContext>()
    .AddDefaultTokenProviders();

// -----------------------------
// Supabase Client (runtime only)
// -----------------------------
if (!isDesignTime)
{
    var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
    var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY");

    if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
        throw new InvalidOperationException("Missing SUPABASE_URL or SUPABASE_SERVICE_ROLE_KEY.");

    var supabaseOptions = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions);
    await supabaseClient.InitializeAsync();

    builder.Services.AddSingleton(supabaseClient);
}

// -----------------------------
// MVC / Controllers
// -----------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -----------------------------
// Middleware
// -----------------------------
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

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
