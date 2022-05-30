using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebASP_MVC.Models;

var builder = WebApplication.CreateBuilder(args);

// ...........
// Добавлено в новой редакции для аутентификации
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=userstoredb;Trusted_Connection=True;"));
// Установка конфигурации подключения
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => //CookieAuthenticationOptions
    {
        options.LoginPath = new PathString("/Account/Login");
    });

// ...........
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
// ...........
// Добавлено в новой редакции
app.UseAuthentication();
// ...........
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FoodDelivery}/{action=Index}/{id?}/{ingrId?}");

//app.MapControllerRoute(
//    name: "default",
//pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
