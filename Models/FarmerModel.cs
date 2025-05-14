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
    public int FarmerId { get; set; }

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Contact number is required.")]
    [Display(Name = "Contact Number")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20, ErrorMessage = "Contact number cannot be longer than 20 characters.")]
    public string ContactNumber { get; set; }

    // Make the navigation property optional
    public virtual IdentityUser? User { get; set; }
    
    //public List<ProductModel> Products { get; set; }
   
}
