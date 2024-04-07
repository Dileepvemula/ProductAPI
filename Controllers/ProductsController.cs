using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Models;
using ProductAPI.Repositories;
using Microsoft.Extensions.Logging;

namespace ProductAPI.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductsController> _logger;

        /// <summary>
        /// Initializes a new instance of the ProductsController class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="logger">The logger.</param>
        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves details of all products with pagination.
        /// </summary>
        /// <remarks>
        /// Usage: GET /api/products?page={page}&pageSize={pageSize}
        /// </remarks>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The page size (default is 10).</param>
        /// <returns>Returns a paginated list of products along with pagination details.</returns>
        [HttpGet(Name = "GetAllProducts")]
        public IActionResult GetAllProducts(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalProductsCount = _productRepository.GetProducts().Count;
                var totalPages = (int)Math.Ceiling((double)totalProductsCount / pageSize);

                if (page < 1 || pageSize < 1 || pageSize > 1000)  // Limiting pageSize to avoid large queries
                {
                    _logger.LogWarning("Invalid pagination parameters provided.");
                    return BadRequest("Invalid pagination parameters provided.");
                }

                var products = _productRepository.GetProducts()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new
                {
                    PageSize = pageSize,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalProductsCount,
                    Products = products
                };

                _logger.LogInformation("Retrieved products for page {Page} with page size {PageSize} successfully.", page, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products with pagination.");
                return StatusCode(500, "Internal server error.");
            }
        }


        /// <summary>
        /// Retrieves details of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <remarks>
        /// Usage: GET /api/products/{id}
        /// </remarks>
        /// <returns>Returns the product if exists, else returns an error.</returns>
        [HttpGet("{id}", Name = "GetProductByID")]
        public IActionResult GetProduct([Required(ErrorMessage = "Id is required")] int id)
        {
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning("Invalid Id provided: {Id}. Id must be greater than 0.", id);
                    return BadRequest("Invalid Id provided. Id must be greater than 0.");
                }

                var product = _productRepository.GetProductById(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with Id: {Id} not found.", id);
                    ProductNotFoundError errorResponse = new ProductNotFoundError("Product with this id does not exist.", id);
                    return NotFound(errorResponse);
                }

                var response = new
                {
                    StatusCode = Ok().StatusCode,
                    Product = product
                };
                _logger.LogInformation("Retrieved product with ID: {Id} successfully.", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product by ID.");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="brand">The brand of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <returns>Returns the newly created product if successful, else returns an error.</returns>
        [HttpPost(Name = "CreateProduct")]
        public IActionResult CreateProduct([Required(ErrorMessage = "Name is required")] string name,
            [Required(ErrorMessage = "Brand is required")] string brand,
            [Required(ErrorMessage = "Price is required ")][Range(0, double.MaxValue, ErrorMessage = "Price must be a decimal number")] decimal? price)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(brand) || !price.HasValue)
                {
                    _logger.LogWarning("Name, brand, or price cannot be empty.");
                    return BadRequest("Name, brand, or price cannot be empty.");
                }

                if (price < 0)
                {
                    _logger.LogWarning("Price cannot be a negative number.");
                    return BadRequest("Price cannot be a negative number.");
                }

                if (_productRepository.ProductExists(name, brand))
                {
                    _logger.LogWarning("Product with the same name and brand already exists.");
                    return BadRequest("Product with the same name and brand already exists.");
                }

                Product product = new Product(name, brand);
                product.Price = (decimal)price;

                var createdProduct = _productRepository.AddProduct(product);
                var response = new
                {
                    message = "New product created successfully.",
                    product = createdProduct
                };

                _logger.LogInformation("Created new product with ID: {Id}.", createdProduct.Id);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, response);
            }
            catch (InvalidOperationException ioex)
            {
                _logger.LogError(ioex, "Error occurred while creating new product.");
                return BadRequest(ioex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product.");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing product with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="name">The updated name of the product.</param>
        /// <param name="brand">The updated brand of the product.</param>
        /// <param name="price">The updated price of the product.</param>
        /// <remarks>Usage: PUT /api/products/{id}</remarks>
        /// <returns>Returns the updated product if successful, else BadRequest or NotFound.</returns>
        [HttpPut("{id}", Name = "UpdateProduct")]
        public IActionResult UpdateProduct([Required(ErrorMessage = "Id is required")] int id, string name, string brand, decimal? price)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid Id provided: {Id}. Id must be greater than 0.", id);
                    return BadRequest("Invalid Id provided. Id must be greater than 0.");
                }

                var existingProduct = _productRepository.GetProductById(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with Id: {Id} not found for update.", id);
                    return NotFound(new ProductNotFoundError("Product with this id does not exist to update", id));
                }

                if (price < 0)
                {
                    _logger.LogWarning("Price to update cannot be a negative number.");
                    return BadRequest("Price to update cannot be a negative number.");
                }

                string updatedName = string.IsNullOrEmpty(name) ? existingProduct.Name : name;
                string updatedBrand = string.IsNullOrEmpty(brand) ? existingProduct.Brand : brand;
                decimal updatedPrice = (price.HasValue && price >= 0) ? price.Value : existingProduct.Price;

                if (_productRepository.ProductExists(updatedName, updatedBrand))
                {
                    _logger.LogWarning("Product with updated name and brand already exists.");
                    return BadRequest("Product with this updated name and brand already exists.");
                }

                existingProduct.Name = updatedName;
                existingProduct.Brand = updatedBrand;
                existingProduct.Price = updatedPrice;

                _productRepository.UpdateProduct(existingProduct);
                var response = new
                {
                    Message = $"Product with id {id} updated successfully.",
                    Product = existingProduct
                };
                _logger.LogInformation("Updated product with ID: {Id} successfully.", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product.");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a product with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <remarks>Usage: DELETE /api/products/{id}</remarks>
        /// <returns>Returns NoContent if product is successfully deleted, else BadRequest or NotFound.</returns>
        [HttpDelete("{id}", Name = "DeleteProductById")]
        public IActionResult DeleteProduct([Required(ErrorMessage = "Id is required")] int id)
        {
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning("Invalid Id provided: {Id}. Id must be greater than 0.", id);
                    return BadRequest("Invalid Id provided. Id must be greater than 0.");
                }

                var existingProduct = _productRepository.GetProductById(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with Id: {Id} not found for deletion.", id);
                    return NotFound(new ProductNotFoundError("Product with this id does not exist.", id));
                }

                _productRepository.DeleteProduct(id);
                var response = new
                {
                    message = "Product deleted successfully.",
                    productId = id
                };

                _logger.LogInformation("Deleted product with ID: {Id} successfully.", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product.");
                return BadRequest(ex.Message);
            }
        }
    }
}
