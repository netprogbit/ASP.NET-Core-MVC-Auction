using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuctionMvc.Controllers
{
    /// <summary>
    /// Authentication features
    /// </summary>
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="userVM">Registration form model</param>        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel userVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", MessageHelper.ModelValidationFailed);
                return View();
            }

            bool isRegistered = await _authService.RegisterAsync(userVM);

            if (!isRegistered)
            {
                TempData["Message"] = MessageHelper.UserExists;
                return LocalRedirect("~/Auth/Register");
            }

            return LocalRedirect("~/Auth/Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Signing in
        /// </summary>
        /// <param name="loginVM">Login form model</param>        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", MessageHelper.ModelValidationFailed);
                return View();
            }

            AccessToken accessToken = await _authService.Authenticate(loginVM.Email, loginVM.Password);

            if (accessToken == null)
            {
                TempData["Message"] = MessageHelper.UserNotFound;
                return View();
            }

            //Save access data in session store
            HttpContext.Session.SetString("userId", accessToken.UserId.ToString());
            HttpContext.Session.SetString("token", accessToken.Token);

            return LocalRedirect("~/Auction/Home");
        }

        /// <summary>
        /// Signing out
        /// </summary>
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return LocalRedirect("~/Auth/Login");
        }

    }
}
