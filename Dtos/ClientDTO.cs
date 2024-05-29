namespace AspnetApi.Dtos;

public record ClientRequest(
    string Email,
    string Password,
    string Cpf,
    string Name
);
