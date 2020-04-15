using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuctionMvc.Helpers
{
  public static class JwtHelper
  {
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
      return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.StaticConfig["AppSettings:Secret"]));
    }
  }
}
