using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuctionMvc.Models;
using AuctionMvc.Helpers;
using System;
using AuctionMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AuctionMvc.Controllers
{
    /// <summary>
    /// Auction features
    /// </summary>
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly IProductService _productService;

        public AuctionController(IAuctionService auctionService, IProductService productService)
        {
            _auctionService = auctionService;
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return LocalRedirect("~/Auction/Home");

            return LocalRedirect("~/Auth/Login");
        }

        /// <summary>
        ///  Product list for buying
        /// </summary>
        /// <param name="currentFilter">For paginagion</param>
        /// <param name="searchString">Search term</param>
        /// <param name="pageNumber">For paginagion</param>
        /// <returns>Goods select view</returns>
        [HttpGet]
        public async Task<IActionResult> Home(string currentFilter, string searchString, int? pageNumber)
        {
            if (searchString == null)
                searchString = currentFilter;
            else
                pageNumber = 1;

            ViewData["CurrentFilter"] = searchString;
            return View(await _productService.GetProductsAsync(searchString, pageNumber));            
        }

        /// <summary>
        /// Product purchase
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>Selected product view</returns>
        [HttpGet]
        public async Task<IActionResult> Detail(long id)
        {
            ProductViewModel productViewModel = await _productService.CreateViewModelAsync(id);
            return View(productViewModel);
        }

        /// <summary>
        /// Bid offer
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>Return back to product</returns>
        [HttpGet]
        public async Task<IActionResult> Buy(long id)
        {
            decimal price = await _auctionService.Buy(Convert.ToInt64(HttpContext.Session.GetString("userId")), id);
            TempData["Message"] = MessageHelper.YourBidOffer + String.Format("{0:0.00}", price);
            return RedirectToAction("Detail", new { id = id });
        }

    }
}
