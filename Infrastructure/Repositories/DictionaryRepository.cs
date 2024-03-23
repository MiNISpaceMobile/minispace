using Domain.Abstractions;
using Domain.BaseTypes;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Repositories;

public class DictionaryRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private Dictionary<ulong, RecordType> records;

    public int SaveChangesCounter { get; private set; }

    public DictionaryRepository(Dictionary<ulong, RecordType> records)
    {
        this.records = records;
        SaveChangesCounter = 0;
    }

    public void Add(RecordType record) => records.Add(record.Id, record);

    public RecordType? Get(ulong key)
    {
        records.TryGetValue(key, out RecordType? record);
        return record;
    }

    public bool TryGet(ulong key, [MaybeNullWhen(false)] out RecordType record) => records.TryGetValue(key, out record);

    public IEnumerable<RecordType> GetAll() => records.Values;

    public bool TryDelete(ulong key) => records.Remove(key);

    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = records.Values.Where(predicate);
        foreach (var record in selected)
            records.Remove(record.Id);
        return selected.Count();
    }

    public void SaveChanges() => SaveChangesCounter++;
}
