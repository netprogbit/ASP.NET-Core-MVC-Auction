using System.Threading.Tasks;
using AuctionMvc.Models;
using AuctionMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionMvc.Controllers
{
    /// <summary>
    /// User admin panet
    /// </summary>
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Call user list view
        /// </summary>
        /// <param name="currentFilter">For pagination</param>
        /// <param name="searchString">Search term</param>
        /// <param name="pageNumber">Form pagination</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            if (searchString == null)
                searchString = currentFilter;
            else
                pageNumber = 1;

            ViewData["CurrentFilter"] = searchString;
            return View(await _userService.GetCategoriesAsync(searchString, pageNumber));
        }

        /// <summary>
        /// Call user edit view
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            UserViewModel userViewModel = await _userService.CreateViewModelAsync(id);
            return View(userViewModel);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userVM">User form model</param>
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM); // Validation failed. Return to edit form
            }

            await _userService.EditAsync(userVM);
            return LocalRedirect("~/User/Index");
        }

        /// <summary>
        /// Call user delete view
        /// </summary>
        /// <param name="id">User Id</param>
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            UserViewModel userViewModel = await _userService.CreateViewModelAsync(id);
            return View(userViewModel);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userVM">User form model</param>
        [HttpPost]
        public async Task<IActionResult> Delete(UserViewModel userVM)
        {
            await _userService.DeleteAsync(userVM.Id);
            return LocalRedirect("~/User/Index");
        }                
    }
}
