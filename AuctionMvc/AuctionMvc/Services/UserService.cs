using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Paginator;
using DataLayer.Entities;
using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<UserViewModel>> GetCategoriesAsync(string search, int? pageNumber)
        {
            if (search == null)
                search = string.Empty;

            IEnumerable<User> categories = await _unitOfWork.Users.FindAllAsync(search);

            IEnumerable<UserViewModel> categoryVMs = categories.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            }).ToList();

            return PaginatedList<UserViewModel>.Create(categoryVMs, pageNumber ?? 1, PaginatorHelper.DefaultPageSize);
        }

        public async Task EditAsync(UserViewModel userVM)
        {
            // Update user from DB
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    User user = await _unitOfWork.Users.FindAsync(userVM.Id);
                    user.Id = userVM.Id;
                    user.FirstName = userVM.FirstName;
                    user.LastName = userVM.LastName;
                    user.Email = userVM.Email;
                    user.Role = userVM.Role;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB       
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        public async Task DeleteAsync(long id)
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
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB       
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        public async Task<UserViewModel> CreateViewModelAsync(long id)
        {
            User user = await _unitOfWork.Users.FindAsync(id);

            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
