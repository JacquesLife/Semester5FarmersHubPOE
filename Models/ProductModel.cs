using System;
using System.ComponentModel.DataAnnotations;

namespace Programming3A.Models;

public class ProductModel
{
    [Key]
    public int ProductId { get; set; }
    public int FarmerId { get; set; }
    public string ProductName { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime ProductionDate { get; set; }
    public FarmerModel Farmer { get; set; }

}
