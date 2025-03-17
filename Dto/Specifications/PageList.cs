using Microsoft.EntityFrameworkCore;

namespace Farma_api.Dto.Specifications;

public class PageList<T> : List<T> where T : class
{
    private PageList(List<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
        AddRange(items);
    }

    public MetaData MetaData { get; set; }

    public static async Task<PageList<T>> ToPageList(IQueryable<T> entity, int pageNumber, int pageSize)
    {
        List<T> items = await entity.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await entity.CountAsync();
        return new PageList<T>(items, count, pageNumber, pageSize);
    }
}