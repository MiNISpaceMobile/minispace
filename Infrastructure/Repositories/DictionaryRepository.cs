using Domain.Abstractions;
using Domain.BaseTypes;

namespace Infrastructure.Repositories;

[Obsolete("Only use for tests!")]
public class DictionaryRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private Dictionary<Guid, BaseEntity> table;

    public DictionaryRepository(Dictionary<Guid, BaseEntity> table)
    {
        this.table = table;
    }

    public void Add(RecordType record)
    {
        table.Add(record.Guid, record);
    }

    public RecordType? Get(Guid guid)
    {
        table.TryGetValue(guid, out BaseEntity? record);
        return (RecordType?)record;
    }
    public IEnumerable<RecordType> GetAll() => table.Values.Select(r => (RecordType)r);

    public bool TryDelete(Guid guid)
    {
        var record = Get(guid);
        if (record is null)
            return false;
        table.Remove(record.Guid);
        return true;
    }
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = GetAll().Where(predicate);
        foreach (var record in selected)
            table.Remove(record.Guid);
        return selected.Count();
    }
}
