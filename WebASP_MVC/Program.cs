
#if jwt
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
#endif
#if cookies || role
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebASP_MVC.Models;
#endif
#if Identity
using Entities.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
#endif

var builder = WebApplication.CreateBuilder(args);
#if Identity
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FoodDeliveryDB;Trusted_Connection=True;"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>
    (
    //options =>
    //{
    //    options.SignIn.RequireConfirmedAccount = false;
    //    //Other options go here
    //}
    ).AddEntityFrameworkStores<DataContext>();
#elif cookies
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = userstoredb; Trusted_Connection = True;"));

// ��������� ������������ �����������
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
#elif jwt
// JWT
//.............
//builder.Services.AddAuthentication("Bearer")  // ���������� �������� ��������������
//    .AddJwtBearer();      // ����������� �������������� � ������� jwt-�������
string s = builder.Configuration["TokenKey"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ���������, ����� �� �������������� �������� ��� ��������� ������
        ValidateIssuer = true,
        // ������, �������������� ��������
        ValidIssuer = AuthOptions.ISSUER,
        // ����� �� �������������� ����������� ������
        ValidateAudience = true,
        // ��������� ����������� ������
        ValidAudience = AuthOptions.AUDIENCE,
        // ����� �� �������������� ����� �������������
        ValidateLifetime = true,
        // ��������� ����� ������������
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        // ��������� ����� ������������
        ValidateIssuerSigningKey = true,
    };
});
//.......................
//��������� ������������ �����������
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
#if cookies || role || jwt
builder.Services.AddAuthorization();
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

#if jwt
//Jwt.........
app.UseDefaultFiles();
//...........
#endif
app.UseStaticFiles();

app.UseRouting();

#if cookies || role || jwt || Identity
app.UseAuthentication();
app.UseAuthorization();
#endif

// Jwt
// Token creation....................................................
//app.Map("/login/{username}", (string username) =>
//{
//    return new JwtService(builder.Configuration).CreateToken(new Entities.Domain.ApplicationUser() { UserName = username });
//    //var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
//    //// ������� JWT-�����
//    //var jwt = new JwtSecurityToken(
//    //        issuer: AuthOptions.ISSUER,
//    //        audience: AuthOptions.AUDIENCE,
//    //        claims: claims,
//    //        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
//    //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

//    //return new JwtSecurityTokenHandler().WriteToken(jwt);
//});

///.....................................
// Jwt.............
#if jwt
// �������� �� � ��������������
var people = new List<Person>
 {
    new Person("tom@gmail.com", "12345"),
    new Person("bob@gmail.com", "55555")
};
app.MapPost("/login", (Person loginData) =>
{
    // ������� ������������ 
    //ApplicationUser? user = //ApiClient.users.FirstOrDefault(p => p.Email == loginData.Email);
    //// ���� ������������ �� ������, ���������� ��������� ��� 401
    //if (user is null) return Results.Unauthorized();
    // ������� ������������ 
    Person? person = people.FirstOrDefault(p => p.Email == loginData.Email && p.Password == loginData.Password);
    // ���� ������������ �� ������, ���������� ��������� ��� 401
    if (person is null) return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
    // ������� JWT-�����
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    // ��������� �����
    var response = new
    {
        access_token = encodedJwt,
        username = person.Email
    };
    return Results.Json(new
    {
        access_token =// new JwtService().CreateToken(person),
        encodedJwt,
        username = person.Email
    });
});
app.Map("/data", [Authorize] () => new { message = "Hello World!" });
app.Map("/hello", [Authorize] () => "Hello World!");
app.Map("/", () => "Home Page");
#endif
//...................
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FoodDelivery}/{action=Index}/{id?}/{ingrId?}");
app.Run();
//Jwt..........
#if jwt
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // �������� ������
    public const string AUDIENCE = "MyAuthClient"; // ����������� ������
    const string KEY = "mysupersecret_secretkey!123";   // ���� ��� ��������

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}

internal record class Person(string Email, string Password);
#endif
//...............