using System.ComponentModel.DataAnnotations;

namespace AuctionMvc.Models
{
  public class LoginViewModel
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not correct")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}
