using Microsoft.AspNetCore.Mvc;
using DataLayer;
using System.Threading.Tasks;
using System.Linq;
using DataLayer.Entities;
using AuctionMvc.Models;
using Microsoft.EntityFrameworkCore;
using AuctionMvc.Helpers;
using AuctionMvc.Paginator;
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
    private readonly UnitOfWork _unitOfWork;
    private readonly AuctionService _auctionService;

    public AuctionController(UnitOfWork unitOfWork, AuctionService auctionService)
    {
      _unitOfWork = unitOfWork;
      _auctionService = auctionService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
      if (User.Identity.IsAuthenticated)
        return Redirect("~/Auction/Home");

      return Redirect("~/Auth/Login");
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
      IQueryable<Product> products = _unitOfWork.Products.GetAll();

      if (!String.IsNullOrEmpty(searchString))
        products = products.Where(p => p.Category.Name.Contains(searchString) || p.Name.Contains(searchString));

      int pageSize = 10;

      IQueryable<ProductViewModel> productVMs = products.AsNoTracking().Select(p => new ProductViewModel
      {
        Id = p.Id,
        CategoryName = p.Category.Name,
        Name = p.Name,
        Description = p.Description,
        Price = PriceHelper.IntToDecimal(p.Price),
        ImageUrl = FileHelper.GetLinkImage(p.ImageFileName),
        LogoUrl = FileHelper.GetLinkImage(p.Category.ImageFileName),
        Bidder = p.Bidder
      });

      return View(await PaginatedList<ProductViewModel>.CreateAsync(productVMs, pageNumber ?? 1, pageSize));
    }

    /// <summary>
    /// Product purchase
    /// </summary>
    /// <param name="id">Product Id</param>
    /// <returns>Selected product view</returns>
    [HttpGet]
    public async Task<IActionResult> Detail(long id)
    {
      Product product = await _unitOfWork.Products.GetAll().SingleOrDefaultAsync(p => p.Id == id);

      return View(new ProductViewModel
      {
        Id = product.Id,
        CategoryName = product.Category.Name,
        Name = product.Name,
        Description = product.Description,
        Price = PriceHelper.IntToDecimal(product.Price),
        SellerPrice = PriceHelper.IntToDecimal(product.SellerPrice),
        ImageUrl = FileHelper.GetLinkImage(product.ImageFileName),
        Bidder = product.Bidder
      });
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
