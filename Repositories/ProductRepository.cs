using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductAPI.Repositories
{
    /// <inheritdoc />
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new List<Product>();
        private int _nextId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class with initial products.
        /// </summary>
        public ProductRepository()
        {
            // Adding initial products to the repository
            _products.Add(new Product { Id = _nextId++, Name = "Product 1", Brand = "Brand A", Price = 100 });
            _products.Add(new Product { Id = _nextId++, Name = "Product 2", Brand = "Brand B", Price = 150 });
            _products.Add(new Product { Id = _nextId++, Name = "Product 3", Brand = "Brand C", Price = 200 });
        }

        /// <inheritdoc />
        public List<Product> GetProducts()
        {
            // Returns the list of all products in the repository
            return _products;
        }

        /// <inheritdoc />
        public Product? GetProductById(int id)
        {
            // Returns the product with the specified ID or null if not found
            return _products.FirstOrDefault(p => p.Id == id);
        }

        /// <inheritdoc />
        public Product AddProduct(Product product)
        {
            // Adds a new product to the repository if it doesn't already exist
            if (ProductExists(product.Name, product.Brand))
            {
                throw new InvalidOperationException("Product with the same Name and Brand already exists.");
            }

            product.Id = _nextId++; // Assigns a new ID to the product
            _products.Add(product); // Adds the product to the repository
            return product; // Returns the added product
        }

        /// <inheritdoc />
        public void UpdateProduct(Product product)
        {
            // Updates an existing product's details in the repository
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Brand = product.Brand;
                existingProduct.Price = product.Price;
            }
            else
            {
                throw new InvalidOperationException("Product to update does not exist");
            }
        }

        /// <inheritdoc />
        public void DeleteProduct(int id)
        {
            // Removes a product from the repository based on its ID
            var productToRemove = _products.FirstOrDefault(p => p.Id == id);
            if (productToRemove != null)
            {
                _products.Remove(productToRemove); // Removes the product from the repository
            }
            else
            {
                throw new InvalidOperationException("Product to delete does not exist");
            }
        }

        /// <inheritdoc />
        public bool ProductExists(int id)
        {
            // Checks if a product with the specified ID exists in the repository
            return _products.Any(p => p.Id == id);
        }

        /// <inheritdoc />
        public bool ProductExists(string name, string brand)
        {
            // Checks if a product with the specified name and brand exists in the repository
            return _products.Any(p => p.Name == name && p.Brand == brand);
        }
    }
}
