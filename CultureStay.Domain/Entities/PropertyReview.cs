using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class PropertyReview : EntityBase
{
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public int Cleanliness { get; set; }
    public int Communication { get; set; }
    public int CheckIn { get; set; }
    public int Accuracy { get; set; }
    public int Location { get; set; }
    public int Value { get; set; }
    public string Content { get; set; } = string.Empty;
}