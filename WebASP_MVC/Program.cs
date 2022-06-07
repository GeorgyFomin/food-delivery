
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebASP_MVC.Models;

var builder = WebApplication.CreateBuilder(args);

#if cookies
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = userstoredb; Trusted_Connection = True;"));

// установка конфигурации подключения
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => //CookieAuthenticationOptions
    {
        options.LoginPath = new PathString("/Account/Login");
    });

#elif role
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=rolesappdb;Trusted_Connection=True;"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                });
#else
// установка конфигурации подключения
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        // Cookie settings
//        options.Cookie.HttpOnly = true;
//        //options.Cookie.Expiration 
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//        options.LoginPath = "/Account/Login";
//        options.LogoutPath = "/Account/Logout";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.SlidingExpiration = true;
//    });
#endif
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
//var defaultCulture = new CultureInfo("es-UY");
//var localizationOptions = new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture(defaultCulture),
//    SupportedCultures = new List<CultureInfo> { defaultCulture },
//    SupportedUICultures = new List<CultureInfo> { defaultCulture }
//};
//app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//#if cookies || role
app.UseAuthentication();
//#endif
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FoodDelivery}/{action=Index}/{id?}/{ingrId?}");

app.Run();
