using DataLayer.Abstractions;
using DataLayer.DbContexts;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
  /// <summary>
  /// Categories repository
  /// </summary>
  public class CategoryRepository : IRepository<Category>
  {
    private readonly AuctionDbContext _db;

    public CategoryRepository(AuctionDbContext db)
    {
      _db = db;
    }

    public IQueryable<Category> GetAll()
    {
      return _db.Categories;
    }

    public async Task<Category> GetAsync(long id)
    {
      return await _db.Categories.FindAsync(id);
    }

    public async Task CreateAsync(Category category)
    {
      await _db.Categories.AddAsync(category);
    }

    public void Update(Category category)
    {
      _db.Entry(category).State = EntityState.Modified;
    }

    public bool Delete(long id)
    {
      var category = _db.Categories.Find(id);

      if (category == null)
        return false;

      _db.Categories.Remove(category);
      return true;
    }
  }
}
