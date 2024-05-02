using Flunt.Notifications;

namespace AspnetApi.Domain;

public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public bool Active { get; set; }

    public Entity()
    {
        Id = new Guid();
    }
}