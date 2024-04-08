using Domain.Abstractions;
using Domain.BaseTypes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DatabaseRepository<RecordType> : IRepository<RecordType> where RecordType : notnull, BaseEntity
{
    private DbSet<RecordType> table;

    public DatabaseRepository(DbSet<RecordType> table)
    {
        this.table = table;
    }

    public void Add(RecordType record) => table.Add(record);
    public void AddMany(IEnumerable<RecordType> records) => table.AddRange(records);

    public RecordType? Get(Guid guid) => table.FirstOrDefault(r => r.Guid == guid);
    public IQueryable<RecordType> GetAll() => table;

    public bool TryDelete(Guid guid)
    {
        var record = Get(guid);
        if (record is null)
            return false;
        table.Remove(record);
        return true;
    }
    public int DeleteAll(Func<RecordType, bool> predicate)
    {
        var selected = table.Where(predicate);
        table.RemoveRange(selected);
        return selected.Count();
    }
}
