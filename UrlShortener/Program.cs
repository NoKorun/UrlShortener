using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("DBCS")));

builder.Services.AddControllersWithViews();
// Add services for authentication and authorization
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyAuthCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Linkweb}/{action=Dashboard}");

app.MapControllers();

app.Run();
