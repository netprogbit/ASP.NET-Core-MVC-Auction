using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionMvc.Controllers
{
    /// <summary>
    /// Product admin panel
    /// </summary>
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {        
        private readonly ProductService _productService;
        private readonly AuctionService _auctionService;

        public ProductController(ProductService productService, AuctionService auctionService)
        {            
            _productService = productService;
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
            return View(await _productService.GetProductsAsync(searchString, pageNumber));
        }

        /// <summary>
        /// Call product add view
        /// </summary>
        [HttpGet]
        public async  Task<IActionResult> Add()
        {
            await InitCategorySelectorAsync();
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
                await InitCategorySelectorAsync();
                return View(productVM); // Validation failed. Return to add form
            }

            await _productService.AddAsync(productVM);
            return LocalRedirect("~/Product/Index");
        }

        /// <summary>
        /// Call product edit view
        /// </summary>
        /// <param name="id">Product Id</param>    
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            await InitCategorySelectorAsync();
            ProductViewModel productViewModel = await _productService.CreateViewModelAsync(id);
            return View(productViewModel);
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
                await InitCategorySelectorAsync();
                return View(productVM); // Validation failed. Return to edit form
            }

            await _productService.EditAsync(productVM);
            return LocalRedirect("~/Product/Index");
        }

        /// <summary>
        /// Call delete product view
        /// </summary>
        /// <param name="id">Product Id</param>    
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            ProductViewModel productViewModel = await _productService.CreateViewModelAsync(id);
            return View(productViewModel);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="productVM">Product form model</param>
        [HttpPost]
        public async Task<IActionResult> Delete(ProductViewModel productVM)
        {
            await _productService.DeleteAsync(productVM);
            return LocalRedirect("~/Product/Index");
        }                

        private async Task InitCategorySelectorAsync()
        {
            IEnumerable<CategoryViewModel> categoryViewModels = await _productService.GetCategoryViewModelsAsync();
            ViewBag.CategoryList = new SelectList(categoryViewModels, nameof(CategoryViewModel.Name), nameof(CategoryViewModel.Name));
        }

    }
}
