using AuctionMvc.Models;

namespace AuctionMvc.Helpers
{
  public static class MessageHelper
  {
    public static readonly string ErrorWillBeFixed = "The error has been logged and it will be fixed.";
    public static readonly string LogInPlease = "Log in, please.";
    public static readonly string NotPermissions = "You do not have enough permissions.";
    public static readonly string ModelValidationFailed = "Fields Validation Failed.";
    public static readonly string AplicationException = "Error. Aplication Exception: ";
    public static readonly string HttpErrorStatus = "Error. Http Status Code: ";
    public static readonly string UserExists = "User exists with this Email.";
    public static readonly string UserNotFound = "User not found.";
    public static readonly string YourBidOffer = "Your bid offer: $";
    public static readonly string EmailMessageNotSent = "Email message not sent.";
  }
}
