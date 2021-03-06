using AuctionMvc.Helpers;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AuctionMvc.Hubs
{
  /// <summary>
  /// SignalR auction hub
  /// </summary>
  public class AuctionHub : Hub
  {
    /// <summary>
    /// Send price all sabscribers
    /// </summary>
    public async Task SendAsync(long productId, int price)
    {
      await Clients.All.SendAsync(productId.ToString(), PriceHelper.IntToDecimal(price));
    }
  }
}
