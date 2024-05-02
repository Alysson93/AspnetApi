using Flunt.Validations;

namespace AspnetApi.Domain.Products;

public class Category : Entity
{
    public Category(string name, string createdBy)
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(name, "Name", "Nome é um atributo obrigatório.");
        AddNotifications(contract);
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        EditedBy = createdBy;
    }
}
