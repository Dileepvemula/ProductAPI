# ProductAPI

ProductAPI is a RESTful API for managing products. It supports CRUD operations for products, including listing, retrieving, creating, updating, and deleting products.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Unit Testing](#unit-testing)
- [Contributing](#contributing)
- [License](#license)

## Introduction

ProductAPI is designed to provide a simple and efficient way to manage products in an application. It offers a range of endpoints to interact with product data, making it suitable for various e-commerce and inventory management systems.

## Features

- List all products with pagination
- Retrieve a product by ID
- Create a new product
- Update an existing product
- Delete a product

## Technologies Used

- C#
- ASP.NET Core
- Moq for mocking
- NUnit for unit testing
- Microsoft.Extensions.Logging for logging

## Getting Started

To get started with ProductAPI, follow these steps:

1. Clone the repository:

```bash
git clone https://github.com/Dileepvemula/ProductAPI.git
```
# ProductAPI

Open the solution in Visual Studio or your preferred IDE.

Build the solution to restore NuGet packages.

Run the application.

Use a tool like Postman or curl to test the API endpoints.

## API Endpoints

- **GET /api/products** - Retrieves a paginated list of products.
    - Parameters: `page` (default: 1), `pageSize` (default: 10)
- **GET /api/products/{id}** - Retrieves a product by its ID.
- **POST /api/products** - Creates a new product.
    - Request Body: `{ "name": "Product Name", "brand": "Product Brand", "price": 100 }`
- **PUT /api/products/{id}** - Updates an existing product by its ID.
    - Request Body: `{ "name": "Updated Product Name", "brand": "Updated Product Brand", "price": 150 }`
- **DELETE /api/products/{id}** - Deletes a product by its ID.

## Unit Testing

ProductAPI includes unit tests for the controller using Moq and NUnit. You can run these tests to ensure the correctness of the API endpoints and their behavior.

To run the unit tests:

1. Open the solution in Visual Studio.
2. Navigate to the `ProductAPI.UnitTests` project.
3. Run the unit tests using the test runner.

## Contributing

Contributions are welcome! If you encounter any issues, have suggestions for improvements, or want to add new features, feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
