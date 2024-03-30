using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWorks;

[Obsolete("Only use for tests!")]
public class DictionaryUnitOfWork : IUnitOfWork
{
    private Dictionary<Type, List<BaseEntity>> tables;

    public int CommitCount { get; private set; }

    public DictionaryUnitOfWork(IEnumerable<BaseEntity> records)
    {
        tables = new Dictionary<Type, List<BaseEntity>>();
        foreach (BaseEntity record in records)
        {
            if (!tables.TryGetValue(record.GetType(), out List<BaseEntity>? table))
            {
                table = new List<BaseEntity>();
                tables.Add(record.GetType(), table);
            }
            table.Add(record);
        }
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
    {
        if (!tables.TryGetValue(typeof(RecordType), out List<BaseEntity>? records))
            records = new List<BaseEntity>(0);

        return new DictionaryRepository<RecordType>(records.Select(r => (RecordType)r));
    }

    public void Commit() => CommitCount++;
}
