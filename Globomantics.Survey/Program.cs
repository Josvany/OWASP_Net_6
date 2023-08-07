using Microsoft.AspNetCore.Identity;
using Globomantics.Survey.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Globomantics.Survey.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GloboIdentityDbConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GlobomanticsSurveyDbContext>(
    dbContextoptions => dbContextoptions.UseSqlite(builder.Configuration["ConnectionStrings:GloboSurveyDbConnectionString"]));

builder.Services.AddDbContext<IdentityDbContext>(
    dbContextoptions => dbContextoptions.UseSqlite(builder.Configuration["ConnectionStrings:GloboIdentityDbConnectionString"]));
//pedir confirmacion de correo en true para confirmacion
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<IdentityDbContext>();

//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<IdentityDbContext>();


builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opt.Cookie.SameSite = SameSiteMode.Strict; // esto indica cuando se ajusta la solicitud de la cookie es decir que solo se podra leer cuando la fuente de la solicitud sea nuestro sitio
    opt.Cookie.Path = "/"; // esto es para que la cookie sea revelevante para todo el directorio
    opt.Cookie.Name = "__Host-Identity";
    opt.Cookie.MaxAge = TimeSpan.FromHours(12);
    opt.ExpireTimeSpan = TimeSpan.FromHours(12);
});


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

// habilitar la cache y session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opt.Cookie.SameSite = SameSiteMode.Strict; // esto indica cuando se ajusta la solicitud de la cookie es decir que solo se podra leer cuando la fuente de la solicitud sea nuestro sitio
    opt.Cookie.Path = "/"; // esto es para que la cookie sea revelevante para todo el directorio
    opt.Cookie.Name = "__Host-Session";
    opt.Cookie.MaxAge = TimeSpan.FromHours(1);
    opt.IdleTimeout = TimeSpan.FromMinutes(20);
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
