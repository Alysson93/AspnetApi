namespace AspnetApi.Dtos;

public record EmployeeRequest(
    string Email,
    string Password,
    string Name
);
