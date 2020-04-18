using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DbContexts
{
  /// <summary>
  /// Auction database context
  /// </summary>
  public class AuctionDbContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
        : base(options)
    {
      Database.EnsureCreated();
    }

    /// <summary>
    /// Default database initialization
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(); // Email must be unique      

      modelBuilder.Entity<User>().HasData(
  new User[]
  {
                    new User { Id=1, FirstName="Yuri", LastName="Yuriev", Email="admin@auction.com", Password="CQF9pVh87cIuoNg0xksMsOEJrcqD86hy/H9P8fSjl8mk5ymCjBE2ZOrm1l0C6DlV5xhVeX7I9ecQ8upjo7/Dcg==", Role="admin"},
                    new User { Id=2, FirstName="Ivan", LastName="Ivanov", Email="ivan@gmail.com", Password="WgkOqhMuNVfIVBZxP++JdOWBXZkVWNbztiLBuV2ICeZxV1aDC3Rl3DHaTaDqzKdaqy0LQio+kJdy6xxDFlNR3Q==", Role="user"}
  });

      modelBuilder.Entity<Category>().HasData(
        new Category[]
        {
                    new Category { Id=1, Name="Monitors", ImageFileName="7a9ae808-5cba-4aca-a548-9e5497dcf520.png" },
                    new Category { Id=2, Name="Smartphones", ImageFileName="d9b53dda-f99b-4569-a294-3807faefec49.png" },
                    new Category { Id=3, Name="Tablets", ImageFileName="f2647ff0-3530-4e7a-a009-a2957e19db6d.png" }
        });

      modelBuilder.Entity<Product>().HasData(
          new Product[]
          {
                new Product { Id=1, CategoryId=1, Name="Monitor", Description="M: 1201", Price=500000, SellerPrice=500000, ImageFileName="7b739908-b892-4961-80cd-46a80d45b1e9.png"},
                new Product { Id=2, CategoryId=2, Name="Smartphone", Description="S: 1301", Price=300000, SellerPrice=300000, ImageFileName="0cbc67c4-37e1-4046-9068-5fd6b5eb8410.png"},
                new Product { Id=3, CategoryId=3, Name="Tablet", Description="T: 1401", Price=400000, SellerPrice=400000, ImageFileName="00c24b2c-3117-4e82-8161-c877e8550b09.png"},
                new Product { Id=4, CategoryId=1, Name="Monitor", Description="M: 1202", Price=500000, SellerPrice=500000, ImageFileName="9d3ace22-2a12-4fe8-8998-a4464aff13f6.png"},
                new Product { Id=5, CategoryId=2, Name="Smartphone", Description="S: 1302", Price=300000, SellerPrice=300000, ImageFileName="2f4e71c1-b99f-41f5-a4e3-04eae3998cf4.png"},
                new Product { Id=6, CategoryId=3, Name="Tablet", Description="T: 1402", Price=400000, SellerPrice=400000, ImageFileName="1a710d16-e145-4eff-9df3-8422e917e582.png"},
                new Product { Id=7, CategoryId=1, Name="Monitor", Description="M: 1203", Price=500000, SellerPrice=500000, ImageFileName="578dd49b-b719-40bb-a562-31f47f98c68b.png"},
                new Product { Id=8, CategoryId=2, Name="Smartphone", Description="S: 1303", Price=300000, SellerPrice=300000, ImageFileName="7b5554f9-8eee-4d7f-95ed-943b31ddfcfd.png"},
                new Product { Id=9, CategoryId=3, Name="Tablet", Description="T: 1403", Price=400000, SellerPrice=400000, ImageFileName="7eb25132-59a7-4ce8-95a4-fb9239200147.png"},
                new Product { Id=10, CategoryId=1, Name="Monitor", Description="M: 1204", Price=500000, SellerPrice=500000, ImageFileName="9049d1b8-2d9a-40f7-b38f-b5d185d4b547.png"},
                new Product { Id=11, CategoryId=2, Name="Smartphone", Description="S: 1304", Price=300000, SellerPrice=300000, ImageFileName="56c9618e-fb42-42d3-bdc3-eeec29f28e72.png"},
                new Product { Id=12, CategoryId=3, Name="Tablet", Description="T: 1404", Price=400000, SellerPrice=400000, ImageFileName="babeb68c-ff0b-4b24-b237-8c57c4773924.png"},
                new Product { Id=13, CategoryId=1, Name="Monitor", Description="M: 1205", Price=500000, SellerPrice=500000, ImageFileName="99145066-ab8a-409f-88a9-6096ce62d6f6.png"},
                new Product { Id=14, CategoryId=2, Name="Smartphone", Description="S: 1305", Price=300000, SellerPrice=300000, ImageFileName="60917223-b1cb-47a0-a167-b229d2bca970.png"},
                new Product { Id=15, CategoryId=3, Name="Tablet", Description="T: 1405", Price=400000, SellerPrice=400000, ImageFileName="d84d23da-f2ca-4271-8a49-b4d0b474d549.png"}
          });
    }
  }
}
