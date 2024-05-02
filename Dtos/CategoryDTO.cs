namespace AspnetApi.Dtos;

public record CategoryRequest(
    string Name,
    bool Active = true
);

public record CategoryResponse(
    string Name,
    bool Active
);