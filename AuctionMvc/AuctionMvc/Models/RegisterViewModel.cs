using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionMvc.Models
{
  public class RegisterViewModel
  {
    [Required(ErrorMessage = "First name is required")]
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not correct")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Passwords not match")]
    [DisplayName("Password Confirmation")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }
  }
}
