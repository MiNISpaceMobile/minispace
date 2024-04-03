using Domain.Abstractions;
using Domain.BaseTypes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DatabaseRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private DbSet<RecordType> records;

    public DatabaseRepository(DbSet<RecordType> records)
    {
        this.records = records;
    }

    public void Add(RecordType record) => records.Add(record);

    public RecordType? Get(Guid guid) => records.FirstOrDefault(r => r.Guid == guid);
    public IQueryable<RecordType> GetAll() => records;

    public bool TryDelete(Guid guid)
    {
        var record = Get(guid);
        if (record is null)
            return false;
        records.Remove(record);
        return true;
    }
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = records.Where(predicate);
        records.RemoveRange(selected);
        return selected.Count();
    }
}
