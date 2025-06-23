/// <summary>
/// This is the Program.cs file for the ASP.NET Core MVC application.
/// It sets up the application, configures services, and runs the application.
/// It uses Entity Framework Core with SQLite for the database.
/// It also sets up ASP.NET Core Identity for user authentication and authorization.
/// The application is designed to be a prototype for a farm management system.
/// <summary>
/// <All remarks>
// /// - [Microsoft Docs - ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0)
// - [Cli Tools for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-9.0)
// - [Bootswatch Themes](https://bootswatch.com/minty/)
// - [Pixelbay](https://pixabay.com/)
// - [CSS Tricks](https://css-tricks.com/)
// - [How to Setup Roles Video](https://www.youtube.com/watch?app=desktop&v=Y6DCP-yH-9Q)
// - [Complex Models](vhttps://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-a-more-complex-data-model-for-an-asp-net-mvc-application)
// - [Setting up SQLite Video](https://www.youtube.com/watch?v=S7SdtcIr28s)
// - [Setting up File Services](https://www.youtube.com/watch?v=hcoKLORWbjY)
// /// <remark>

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Programming3A.Database; 
using Programming3A.Seed; 
using Programming3A.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register FileService
builder.Services.AddScoped<IFileService, FileService>();

// Register ApplicationDbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity with default UI and EF Core
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication(); //  Must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages for Identity UI
app.MapRazorPages(); 

// Seed roles and admin user on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedRolesAndAdminAsync(services);
}

app.Run();


// ------------------------------------------End of File-----------------------------------------------------