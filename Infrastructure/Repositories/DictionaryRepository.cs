using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.UnitOfWorks;

namespace Infrastructure.Repositories;

[Obsolete("Only use for tests!")]
public class DictionaryRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private DictionaryUnitOfWork uow;
    private DictionaryUnitOfWork UnitOfWork
        => !uow.Disposed ? uow : throw new ObjectDisposedException(nameof(DictionaryUnitOfWork));

    public DictionaryRepository(DictionaryUnitOfWork uow)
    {
        this.uow = uow;
    }

    public void Add(RecordType record) => UnitOfWork.Add(record);
    public void AddMany(IEnumerable<RecordType> records)
    {
        foreach (var record in records)
            Add(record);
    }

    public RecordType? Get(Guid guid)
    {
        UnitOfWork.Tables[typeof(RecordType)].TryGetValue(guid, out BaseEntity? record);
        return (RecordType?)record;
    }
    public IQueryable<RecordType> GetAll() => UnitOfWork.Tables[typeof(RecordType)].Values.Select(r => (RecordType)r).AsQueryable();

    public void Delete(RecordType record) => UnitOfWork.TryDelete(record);
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = GetAll().Where(predicate);
        int count = selected.Count();
        foreach (var record in selected)
            UnitOfWork.TryDelete(record);
        return count;
    }
}
