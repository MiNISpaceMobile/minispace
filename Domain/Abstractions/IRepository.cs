using Domain.BaseTypes;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstractions;

public interface IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    void Add(RecordType record);
    void AddMany(IEnumerable<RecordType> records);

    RecordType? Get(Guid guid);
    bool TryGet(Guid guid, [MaybeNullWhen(false)] out RecordType record) => (record = Get(guid)) is not null;
    IQueryable<RecordType> GetAll();

    void Delete(RecordType record);
    bool TryDelete(Guid guid)
    {
        bool result;
        if (result = TryGet(guid, out RecordType? record))
            Delete(record!);
        return result;
    }
    int DeleteAll(Func<RecordType, bool> predicate);
}
