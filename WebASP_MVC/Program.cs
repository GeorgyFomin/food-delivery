
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
// JWT
////.............
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        // указывает, будет ли валидироваться издатель при валидации токена
//        ValidateIssuer = true,
//        // строка, представляющая издателя
//        ValidIssuer = AuthOptions.ISSUER,
//        // будет ли валидироваться потребитель токена
//        ValidateAudience = true,
//        // установка потребителя токена
//        ValidAudience = AuthOptions.AUDIENCE,
//        // будет ли валидироваться время существования
//        ValidateLifetime = true,
//        // установка ключа безопасности
//        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
//        // валидация ключа безопасности
//        ValidateIssuerSigningKey = true,
//    };
//});
////.......................
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
//app.UseAuthentication();
//#endif
app.UseAuthorization();
// Jwt
///....................................................
//app.Map("/login/{username}", (string username) =>
//{
//    return new JwtService().CreateToken(new Entities.Domain.ApplicationUser() { UserName = username });
//    //var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
//    //// создаем JWT-токен
//    //var jwt = new JwtSecurityToken(
//    //        issuer: AuthOptions.ISSUER,
//    //        audience: AuthOptions.AUDIENCE,
//    //        claims: claims,
//    //        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
//    //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

//    //return new JwtSecurityTokenHandler().WriteToken(jwt);
//});

//app.Map("/data", [Authorize] () => new { message = "Hello World!" });
///.....................................

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FoodDelivery}/{action=Index}/{id?}/{ingrId?}");

app.Run();
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}