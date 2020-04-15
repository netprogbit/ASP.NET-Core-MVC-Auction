using DataLayer.Abstractions;
using DataLayer.DbContexts;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
  /// <summary>
  /// Products repository
  /// </summary>
  public class ProductRepository : IRepository<Product>
  {
    private readonly AuctionDbContext _db;

    public ProductRepository(AuctionDbContext db)
    {
      _db = db;
    }

    public IQueryable<Product> GetAll()
    {
      return _db.Products.Include(p => p.Category);
    }

    public async Task<Product> GetAsync(long id)
    {
      return await _db.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Product product)
    {
      await _db.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
      _db.Entry(product).State = EntityState.Modified;
    }

    public bool Delete(long id)
    {
      var product = _db.Products.Find(id);

      if (product == null)
        return false;

      _db.Products.Remove(product);
      return true;
    }
  }
}
