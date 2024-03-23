using Domain.BaseTypes;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstractions;

public interface IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    void Add(RecordType record);

    RecordType? Get(ulong id);
    RecordType? Get(Guid guid);
    bool TryGet(ulong id, [MaybeNullWhen(false)] out RecordType record) => (record = Get(id)) is not null;
    bool TryGet(Guid guid, [MaybeNullWhen(false)] out RecordType record) => (record = Get(guid)) is not null;
    IEnumerable<RecordType> GetAll();

    bool TryDelete(ulong id);
    bool TryDelete(Guid guid);
    int DeleteAll(Func<RecordType, bool> predicate);

    void SaveChanges();
}
