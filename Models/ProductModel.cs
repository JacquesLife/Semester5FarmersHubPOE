using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Programming3A.Models
{
    public class ProductModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [ForeignKey("Farmer")]
        public int FarmerId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product category is required.")]
        [Display(Name = "Category")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

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

        [Required(ErrorMessage = "Price is required.")]
        [Display(Name = "Price")]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be between R0.01 and R99,999.99")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Production date is required.")]
        [Display(Name = "Production Date")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        public virtual FarmerModel Farmer { get; set; }
    }
}