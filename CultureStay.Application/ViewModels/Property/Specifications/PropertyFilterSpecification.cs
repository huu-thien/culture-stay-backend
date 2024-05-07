using CultureStay.Application.ViewModels.Property.Enums;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Property.Specifications;

public class PropertyFilterSpecification : Specification<CultureStay.Domain.Entities.Property>
{
     public PropertyFilterSpecification(PropertyQueryParameters pqp, string role, int hostId = 0 )
    {
        AddInclude(p => p.PropertyImages);
        AddInclude(p => p.PropertyReviews);
        AddInclude(p => p.Host.User);
        AddInclude(p => p.Wishlists);

        if (role == "Admin")
        {
            if (pqp.Status is not null)
                AddFilter(p => p.Status == pqp.Status);
        }
        else
        {
            AddFilter(p => p.Status == PropertyStatus.Approved);
        }

        if (hostId > 0)
            AddFilter(p => p.HostId == hostId);
        
        // Search by name, description, city
        if (!string.IsNullOrWhiteSpace(pqp.Search))
        {
            AddSearchTerm(pqp.Search);
            AddSearchField(nameof(CultureStay.Domain.Entities.Property.Title));
            AddSearchField(nameof(CultureStay.Domain.Entities.Property.Description));
            AddSearchField(nameof(CultureStay.Domain.Entities.Property.City));
        }
        
        if (pqp.Type != null && pqp.Type.Any())
            AddFilter(p => pqp.Type.Contains(p.Type));
        
        // FilterByPriceRange(pqp);

        FilterByRoomAndBedCount(pqp);

        if (!string.IsNullOrWhiteSpace(pqp.City))
            AddFilter(p => p.City == pqp.City);
        
        FilterByDateRange(pqp);

        FilterByGuestCount(pqp);
        FilterByStatus(pqp);

        // Order by
        if (pqp.OrderBy is not null)
        {
            AddOrderByField(pqp.OrderBy.ToString());
            var orderBy = pqp.OrderBy switch
            {
                PropertySortBy.Rating => $"{nameof(CultureStay.Domain.Entities.Property.PropertyReviews)}.Average(" +
                                         $"({nameof(PropertyReview.Cleanliness)} + " +
                                         $"{nameof(PropertyReview.Accuracy)} + " +
                                         $"{nameof(PropertyReview.Communication)} + " +
                                         $"{nameof(PropertyReview.CheckIn)} + " +
                                         $"{nameof(PropertyReview.Value)} + " +
                                         $"{nameof(PropertyReview.Location)}) / 6.0)",
                PropertySortBy.NumberOfReviews => $"{nameof(CultureStay.Domain.Entities.Property.PropertyReviews)}.Count()",
                PropertySortBy.Title => pqp.OrderBy.ToString(),
                PropertySortBy.Description => pqp.OrderBy.ToString(),
                _ => nameof(PropertySortBy.Id)
            };
            AddOrderByField(orderBy);
            if (pqp.IsDescending)
                ApplyDescending();
        }
        
        ApplyPaging(pageIndex: pqp.PageIndex, pageSize: pqp.PageSize);
    }

    private void FilterByGuestCount(PropertyQueryParameters pqp)
    {
        // Filter by adult count
        if (pqp.GuestCount > 0)
            AddFilter(p => p.MaxGuestCount >= pqp.GuestCount);
        
    }

    private void FilterByDateRange(PropertyQueryParameters pqp)
    {
        if (pqp is { CheckInDate: not null, CheckOutDate: not null })
        {
            AddFilter(p => p.Bookings.All(r => r.CheckInDate > pqp.CheckOutDate || r.CheckOutDate < pqp.CheckInDate));        
        }
    }

    private void FilterByRoomAndBedCount(PropertyQueryParameters pqp)
    {
        if (pqp.MinBedroomCount > 0)
            AddFilter(p => p.BedroomCount >= pqp.MinBedroomCount);
        if (pqp.MaxBedroomCount > 0)
            AddFilter(p => p.BedroomCount <= pqp.MaxBedroomCount);

        // Filter by bed count range
        if (pqp.MinBedCount > 0)
            AddFilter(p => p.BedCount >= pqp.MinBedCount);
        if (pqp.MaxBedCount > 0)
            AddFilter(p => p.BedCount <= pqp.MaxBedCount);

        // Filter by bathroom count range
        if (pqp.MinBathroomCount > 0)
            AddFilter(p => p.BathroomCount >= pqp.MinBathroomCount);
        if (pqp.MaxBathroomCount > 0)
            AddFilter(p => p.BathroomCount <= pqp.MaxBathroomCount);
    }

    // private void FilterByPriceRange(PropertyQueryParameters pqp)
    // {
    //     if (pqp.MinPrice > 0)
    //         AddFilter(p => p.PricePerNight >= pqp.MinPrice);
    //     if (pqp.MaxPrice > 0)
    //         AddFilter(p => p.PricePerNight <= pqp.MaxPrice);
    // }

    private void FilterByStatus(PropertyQueryParameters pqp)
    {
        if (pqp.Status is not null)
            AddFilter(p => p.Status == pqp.Status);
    }
}