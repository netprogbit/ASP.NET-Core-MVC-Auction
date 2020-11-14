using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public interface IAuctionService
    {
        Task<decimal> Buy(long userId, long productId);
        Task StartSaleAsync(long productId, int price);
        Task StopSale(long productId);
    }
}