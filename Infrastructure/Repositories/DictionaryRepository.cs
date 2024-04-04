using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.UnitOfWorks;

namespace Infrastructure.Repositories;

[Obsolete("Only use for tests!")]
public class DictionaryRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private DictionaryUnitOfWork uow;
    private Dictionary<Guid, BaseEntity> Table
    {
        get
        {
            if (uow.Disposed)
                throw new InvalidOperationException("UnitOfWork was disposed");

            return uow.tables[typeof(RecordType)];
        }
    }

    public DictionaryRepository(DictionaryUnitOfWork uow)
    {
        this.uow = uow;
    }

    public void Add(RecordType record)
    {
        Table.Add(record.Guid, record);
    }

    public RecordType? Get(Guid guid)
    {
        Table.TryGetValue(guid, out BaseEntity? record);
        return (RecordType?)record;
    }
    public IQueryable<RecordType> GetAll() => Table.Values.Select(r => (RecordType)r).AsQueryable<RecordType>();

    public bool TryDelete(Guid guid)
    {
        var record = Get(guid);
        if (record is null)
            return false;
        Table.Remove(record.Guid);
        return true;
    }
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = GetAll().Where(predicate);
        foreach (var record in selected)
            Table.Remove(record.Guid);
        return selected.Count();
    }
}
