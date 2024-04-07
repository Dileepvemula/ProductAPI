using ProductAPI.Models;

namespace ProductAPI.Repositories
{
    /// <summary>
    /// Represents a repository for managing products.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>The list of products.</returns>
        List<Product> GetProducts();
        
        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID, or null if not found.</returns>
        Product? GetProductById(int id);
        
        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <returns>The added product.</returns>
        Product AddProduct(Product product);
        
        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="product">The product to update.</param>
        void UpdateProduct(Product product);
        
        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        void DeleteProduct(int id);
        
        /// <summary>
        /// Checks if a product with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        bool ProductExists(int id);
        
        /// <summary>
        /// Checks if a product with the specified name and brand exists.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="brand">The brand of the product.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        bool ProductExists(string name, string brand);
    }
}