using System;
using System.Linq;
using System.Threading.Tasks;
using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Paginator;
using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionMvc.Controllers
{
  /// <summary>
  /// Category admin panel
  /// </summary>
  [Authorize(Roles = "admin")]
  public class CategoryController : Controller
  {
    private readonly UnitOfWork _unitOfWork;

    public CategoryController(UnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Call category list view
    /// </summary>
    /// <param name="currentFilter">For pagination</param>
    /// <param name="searchString">Search term</param>
    /// <param name="pageNumber">For pagination</param>
    /// <returns>Category list</returns>
    [HttpGet]
    public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
    {
      if (searchString == null)
        searchString = currentFilter;
      else
        pageNumber = 1;

      ViewData["CurrentFilter"] = searchString;
      IQueryable<Category> categories = _unitOfWork.Categories.GetAll();

      if (!String.IsNullOrEmpty(searchString))
        categories = categories.Where(c => c.Name.Contains(searchString));

      int pageSize = 5;

      IQueryable<CategoryViewModel> categoryVMs = categories.AsNoTracking().Select(c => new CategoryViewModel
      {
        Id = c.Id,
        Name = c.Name
      });

      return View(await PaginatedList<CategoryViewModel>.CreateAsync(categoryVMs, pageNumber ?? 1, pageSize));
    }

    /// <summary>
    /// Call category add view
    /// </summary>
    [HttpGet]
    public IActionResult Add()
    {
      return View(new CategoryViewModel { ImageUrl = FileHelper.GetLinkNoImage() });
    }

    /// <summary>
    /// Add category
    /// </summary>
    /// <param name="categoryVM">Category form model</param>        
    [HttpPost]
    public async Task<IActionResult> Add(CategoryViewModel categoryVM)
    {
      if (!ModelState.IsValid)
      {
        return View(categoryVM); // Validation failed. Return to add form
      }

      string newFileName = FileHelper.GetUniqueFileName(categoryVM.ImageFile?.FileName);
      await AddCategoryAsync(categoryVM, newFileName);
      await FileHelper.AddImageAsync(categoryVM.ImageFile, newFileName);
      return Redirect("~/Category/Index");
    }

    /// <summary>
    /// Call category edit view
    /// </summary>
    /// <param name="id">Category Id</param>
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
      Category category = await _unitOfWork.Categories.GetAll().AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);

      return View(new CategoryViewModel
      {
        Id = category.Id,
        Name = category.Name,
        ImageUrl = FileHelper.GetLinkImage(category.ImageFileName)
      });
    }

    /// <summary>
    /// Update category
    /// </summary>
    /// <param name="categoryVM">Category form model</param>        
    [HttpPost]
    public async Task<IActionResult> Edit(CategoryViewModel categoryVM)
    {
      if (!ModelState.IsValid)
      {
        return View(categoryVM); // Validation failed. Return to edit form
      }

      string newFileName = FileHelper.GetUniqueFileName(categoryVM.ImageFile?.FileName);
      string oldFileName = await UpdateCategoryAsync(categoryVM, newFileName);
      await FileHelper.UpdateImageAsync(categoryVM.ImageFile, oldFileName, newFileName);
      return Redirect("~/Category/Index");
    }

    private async Task AddCategoryAsync(CategoryViewModel categoryVM, string newFileName)
    {
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          Category category = new Category();
          category.Name = categoryVM.Name;
          category.ImageFileName = FileHelper.FilterImageName(newFileName);
          await _unitOfWork.Categories.CreateAsync(category);
          await _unitOfWork.SaveAsync();
          dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
          dbContextTransaction.Rollback(); // Rollbacking DB      
          throw new ApplicationException("DB Transaction Failed. " + e.Message);
        }
      }
    }

    private async Task<string> UpdateCategoryAsync(CategoryViewModel categoryVM, string newFileName)
    {
      string oldFileName = null;

      // Update category from DB
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          Category category = await _unitOfWork.Categories.GetAll().AsNoTracking().SingleOrDefaultAsync(c => c.Id == categoryVM.Id);
          oldFileName = category.ImageFileName;
          category.Id = categoryVM.Id;
          category.Name = categoryVM.Name;

          if (!string.IsNullOrEmpty(newFileName))
            category.ImageFileName = newFileName;

          _unitOfWork.Categories.Update(category);
          await _unitOfWork.SaveAsync();
          dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
          dbContextTransaction.Rollback(); // Rollbacking DB       
          throw new ApplicationException("DB Transaction Failed. " + e.Message);
        }
      }

      return oldFileName;
    }

  }
}
