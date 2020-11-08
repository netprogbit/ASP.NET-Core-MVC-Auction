using AuctionMvc.Helpers;
using AuctionMvc.Hubs;
using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    /// <summary>
    /// Auction actions service
    /// </summary>
    public class AuctionService
    {
        private readonly double priceIncreaseCoeff = 1.1; // Increase by 10%
        private readonly IHubContext<AuctionHub> _auctionHub;
        private readonly UnitOfWork _unitOfWork;

        public AuctionService(IHubContext<AuctionHub> auctionHub, UnitOfWork unitOfWork)
        {
            _auctionHub = auctionHub;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Start of sale
        /// </summary>
        public async Task StartSaleAsync(long productId, int price)
        {
            await _auctionHub.Clients.All.SendAsync(productId.ToString(), PriceHelper.IntToDecimal(price)); // Potential buyer price notification
        }

        /// <summary>
        /// Bid offer
        /// </summary>
        public async Task<decimal> Buy(long userId, long productId)
        {
            Product product = await _unitOfWork.Products.FindAsync(productId);
            product.Price = product.SellerPrice;
            product.SellerPrice = (int)(product.Price * priceIncreaseCoeff);
            product.Bidder = userId;

            // DB Transaction of updating product
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.SaveAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();  // Rollbacking
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            await _auctionHub.Clients.All.SendAsync(productId.ToString(), PriceHelper.IntToDecimal(product.SellerPrice));
            return PriceHelper.IntToDecimal(product.Price);
        }

        /// <summary>
        /// End of sale
        /// </summary>
        public async Task StopSale(long productId)
        {
            await _auctionHub.Clients.All.SendAsync(productId.ToString(), 0.00M); // Product sale is over        
        }

    }
}
