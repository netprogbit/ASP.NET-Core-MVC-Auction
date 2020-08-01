using System;
using System.Linq;
using System.Threading.Tasks;
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
  /// User admin panet
  /// </summary>
  [Authorize(Roles = "admin")]
  public class UserController : Controller
  {
    private readonly UnitOfWork _unitOfWork;

    public UserController(UnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
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
      IQueryable<User> users = _unitOfWork.Users.GetAll();

      if (!String.IsNullOrEmpty(searchString))
        users = users.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString));

      int pageSize = 5;

      IQueryable<UserViewModel> userVMs = users.AsNoTracking().Select(u => new UserViewModel
      {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Role = u.Role
      });

      return View(await PaginatedList<UserViewModel>.CreateAsync(userVMs, pageNumber ?? 1, pageSize));
    }

    /// <summary>
    /// Call user edit view
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
      User user = await _unitOfWork.Users.GetAll().AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);

      return View(new UserViewModel
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role
      });
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

      await UpdateUserAsync(userVM);
      return Redirect("~/User/Index");
    }

    /// <summary>
    /// Call user delete view
    /// </summary>
    /// <param name="id">User Id</param>
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
      User user = await _unitOfWork.Users.GetAll().AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);

      return View(new UserViewModel
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role
      });
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="userVM">User form model</param>
    [HttpPost]
    public async Task<IActionResult> Delete(UserViewModel userVM)
    {
      await DeleteUserAsync(userVM.Id);
      return Redirect("~/User/Index");
    }

    private async Task UpdateUserAsync(UserViewModel userVM)
    {
      // Update user from DB
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          User user = await _unitOfWork.Users.GetAll().AsNoTracking().SingleOrDefaultAsync(u => u.Id == userVM.Id);
          user.Id = userVM.Id;
          user.FirstName = userVM.FirstName;
          user.LastName = userVM.LastName;
          user.Email = userVM.Email;
          user.Role = userVM.Role;
          _unitOfWork.Users.Update(user);
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

    private async Task DeleteUserAsync(long id)
    {
      // Detete user from DB
      using (var dbContextTransaction = _unitOfWork.BeginTransaction())
      {
        try
        {
          _unitOfWork.Users.Delete(id);
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

  }
}
