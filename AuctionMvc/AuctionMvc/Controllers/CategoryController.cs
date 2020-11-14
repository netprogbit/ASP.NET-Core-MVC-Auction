using System.Threading.Tasks;
using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMvc.Controllers
{
    /// <summary>
    /// Category admin panel
    /// </summary>
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
            return View(await _categoryService.GetCategoriesAsync(searchString, pageNumber));
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

            await _categoryService.AddAsync(categoryVM);
            return LocalRedirect("~/Category/Index");
        }

        /// <summary>
        /// Call category edit view
        /// </summary>
        /// <param name="id">Category Id</param>
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            CategoryViewModel categoryViewModel = await _categoryService.CreateViewModelAsync(id);
            return View(categoryViewModel);
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

            await _categoryService.EditAsync(categoryVM);
            return LocalRedirect("~/Category/Index");
        }                
    }
}
