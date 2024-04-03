using Domain.BaseTypes;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstractions;

public interface IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    void Add(RecordType record);

    RecordType? Get(Guid guid);
    bool TryGet(Guid guid, [MaybeNullWhen(false)] out RecordType record) => (record = Get(guid)) is not null;
    IQueryable<RecordType> GetAll();

    bool TryDelete(Guid guid);
    int DeleteAll(Func<RecordType, bool> predicate);
}
