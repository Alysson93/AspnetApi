namespace AspnetApi.Dtos;

public record ProductRequest(
    string Name,
    Guid CategoryId,
    string Description,
    decimal Price,
    bool HasStock = true,
    bool Active = true
);

public record ProductResponse(
    string Name,
    string? CategoryName,
    string Description,
    decimal Price,
    bool HasStock = true,
    bool Active = true
);
