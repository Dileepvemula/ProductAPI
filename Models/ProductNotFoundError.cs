using System;

/// <summary>
/// Represents an error response for a product not found.
/// </summary>
public class ProductNotFoundError
{
    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The ID of the product that was not found.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductNotFoundError"/> class with the specified message and product ID.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="id">The ID of the product.</param>
    public ProductNotFoundError(string message, int id)
    {
        Message = message ?? "No Error message provided";
        ProductId = id;
        StatusCode = 404;
    }
}