using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
  public class Product
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int SellerPrice { get; set; }
    public string ImageFileName { get; set; }
    public long Bidder { get; set; }
  }
}
