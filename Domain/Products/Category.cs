using Flunt.Validations;

namespace AspnetApi.Domain.Products;

public class Category : Entity
{
    public Category(string name, string createdBy)
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(name, "Name", "Nome da categoria é um atributo obrigatório.")
            .IsGreaterOrEqualsThan(name, 3, "Name", "Categoria deve ter no mínimo 3 caracteres.")
            .IsNotNullOrEmpty(createdBy, "CreatedBy", "Nome do criador é um atributo obrigatório.")
            .IsNotNullOrEmpty(createdBy, "EditedBy", "Nome do editor é um atributo obrigatório.");
        AddNotifications(contract);
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        EditedBy = createdBy;
        CreatedAt = DateTime.Now;
        EditedAt = DateTime.Now;
    }
}
