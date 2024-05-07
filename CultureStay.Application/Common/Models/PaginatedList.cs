using Microsoft.EntityFrameworkCore;

namespace CultureStay.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Data { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> data, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Data = data;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var data = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(data, count, pageNumber, pageSize);
    }
}