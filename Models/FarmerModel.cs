using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Programming3A.Models;

public class FarmerModel
{
    [Key]
    public int FarmerId { get; set; }
    
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public IdentityUser User { get; set; }
    public List<ProductModel> Products { get; set; }
   
}
