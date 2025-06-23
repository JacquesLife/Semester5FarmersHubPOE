/// <summary>
/// The ProductModel class represents a product in the system.
/// It contains properties for the product's ID, farmer ID, name, category, description, image path, price, and production date.
/// It also includes data annotations for validation and database mapping.
/// The class is linked to the FarmerModel class for managing products associated with farmers.
/// The ProductModel class is used to store and manage product information in the database
/// and is linked to the FarmerModel class for managing products associated with farmers.
/// The class includes data annotations for validation and database mapping.
/// </summary>
/// <reference = https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-a-more-complex-data-model-for-an-asp-net-mvc-application

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Programming3A.Models
{
    public class ProductModel
    {
        // This is the primary key for the ProductModel
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        // This is the foreign key that links the ProductModel to the FarmerModel
        [ForeignKey("Farmer")]
        public int FarmerId { get; set; }

        // This is the name of the product
        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        // This is the category of the product
        [Required(ErrorMessage = "Product category is required.")]
        [Display(Name = "Category")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

        // This is the description of the product
        [Required(ErrorMessage = "Product description is required.")]
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // Store the image path in the database
        [Display(Name = "Product Image")]
        public string? ImagePath { get; set; }

        // Property for handling file upload (not mapped to database)
        [NotMapped]
        [Display(Name = "Upload Product Image")]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        // This is the price of the product
        [Required(ErrorMessage = "Price is required.")]
        [Display(Name = "Price")]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be between R0.01 and R99,999.99")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // This is the production date of the product
        [Required(ErrorMessage = "Production date is required.")]
        [Display(Name = "Production Date")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        // Navigation property to link to the FarmerModel
        public virtual FarmerModel Farmer { get; set; }
    }
}