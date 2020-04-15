using DataLayer.DbContexts;
using DataLayer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace DataLayer
{
  /// <summary>
  /// UnitOfWork pattern for auction database
  /// </summary>
  public class UnitOfWork : IDisposable
  {
    private readonly AuctionDbContext _db;

    public UnitOfWork(AuctionDbContext db)
    {
      _db = db;
    }

    private CategoryRepository _categoryRepository;
    public CategoryRepository Categories => _categoryRepository ?? (_categoryRepository = new CategoryRepository(_db));


    private ProductRepository _productRepository;
    public ProductRepository Products => _productRepository ?? (_productRepository = new ProductRepository(_db));

    private UserRepository _userRepository;
    public UserRepository Users => _userRepository ?? (_userRepository = new UserRepository(_db));

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
