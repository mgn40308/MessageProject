using MessageProject.Hubs;
using MessageProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IDbConnection>(provider =>
{
    string connectionString = builder.Configuration.GetConnectionString("MySQL");
    return new MySqlConnection(connectionString);
});



builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

})
.AddEntityFrameworkStores<CusomIdentityDbContext>()
.AddUserManager<CustomUserManager>()
.AddSignInManager<CustomSignInManager>()
.AddRoleManager<CustomRoleManager>()
.AddDefaultTokenProviders();

builder.Services.AddDbContext<CusomIdentityDbContext>(options =>
       options.UseMySql(builder.Configuration.GetConnectionString("MySQL"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQL"))));


builder.Services.AddScoped<CustomUserManager>();
builder.Services.AddScoped<CustomSignInManager>();
builder.Services.AddScoped<CustomRoleManager>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLoggedIn", policy =>
        policy.RequireAuthenticatedUser());

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Member/Login";
});

builder.Services.AddSignalR();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
