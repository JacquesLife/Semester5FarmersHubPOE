/// <summary>
/// The FarmerModel class represents a farmer in the system.
/// It contains properties for the farmer's ID, user ID, full name, and contact number.
/// It also includes data annotations for validation and database mapping.
/// The class is linked to the IdentityUser class for user management.
/// The FarmerModel class is used to store and manage farmer information in the database.
/// It is linked to the Identity framework for user management.
/// The class includes data annotations for validation and database mapping.
/// <summary>
/// <reference = https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-a-more-complex-data-model-for-an-asp-net-mvc-application


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Programming3A.Models;

public class FarmerModel
{
    [Key]
    // Ensure that the FarmerId is auto-incremented in the database
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    // This is the primary key for the FarmerModel
    public int FarmerId { get; set; }

    // This is the foreign key that links the FarmerModel to the IdentityUser
    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    // This is the full name of the farmer
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
    public string FullName { get; set; }

    // This is the contact number of the farmer
    [Required(ErrorMessage = "Contact number is required.")]
    [Display(Name = "Contact Number")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20, ErrorMessage = "Contact number cannot be longer than 20 characters.")]
    public string ContactNumber { get; set; }

    // Make the navigation property optional
    public virtual IdentityUser? User { get; set; }
    
    //public List<ProductModel> Products { get; set; }
   
}


//--------------------------------------------------End of File-----------------------------------------------------