using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Paginator;
using AuctionMvc.Services;
using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionMvc.Controllers
{
  /// <summary>
  /// Product admin panel
  /// </summary>
  [Authorize(Roles = "admin")]
  public class ProductController : Controller
  {
    protected readonly ILogger<ProductController> _logger;
    private readonly UnitOfWork _unitOfWork;
    private readonly AuctionService _auctionService;

    public ProductController(ILogger<ProductController> logger, UnitOfWork unitOfWork, AuctionService auctionService)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _auctionService = auctionService;
    }

    /// <summary>
    /// Call product list view
    /// </summary>
    /// <param name="currentFilter">For pagination</param>
    /// <param name="searchString">Search term</param>
    /// <param name="pageNumber">For pagination</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
    {
      if (searchString == null)
        searchString = currentFilter;
      else
        pageNumber = 1;

      ViewData["CurrentFilter"] = searchString;
      IQueryable<Product> products = _unitOfWork.Products.GetAll();

      if (!String.IsNullOrEmpty(searchString))
        products = products.Where(p => p.Category.Name.Contains(searchString) || p.Name.Contains(searchString));

      int pageSize = 5;

      IQueryable<ProductViewModel> productVMs = products.AsNoTracking().Select(p => new ProductViewModel
      {
        Id = p.Id,
        CategoryName = p.Category.Name,
        Name = p.Name,
        Description = p.Description,
        Price = PriceHelper.IntToDecimal(p.Price),
        Bidder = p.Bidder
      });

      return View(await PaginatedList<ProductViewModel>.CreateAsync(productVMs, pageNumber ?? 1, pageSize));
    }

    // Call product add view
    [HttpGet]
    public IActionResult Add()
    {
      InitCategorySelector();
      return View(new ProductViewModel { ImageUrl = FileHelper.GetLinkNoImage() });
    }

    /// <summary>
    /// Add product
    /// </summary>
    /// <param name="productVM">Product form model</param>
    [HttpPost]
    public async Task<IActionResult> Add(ProductViewModel productVM)
    {
      if (!ModelState.IsValid)
      {
        InitCategorySelector();
        return View(productVM); // Validation failed. Return to add form
      }

      string newFileName = FileHelper.GetUniqueFileName(productVM.ImageFile?.FileName);
      await AddProductAsync(productVM, newFileName);
      await FileHelper.AddImageAsync(productVM.ImageFile, newFileName);
      await _auctionService.StartSaleAsync(productVM.Id, PriceHelper.DecimalToInt(productVM.Price)); // Statring sale
      return Redirect("~/Product/Index");
    }

    /// <summary>
    /// Call product edit view
    /// </summary>
    /// <param name="id">Product Id</param>    
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
      Product product = await _unitOfWork.Products.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
      InitCategorySelector();

      return View(new ProductViewModel
      {
        Id = product.Id,
        CategoryName = product.Category.Name,
        Name = product.Name,
        Description = product.Description,
        Price = PriceHelper.IntToDecimal(product.Price),
        ImageUrl = FileHelper.GetLinkImage(product.ImageFileName),
        Bidder = product.Bidder
      });
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="productVM">Product form model</param>    
    [HttpPost]
    public async Task<IActionResult> Edit(ProductViewModel productVM)
    {
      if (!ModelState.IsValid)
      {
        InitCategorySelector();
        return View(productVM); // Validation failed. Return to edit form
      }

      string newFileName = FileHelper.GetUniqueFileName(productVM.ImageFile?.FileName);
      string oldFileName = await UpdateProductAsync(productVM, newFileName);
      await FileHelper.UpdateImageAsync(productVM.ImageFile, oldFileName, newFileName);
      await _auctionService.StartSaleAsync(productVM.Id, PriceHelper.DecimalToInt(productVM.Price)); // Statring sale
      return Redirect("~/Product/Index");
    }

    /// <summary>
    /// Call delete product view
    /// </summary>
    /// <param name="id">Product Id</param>    
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
      Product product = await _unitOfWork.Products.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);

      return View(new ProductViewModel
      {
        Id = product.Id,
        CategoryName = product.Category.Name,
        Name = product.Name,
        Description = product.Description,
        Price = PriceHelper.IntToDecimal(product.Price),
        ImageUrl = FileHelper.GetLinkImage(product.ImageFileName),
        Bidder = product.Bidder
      });
    }

    /// <summary>
    /// Delete product
    /// </summary>
    /// <param name="productVM">Product form model</param>
    [HttpPost]
    public async Task<IActionResult> Delete(ProductViewModel productVM)
    {
      string imageName = await DeleteProductAsync(productVM.Id);  // Delete product from DB
      FileHelper.DeleteFile(imageName); // Detele unnecessary image file            
      await _auctionService.StopSale(productVM.Id); // Stopping sale
      User userBidder = await _unitOfWork.Users.GetAsync(productVM.Bidder);

      if (userBidder != null)
      {
        _logger.LogError("{0} User ID: {1}. Product info:  {2} | {3} | ${4}, ", MessageHelper.EmailMessageNotSent, productVM.Bidder, productVM.Name, productVM.Description, productVM.Price);
        EmailHelper.Send(userBidder.Email); // Send info to the buyer about the purchase of the product
      }
      
      return Redirect("~/Product/Index");
    }

    private async Task AddProductAsync(ProductViewModel productVM, string newFileName)
    {
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          Product product = new Product();
          Category category = await _unitOfWork.Categories.GetAll().AsNoTracking().SingleOrDefaultAsync(c => c.Name == productVM.CategoryName);
          product.CategoryId = category.Id;
          product.Name = productVM.Name;
          product.Description = productVM.Description;
          product.Price = PriceHelper.DecimalToInt(productVM.Price);
          product.SellerPrice = product.Price;
          product.ImageFileName = FileHelper.FilterImageName(newFileName);
          product.Bidder = 0;
          await _unitOfWork.Products.CreateAsync(product);
          await _unitOfWork.SaveAsync();
          dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
          dbContextTransaction.Rollback(); // Rollbacking DB      
          throw new Exception("DB Transaction Failed. " + e.Message);
        }
      }
    }

    private async Task<string> UpdateProductAsync(ProductViewModel productVM, string newFileName)
    {
      string oldFileName = null;

      // Update product from DB
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          Product product = await _unitOfWork.Products.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == productVM.Id);
          oldFileName = product.ImageFileName;
          product.Id = productVM.Id;
          Category category = await _unitOfWork.Categories.GetAll().AsNoTracking().SingleOrDefaultAsync(c => c.Name == productVM.CategoryName);
          product.CategoryId = category.Id;
          product.Name = productVM.Name;
          product.Description = productVM.Description;
          product.Price = PriceHelper.DecimalToInt(productVM.Price);
          product.SellerPrice = product.Price;

          if (!string.IsNullOrEmpty(newFileName))
            product.ImageFileName = newFileName;

          product.Bidder = 0;
          _unitOfWork.Products.Update(product);

          await _unitOfWork.SaveAsync();
          dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
          dbContextTransaction.Rollback(); // Rollbacking DB       
          throw new Exception("DB Transaction Failed. " + e.Message);
        }
      }

      return oldFileName;
    }

    private async Task<string> DeleteProductAsync(long id)
    {
      string imageName = null;

      // Detete product from DB
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          Product product = await _unitOfWork.Products.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
          imageName = product.ImageFileName;
          _unitOfWork.Products.Delete(id);
          await _unitOfWork.SaveAsync();
          dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
          dbContextTransaction.Rollback(); // Rollbacking DB       
          throw new Exception("DB Transaction Failed. " + e.Message);
        }
      }

      return imageName;
    }

    private void InitCategorySelector()
    {
      IQueryable<Category> categories = _unitOfWork.Categories.GetAll();
      IEnumerable<CategoryViewModel> categoryViewModels = categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, ImageUrl = FileHelper.GetLinkImage(c.ImageFileName) });
      ViewBag.CategoryList = new SelectList(categoryViewModels, nameof(CategoryViewModel.Name), nameof(CategoryViewModel.Name));
    }

  }
}
