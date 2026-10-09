
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UYCommerce.Data;
using UYCommerce.Paypal;
using UYCommerce.Services;
using UyCommerce.Infrastructure;
using UyCommerce.Infrastructure.Services.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//email config
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<UyCommerce.Infrastructure.Services.Email.EmailConfiguration>();

builder.Services.AddSingleton(emailConfig!);
builder.Services.AddSingleton<UyCommerce.Infrastructure.Services.Email.IEmailService, UyCommerce.Infrastructure.Services.Email.EmailService>();
builder.Services.AddSingleton<IFileService, FileService>();

//paypal settings
builder.Services.AddSingleton(x =>
new PaypalAPI(
    builder.Configuration.GetSection("paypal:settings")["clientId"],
    builder.Configuration.GetSection("paypal:settings")["secretKey"],
    builder.Configuration.GetSection("paypal:settings")["sandboxUrl"]

    ));

//runtime compilation
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

//var connectionString = builder.Configuration.GetConnectionString("Default");

//builder.Services.AddDbContext<ShopContext>(options => options.UseMySql(
//    connectionString, ServerVersion.AutoDetect(connectionString))
//    );

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Login";
        options.LoginPath = "/Login";
        options.Cookie.IsEssential = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
                      policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("User", policy =>
                      policy.RequireClaim(ClaimTypes.Role, "User"));
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions!.Value);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var ServiceProvider = scope.ServiceProvider;
    UyCommerce.Infrastructure.Persistance.DbInitializer.Initialize(ServiceProvider);
}

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();

