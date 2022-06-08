using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using MediatR;
using System.Reflection;
using Microsoft.OpenApi.Models;
using WebApi.Data;
using UseCases.API.Core;
using Entities.Domain;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(typeof(UseCases.API.Deliveries.GetDeliveries).Assembly);
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FoodDeliveryDB;Trusted_Connection=True;"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#if Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>
    (
    options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        //Other options go here
    }
    ).AddEntityFrameworkStores<DataContext>();
#endif
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    Seed.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
