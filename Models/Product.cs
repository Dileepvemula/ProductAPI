using System.Xml.Serialization;

namespace ProductAPI.Models
{
    /// <summary>
    /// Represents a product entity.
    /// </summary>
    [XmlRoot("Product")]
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="brand">The brand of the product.</param>
        public Product(string name, string brand)
        {
            Name = name;
            Brand = brand;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="brand">The brand of the product.</param>
        /// <param name="price">The price of the product.</param>
        public Product(int id, string name, string brand, decimal price)
        {
            Id = id;
            Name = name;
            Brand = brand;
            Price = price;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product() { }

        /// <summary>
        /// Gets or sets the ID of the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the brand of the product.
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }
    }
}