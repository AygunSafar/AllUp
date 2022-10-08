using System.ComponentModel.DataAnnotations.Schema;

namespace AllUp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
        public bool IsDeactive { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public ProductDetail ProductDetail { get; set; }
        [NotMapped]

        public IFormFile[] Photos { get; set; }
    }
}
