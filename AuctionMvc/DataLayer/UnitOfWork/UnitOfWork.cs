using DataLayer.DbContexts;
using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace DataLayer.UnitOfWork
{
    /// <summary>
    /// UnitOfWork pattern for auction database
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AuctionDbContext _db;

        public UnitOfWork(AuctionDbContext db)
        {
            _db = db;
        }

        private IRepository<Category> _categoryRepository;
        public IRepository<Category> Categories => _categoryRepository ?? (_categoryRepository = new CategoryRepository(_db));


        private IRepository<Product> _productRepository;
        public IRepository<Product> Products => _productRepository ?? (_productRepository = new ProductRepository(_db));

        private IRepository<User> _userRepository;
        public IRepository<User> Users => _userRepository ?? (_userRepository = new UserRepository(_db));

        public IDbContextTransaction BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _db.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
