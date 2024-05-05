using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class PropertyImage: EntityBase
{
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
}