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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<CategoryViewModel>> GetCategoriesAsync(string search, int? pageNumber)
        {
            if (search == null)
                search = string.Empty;

            IEnumerable<Category> categories = await _unitOfWork.Categories.FindAllAsync(search);

            IEnumerable<CategoryViewModel> categoryVMs = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return PaginatedList<CategoryViewModel>.Create(categoryVMs, pageNumber ?? 1, PaginatorHelper.DefaultPageSize);
        }

        public async Task AddAsync(CategoryViewModel categoryVM)
        {
            string newFileName = FileHelper.GetUniqueFileName(categoryVM.ImageFile?.FileName);
            await AddCategoryAsync(categoryVM, newFileName);
            await FileHelper.AddImageAsync(categoryVM.ImageFile, newFileName);
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
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB      
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        public async Task EditAsync(CategoryViewModel categoryVM)
        {
            string newFileName = FileHelper.GetUniqueFileName(categoryVM.ImageFile?.FileName);
            string oldFileName = await UpdateCategoryAsync(categoryVM, newFileName);
            await FileHelper.UpdateImageAsync(categoryVM.ImageFile, oldFileName, newFileName);
        }

        private async Task<string> UpdateCategoryAsync(CategoryViewModel categoryVM, string newFileName)
        {
            string oldFileName = null;

            // Update category from DB
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    Category category = await _unitOfWork.Categories.FindAsync(categoryVM.Id);
                    oldFileName = category.ImageFileName;
                    category.Id = categoryVM.Id;
                    category.Name = categoryVM.Name;

                    if (!string.IsNullOrEmpty(newFileName))
                        category.ImageFileName = newFileName;

                    _unitOfWork.Categories.Update(category);
                    await _unitOfWork.SaveAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB       
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            return oldFileName;
        }

        public async Task<CategoryViewModel> CreateViewModelAsync(long id)
        {
            Category category = await _unitOfWork.Categories.FindAsync(id);

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = FileHelper.GetLinkImage(category.ImageFileName)
            };
        }
    }
}
