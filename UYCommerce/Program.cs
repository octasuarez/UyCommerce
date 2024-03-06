
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Paypal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

builder.Services.AddDbContext<ShopContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
    var services = scope.ServiceProvider;

    DbInitializer.Initialize(services);
}


app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();

