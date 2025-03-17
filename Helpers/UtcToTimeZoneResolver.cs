using AutoMapper;

namespace Farma_api.Helpers;

public class UtcToTimeZoneResolver<TSource, TDestination>(string zone)
    : IValueResolver<TSource, TDestination, DateTime>
{
    public DateTime Resolve(TSource source, TDestination destination, DateTime sourceMember, ResolutionContext? context)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone);
        return TimeZoneInfo.ConvertTimeFromUtc(sourceMember, timeZone);
    }
}