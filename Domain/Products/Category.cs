using Flunt.Validations;

namespace AspnetApi.Domain.Products;

public class Category : Entity
{
    public Category(string name, string createdBy)
    {
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        EditedBy = createdBy;
        CreatedAt = DateTime.Now;
        EditedAt = DateTime.Now;
        Validate();
    }

    public void Edit(string name, bool active, string editedBy) {
        Name = name;
        Active = active;
        EditedBy = editedBy;
        EditedAt = DateTime.Now;
        Validate();
    }   

    private void Validate() {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(Name, "Name", "Nome da categoria é um atributo obrigatório.")
            .IsGreaterOrEqualsThan(Name, 3, "Name", "Categoria deve ter no mínimo 3 caracteres.")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy", "Nome do criador é um atributo obrigatório.")
            .IsNotNullOrEmpty(EditedBy, "EditedBy", "Nome do editor é um atributo obrigatório.");
        AddNotifications(contract);
    }
}
