using MediatR;

namespace CultureStay.Domain.Entities.Base;

public abstract class EntityBase : IAuditableEntity
{
	public int Id { get; set; }
	
    private readonly List<INotification> _domainEvents = new();
    
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(INotification domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
    public bool IsDeleted { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}