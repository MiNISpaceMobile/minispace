using Domain.Abstractions;
using Domain.BaseTypes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DatabaseRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private DbContext dbContext;
    private DbSet<RecordType> records;

    public DatabaseRepository(DbContext dbContext)
    {
        this.dbContext = dbContext;
        records = dbContext.Set<RecordType>();
    }

    public void Add(RecordType record) => records.Add(record);

    public RecordType? Get(ulong key) => records.Find(key);

    public IEnumerable<RecordType> GetAll() => records;

    public bool TryDelete(ulong key)
    {
        var record = records.Find(key);
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

    public void SaveChanges() => dbContext.SaveChanges();
}
