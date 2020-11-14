using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Paginator;
using DataLayer.Entities;
using DataLayer.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

/// <summary>
/// Procuct actions service
/// </summary>
namespace AuctionMvc.Services
{
    public class ProductService : IProductService
    {
        protected readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuctionService _auctionService;

        public ProductService(ILogger<ProductService> logger, IUnitOfWork unitOfWork, IAuctionService auctionService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _auctionService = auctionService;
        }

        public async Task<PaginatedList<ProductViewModel>> GetProductsAsync(string search, int? pageNumber)
        {
            if (search == null)
                search = string.Empty;

            IEnumerable<Product> products = await _unitOfWork.Products.FindAllAsync(search);

            IEnumerable<ProductViewModel> productVMs = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                CategoryName = p.Category.Name,
                Name = p.Name,
                Description = p.Description,
                Price = PriceHelper.IntToDecimal(p.Price),
                ImageUrl = FileHelper.GetLinkImage(p.ImageFileName),
                LogoUrl = FileHelper.GetLinkImage(p.Category.ImageFileName),
                Bidder = p.Bidder
            }).ToList();

            return PaginatedList<ProductViewModel>.Create(productVMs, pageNumber ?? 1, PaginatorHelper.DefaultPageSize);
        }

        public async Task AddAsync(ProductViewModel productVM)
        {
            string newFileName = FileHelper.GetUniqueFileName(productVM.ImageFile?.FileName);
            await AddProductAsync(productVM, newFileName);
            await FileHelper.AddImageAsync(productVM.ImageFile, newFileName);
            await _auctionService.StartSaleAsync(productVM.Id, PriceHelper.DecimalToInt(productVM.Price)); // Statring sale            
        }

        private async Task AddProductAsync(ProductViewModel productVM, string newFileName)
        {
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    Product product = new Product();
                    Category category = await _unitOfWork.Categories.FindAsync(c => c.Name == productVM.CategoryName);
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
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB      
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        public async Task EditAsync(ProductViewModel productVM)
        {
            string newFileName = FileHelper.GetUniqueFileName(productVM.ImageFile?.FileName);
            string oldFileName = await UpdateProductAsync(productVM, newFileName);
            await FileHelper.UpdateImageAsync(productVM.ImageFile, oldFileName, newFileName);
            await _auctionService.StartSaleAsync(productVM.Id, PriceHelper.DecimalToInt(productVM.Price)); // Statring sale
        }

        private async Task<string> UpdateProductAsync(ProductViewModel productVM, string newFileName)
        {
            string oldFileName = null;

            // Update product from DB
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    Product product = await _unitOfWork.Products.FindAsync(productVM.Id);
                    oldFileName = product.ImageFileName;
                    product.Id = productVM.Id;
                    Category category = await _unitOfWork.Categories.FindAsync(c => c.Name == productVM.CategoryName);
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
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Rollbacking DB       
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            return oldFileName;
        }

        public async Task DeleteAsync(ProductViewModel productVM)
        {
            Product product = await _unitOfWork.Products.FindAsync(productVM.Id);
            await DeleteProductAsync(productVM.Id);  // Delete product from DB      
            FileHelper.DeleteFile(product.ImageFileName); // Detele unnecessary image file            
            await _auctionService.StopSale(productVM.Id); // Stopping sale
            User userBidder = await _unitOfWork.Users.FindAsync(productVM.Bidder);

            if (userBidder != null)
            {
                _logger.LogError("{0} User ID: {1}. Product info:  {2} | {3} | ${4}, ", MessageHelper.EmailMessageNotSent, product.Bidder, product.Name, product.Description, PriceHelper.IntToDecimal(product.Price));
                EmailHelper.Send(userBidder.Email); // Send info to the buyer about the purchase of the product
            }
        }

        private async Task DeleteProductAsync(long id)
        {
            // Detete product from DB
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Products.Delete(id);
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

        public async Task<ProductViewModel> CreateViewModelAsync(long id)
        {
            Product product = await _unitOfWork.Products.FindAsync(id);

            return new ProductViewModel
            {
                Id = product.Id,
                CategoryName = product.Category.Name,
                Name = product.Name,
                Description = product.Description,
                Price = PriceHelper.IntToDecimal(product.Price),
                SellerPrice = PriceHelper.IntToDecimal(product.SellerPrice),
                ImageUrl = FileHelper.GetLinkImage(product.ImageFileName),
                Bidder = product.Bidder
            };
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoryViewModelsAsync()
        {
            IEnumerable<Category> categories = await _unitOfWork.Categories.FindAllAsync("");
            return categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, ImageUrl = FileHelper.GetLinkImage(c.ImageFileName) });
        }
    }
}
