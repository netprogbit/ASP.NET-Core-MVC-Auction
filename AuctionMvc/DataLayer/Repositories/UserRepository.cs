using DataLayer.Abstractions;
using DataLayer.DbContexts;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
  /// <summary>
  /// Users repository
  /// </summary>
  public class UserRepository : IRepository<User>
  {
    private readonly AuctionDbContext _db;

    public UserRepository(AuctionDbContext db)
    {
      _db = db;
    }

    public IQueryable<User> GetAll()
    {
      return _db.Users;
    }

    public async Task<User> GetAsync(long id)
    {
      return await _db.Users.FindAsync(id);
    }

    public async Task CreateAsync(User user)
    {
      await _db.Users.AddAsync(user);
    }

    public void Update(User user)
    {
      _db.Entry(user).State = EntityState.Modified;
      _db.Entry(user).Property(x => x.Password).IsModified = false;
    }

    public bool Delete(long id)
    {
      var user = _db.Users.Find(id);

      if (user == null)
        return false;

      _db.Users.Remove(user);
      return true;
    }
  }
}
