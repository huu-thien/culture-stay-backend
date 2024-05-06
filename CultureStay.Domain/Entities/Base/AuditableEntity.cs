namespace CultureStay.Domain.Entities.Base;

public abstract class AuditableEntity : EntityBase, IAuditableEntity
{
	public int? CreatedBy { get; set; }
	public DateTime? CreatedOn { get; set; }
	public int? UpdatedBy { get; set; }
	public DateTime? UpdatedOn { get; set; }
	
	public DateTime CreatedAt { get; set; }

	public DateTime? LastModifiedAt { get; set; }
	public void ResetEntity()
	{
		CreatedOn = DateTime.UtcNow;
		CreatedBy = null;
		UpdatedOn = null;
		UpdatedBy = null;
		CreatedAt = DateTime.UtcNow;
		LastModifiedAt = null;
	}
}