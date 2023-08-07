using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Globomantics.Survey.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Globomantics.Survey.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GlobomanticsSurveyDbContext>(
    dbContextoptions => dbContextoptions.UseSqlite(builder.Configuration["ConnectionStrings:GloboSurveyDbConnectionString"]));

builder.Services.AddDbContext<IdentityDbContext>(
    dbContextoptions => dbContextoptions.UseSqlite(builder.Configuration["ConnectionStrings:GloboIdentityDbConnectionString"]));
//pedir confirmacion de correo en true para confirmacion
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.ConfigureApplicationCookie(options => 
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict; // esto indica cuando se ajusta la solicitud de la cookie es decir que solo se podra leer cuando la fuente de la solicitud sea nuestro sitio
    options.Cookie.Path = "/"; // esto es para que la cookie sea revelevante para todo el directorio
    options.Cookie.Name = "__Host-Identity";
    options.Cookie.MaxAge = TimeSpan.FromHours(12);
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Path = "/";
    options.Cookie.Name = "__Host-Session";
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("SuperAdmin", 
    policy => policy.RequireClaim("IsManager", "true")
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
