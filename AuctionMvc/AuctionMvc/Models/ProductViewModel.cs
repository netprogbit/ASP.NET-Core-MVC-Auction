using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionMvc.Models
{
  public class ProductViewModel
  {
    public long Id { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public string CategoryName { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Range(0.01, 20000000.00, ErrorMessage = "Price must be between 0.01 and 20000000.00")]
    public decimal Price { get; set; }

    public decimal SellerPrice { get; set; }

    [DisplayName("Image")]
    public IFormFile ImageFile { get; set; }

    public string ImageUrl { get; set; }

    public string LogoUrl { get; set; }

    public long Bidder { get; set; }
  }
}
