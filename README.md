# How to Setup and Use Farmers Hub

## Github Link
[Farmers Hub](https://github.com/JacquesLife/Semester5FarmersHubPOE)


# Prerequisites for Farmers Hub

1. **.NET 8.0 SDK**  
   [Download .NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

2. **ASP.NET Core Runtime 8.0**  
   Included with the .NET 8 SDK.

3. **Entity Framework Core 9.0.4**  
   Installed via NuGet.

4. **SQLite**  
   Included via `Microsoft.EntityFrameworkCore.Sqlite` NuGet package.

5. **SQL Server**  
   Required only if using `Microsoft.EntityFrameworkCore.SqlServer`.

6. **EF Core CLI Tools**  
   Install globally and add required packages:
   ```bash
   dotnet tool install --global dotnet-aspnet-codegenerator
   dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
   dotnet add package Microsoft.EntityFrameworkCore.Design
   ```

7. **ASP.NET Core Identity UI Packages**  
   Installed via NuGet:
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.UI
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   ```

8. **IDE**  
   - Visual Studio 2022 or later  
   - *or* Visual Studio Code with the C# extension (recommended)

---

# Project Vision
For FarmersHub, I wanted to experiment with a secure approach to development using ASP.NET Core Identity. While the app is not entirely secure for ease of use of a prototype it does follow strong security practices instead of attempting bootleg security. I wanted to continue with code generation and scaffolding to keep best practices in mind and speed up development. This provided me with a structure form FarmersHub which I could build upon. I understand that these security practices are perfect and can be an inconvenience for users I hope the ReadMe is clear enough to help you get started.

## Issues I ran into
CLI tools are great but didn't handle my foreign keys correctly. Whether this was ignorance on my part or a bug in the tools I am not sure. I had to add numerous error handling for some of the basic CRUD operations to rectify this. The error had something to do with how foreign keys were being passed with the signed in user still not entirely sure but I fixed it.

## Frontend
I used the Bootswatch Minty theme for the UI. I wanted to keep it simple and clean with a green theme to match the FarmersHub theme. I also used icons and components from Google fonts and Awesome Icons. I also use CSS tricks to learn to make specific components like cards etc is a great site. Photos are gotten from pixelbay a website that provides free images and videos.

## Areas I wanted to improve because of time 
- UI for Identity pages 
- Better Input validation
- More pages for CRUD operations
- Remove Lorem Ipsum text 
- Better functionality for admin and bug fixing for this role
- More frequent commits to GitHub would have made my life easier
- Some clickable are just for show and don't do anything or route to nothing
- Get JavaScript into it site.js file and not in the cshtml file


# Important Project Structure

1. Seeded Admin Creates Employees (crucial and good security practice) 
2. Employee creates Farmers and view all products
3. Farmer creates products and view their products


## ASP.NET Core Identity Setup  
_Courtesy of [Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0)_

### Step 1: Create the Database Folder and ApplicationDbContext

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Programming3A.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your DbSet<T> here, if needed
        // public DbSet<FarmerModel> Farmers { get; set; }
    }
}
```

### Step 2: Register DbContext and Identity Services in `Program.cs`

Add the following **before** `var app = builder.Build();`:

```csharp
using Microsoft.EntityFrameworkCore;
using Programming3A.Database;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Needed for Identity UI
```

### Step 3: Configure Middleware in `Program.cs`

Add this **after** `var app = builder.Build();`:

```csharp
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Required for Identity scaffolded pages
```

### Step 4: Configure `appsettings.json` Connection String

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=app.db"
}
```

---

## Create and Apply Migrations for Identity

Once everything is set up, run the following commands in the terminal:

```bash
dotnet ef migrations add CreateIdentitySchema
dotnet ef database update
```

---

## How CLI Tools Work

### Scaffold Product Controller
If you have the perquisites installed, you can scaffold your controller after creating your getters and setter in your model. 
CLI is a powerful tool for base CRUD operations.

```bash
dotnet aspnet-codegenerator controller -name ProductController \
  -m Programming3A.Models.ProductModel \
  -dc Programming3A.Database.ApplicationDbContext \
  --relativeFolderPath Controllers \
  --useDefaultLayout \
  --referenceScriptLibraries
```

### Scaffold Farmer Controller

```bash
dotnet aspnet-codegenerator controller -name FarmerController \
  -m Programming3A.Models.FarmerModel \
  -dc Programming3A.Database.ApplicationDbContext \
  --relativeFolderPath Controllers \
  --useDefaultLayout \
  --referenceScriptLibraries
```


# How to Use Farmers Hub

1. Register a User in the Registration page

2. Logout

2. Login as Admin (check the seeded data in Seed folder) and assign the user a role of Employee.
   - Username: admin@farmhub.com
   - Password: Admin123!

3. Now that you are Employee you can create Farmers.

4. Create a Farmer
   - Username: "FarmersFistName"@farmerhub 
   - Password: Farmer123!

5. Login as Farmer

6. You can now create products and view your own products.

## Emergency Data if Struggling (assuming my database come populated I hope it does)

### Farmer
Username: mack@farmerhub
Password: Farmer123!

### Employee
Username: employee@mail.com
Password: Emp12345!

### Admin
Username: admin@farmhub.com
Password: Admin123!

# References

- [Microsoft Docs - ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0)
  
- [Cli Tools for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-9.0)

- [Bootswatch Themes](https://bootswatch.com/minty/)

- [Pixelbay](https://pixabay.com/)
  
- [CSS Tricks](https://css-tricks.com/)

- [How to Setup Roles Video](https://www.youtube.com/watch?app=desktop&v=Y6DCP-yH-9Q)

- [Complex Models](https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-a-more-complex-data-model-for-an-asp-net-mvc-application)

- [Setting up SQLite Video](https://www.youtube.com/watch?v=S7SdtcIr28s)

- [Setting up File Services](https://www.youtube.com/watch?v=hcoKLORWbjY)

- [Claude for Frontend Help](https://claude.ai/new)
