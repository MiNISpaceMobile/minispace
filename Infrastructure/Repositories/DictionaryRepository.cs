using Domain.Abstractions;
using Domain.BaseTypes;

namespace Infrastructure.Repositories;

public class DictionaryRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private Dictionary<ulong, RecordType> recordsById;
    private Dictionary<Guid, RecordType> recordsByGuid;

    public int SaveChangesCounter { get; private set; }

    public DictionaryRepository(IEnumerable<RecordType> records)
    {
        recordsById = records.ToDictionary(r => r.Id);
        recordsByGuid = records.ToDictionary(r => r.Guid);
        SaveChangesCounter = 0;
    }

    public void Add(RecordType record)
    {
        recordsById.Add(record.Id, record);
        recordsByGuid.Add(record.Guid, record);
    }

    public RecordType? Get(ulong id)
    {
        recordsById.TryGetValue(id, out RecordType? record);
        return record;
    }
    public RecordType? Get(Guid guid)
    {
        recordsByGuid.TryGetValue(guid, out RecordType? record);
        return record;
    }
    public IEnumerable<RecordType> GetAll() => recordsById.Values;

    public bool TryDelete(ulong id)
    {
        var record = Get(id);
        if (record is null)
            return false;
        recordsById.Remove(record.Id);
        recordsByGuid.Remove(record.Guid);
        return true;
    }
    public bool TryDelete(Guid guid)
    {
        var record = Get(guid);
        if (record is null)
            return false;
        recordsById.Remove(record.Id);
        recordsByGuid.Remove(record.Guid);
        return true;
    }
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = recordsById.Values.Where(predicate);
        foreach (var record in selected)
        {
            recordsById.Remove(record.Id);
            recordsByGuid.Remove(record.Guid);
        }
        return selected.Count();
    }

    public void SaveChanges() => SaveChangesCounter++;
}
