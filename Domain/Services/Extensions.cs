using Domain.Abstractions;
using Domain.BaseTypes;

namespace Domain.Services;

public static class Extensions
{
    public static RecordType GetOrThrow<RecordType>(this IRepository<RecordType> repository, Guid guid) where RecordType : notnull, BaseEntity
        => repository.Get(guid) ?? throw new InvalidGuidException();
}
