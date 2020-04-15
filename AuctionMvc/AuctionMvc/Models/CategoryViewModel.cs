using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionMvc.Models
{
  public class CategoryViewModel
  {
    public long Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [DisplayName("Image")]
    public IFormFile ImageFile { get; set; }

    public string ImageUrl { get; set; }
  }
}
