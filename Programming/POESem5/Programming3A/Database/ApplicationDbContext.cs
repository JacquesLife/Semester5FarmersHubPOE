/// <summary>
/// Short but crucial file responsible for setting up the database context.
/// It inherits from IdentityDbContext to manage user authentication and authorization.
/// It also contains DbSet properties for the FarmerModel and ProductModel. 
/// It links with Identity framework to manage user roles and authentication.
/// </summary>

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Programming3A.Models;

namespace Programming3A.Database 
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSet for the FarmerModel and ProductModel
        public DbSet<FarmerModel> Farmers { get; set; }
        public DbSet<ProductModel> Products { get; set; }
    }
}

//---------------------------------End of File-----------------------------------------------------