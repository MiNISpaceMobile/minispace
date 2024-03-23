using Domain.BaseTypes;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstractions;

public interface IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    void Add(RecordType record);

    RecordType? Get(ulong key);
    bool TryGet(ulong key, [MaybeNullWhen(false)] out RecordType record) => (record = Get(key)) is not null;
    IEnumerable<RecordType> GetAll();

    bool TryDelete(ulong key);
    int DeleteAll(Func<RecordType, bool> predicate);

    void SaveChanges();
}
