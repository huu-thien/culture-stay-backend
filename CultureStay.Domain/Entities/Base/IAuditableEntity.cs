namespace CultureStay.Domain.Entities.Base;

public interface IAuditableEntity : IEntityBase
{
    int? CreatedBy { get; set; }  
    DateTime? CreatedOn { get; set; }
    int? UpdatedBy { get; set; }
    DateTime? UpdatedOn {get;set;}
    DateTime CreatedAt { get; set; }
    DateTime? LastModifiedAt { get; set; }
}