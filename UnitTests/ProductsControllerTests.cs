using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ProductAPI.Controllers;
using ProductAPI.Models;
using ProductAPI.Repositories;

namespace ProductAPI.UnitTests
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductRepository> _mockRepository;
        private Mock<ILogger<ProductsController>> _mockLogger;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepository.Object, _mockLogger.Object);
        }

        [Test]
        public void GetAllProducts_ReturnsOkResult_WithValidData()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Brand = "Brand A", Price = 100 },
                new Product { Id = 2, Name = "Product 2", Brand = "Brand B", Price = 150 }
            };
            _mockRepository.Setup(repo => repo.GetProducts()).Returns(products);

            int page = 1; // Assuming default values are used for pagination
            int pageSize = 10; // Assuming default values are used for pagination

            // Act
            var result = _controller.GetAllProducts(page, pageSize);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            dynamic response = okResult.Value; // Cast value to dynamic for property access
            Assert.That(response.PageSize, Is.EqualTo(pageSize));
            Assert.That(response.CurrentPage, Is.EqualTo(page));
            Assert.That(response.TotalPages, Is.EqualTo(1)); // Since we have only 2 products, total pages should be 1 with default page size
            Assert.That(response.TotalItems, Is.EqualTo(2)); // Total items should match the number of products

            Assert.That(response.Products, Is.InstanceOf<List<Product>>());

            var productList = response.Products as List<Product>;
            Assert.That(productList.Count, Is.EqualTo(2));

            var productNames = productList.Select(p => p.Name).ToList();
            CollectionAssert.AreEqual(new List<string> { "Product 1", "Product 2" }, productNames); // Compare product names
        }


        [Test]
        public void GetProduct_ReturnsOkResult_WithValidId()
        {
            // Arrange
            int id = 1;
            var product = new Product { Id = id, Name = "Product 1", Brand = "Brand A", Price = 100 };
            _mockRepository.Setup(repo => repo.GetProductById(id)).Returns(product);
        
            // Act
            var result = _controller.GetProduct(id);
        
            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            
            dynamic response = okResult.Value; // Cast value to dynamic for property access
            Assert.That(response.Product , Is.InstanceOf<Product>());
            
            var model = response.Product as Product;
            Assert.That(id, Is.EqualTo(model.Id));
        }
        
        [Test]
        public void CreateProduct_ReturnsCreatedAtAction_WithValidData()
        {
            // Arrange
            var product = new Product { Name = "New Product", Brand = "Brand X", Price = 200 };
            var createdProduct = new Product { Id = 1, Name = product.Name, Brand = product.Brand, Price = product.Price }; // Assuming the repository assigns an ID

            _mockRepository.Setup(repo => repo.AddProduct(It.IsAny<Product>())).Returns(createdProduct);

            // Act
            var result = _controller.CreateProduct(product.Name, product.Brand, product.Price);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtActionResult = result as CreatedAtActionResult;

            Assert.That(createdAtActionResult.Value, Is.InstanceOf<dynamic>());
            dynamic response = createdAtActionResult.Value;

            Assert.That(response.message, Is.EqualTo("New product created successfully."));
            var responseProduct = response.product as Product;
            Assert.That(responseProduct.Id, Is.EqualTo(createdProduct.Id));
            Assert.That(responseProduct.Brand , Is.EqualTo(createdProduct.Brand));
            Assert.That(responseProduct.Name, Is.EqualTo(createdProduct.Name));
            Assert.That(responseProduct.Price, Is.EqualTo(createdProduct.Price));
        }
        
        [Test]
        public void UpdateProduct_ReturnsOkResult_WithValidIdAndData()
        {
            // Arrange
            int id = 1;
            var existingProduct = new Product { Id = id, Name = "Product 1", Brand = "Brand A", Price = 100 };
            _mockRepository.Setup(repo => repo.GetProductById(id)).Returns(existingProduct);
        
            string updatedName = "Updated Product";
            string updatedBrand = "Brand Y";
            decimal updatedPrice = 150;
        
            // Act
            var result = _controller.UpdateProduct(id, updatedName, updatedBrand, updatedPrice);
        
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            
            var okResult = result as OkObjectResult;
            dynamic response = okResult.Value;

            Assert.That(response.Message, Is.EqualTo("Product with id 1 updated successfully."));
            Assert.That(response.Product, Is.InstanceOf<Product>());

            var product = response.Product as Product;
            Assert.That(updatedName, Is.EqualTo(product.Name));
            Assert.That(updatedBrand, Is.EqualTo(product.Brand));
            Assert.That(updatedPrice, Is.EqualTo(product.Price));
        }
        
        [Test]
        public void DeleteProduct_ReturnsOkResult_WithValidId()
        {
            // Arrange
            int id = 1;
            var existingProduct = new Product { Id = id, Name = "Product 1", Brand = "Brand A", Price = 100 };
            _mockRepository.Setup(repo => repo.GetProductById(id)).Returns(existingProduct);
        
            // Act
            var result = _controller.DeleteProduct(id);
        
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            dynamic response = okResult.Value;

            Assert.That(response.message, Is.EqualTo("Product deleted successfully."));
            Assert.That(response.productId, Is.EqualTo(id));
        }
        
        [Test]
        public void GetProduct_ReturnsBadRequest_WithInvalidId()
        {
            // Arrange
            int invalidId = -1;
        
            // Act
            var result = _controller.GetProduct(invalidId);
        
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid Id provided. Id must be greater than 0."));
        }
        
        [Test]
        public void CreateProduct_ReturnsBadRequest_WithInvalidData()
        {
            // Arrange
            var invalidProduct = new Product { Name = "", Brand = "", Price = -10 };
        
            // Act
            var result = _controller.CreateProduct(invalidProduct.Name, invalidProduct.Brand, invalidProduct.Price);
        
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Name, brand, or price cannot be empty."));
        }
        
        [Test]
        public void UpdateProduct_ReturnsBadRequest_WithInvalidId()
        {
            // Arrange
            int invalidId = -1;
            var updatedName = "Updated Product";
            var updatedBrand = "Brand Y";
            var updatedPrice = 150m;
        
            // Act
            var result = _controller.UpdateProduct(invalidId, updatedName, updatedBrand, updatedPrice);
        
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid Id provided. Id must be greater than 0."));
        }
        
        [Test]
        public void UpdateProduct_ReturnsBadRequest_WithInvalidData()
        {
            // Arrange
            int validId = 1;
            var invalidData = new Product { Name = "", Brand = "", Price = -10 };
        
            // Act
            var result = _controller.UpdateProduct(validId, invalidData.Name, invalidData.Brand, invalidData.Price);
        
            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundObject = result as NotFoundObjectResult;
            Assert.That(notFoundObject.Value, Is.InstanceOf<ProductNotFoundError>());
        }
        
        [Test]
        public void DeleteProduct_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            int invalidId = -1;
        
            // Act
            var result = _controller.DeleteProduct(invalidId);
        
            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestObject = result as BadRequestObjectResult;
            Assert.That(badRequestObject.Value, Is.EqualTo("Invalid Id provided. Id must be greater than 0."));
        }
    }
}
