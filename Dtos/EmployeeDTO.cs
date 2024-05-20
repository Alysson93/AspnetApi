namespace AspnetApi.Dtos;

public record EmployeeRequest(
    string Email,
    string Password,
    string Code,
    string Name
);

public record EmployeeResponse(
    string Email,
    string Name
);