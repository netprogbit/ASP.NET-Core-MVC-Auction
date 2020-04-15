using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionMvc.Models
{
  public class UserViewModel
  {
    public long Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not correct")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; }
  }
}
