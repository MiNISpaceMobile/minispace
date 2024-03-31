using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;
using System.Collections.Generic;

namespace Infrastructure.UnitOfWorks;

[Obsolete("Only use for tests!")]
public class DictionaryUnitOfWork : IUnitOfWork
{
    private Dictionary<Type, Dictionary<Guid, BaseEntity>> tables;

    public int CommitCount { get; private set; }

    public DictionaryUnitOfWork(IEnumerable<BaseEntity> records)
    {
        tables = new Dictionary<Type, Dictionary<Guid, BaseEntity>>();
        foreach (BaseEntity record in records)
        {
            if (!tables.TryGetValue(record.GetType(), out Dictionary<Guid, BaseEntity>? table))
            {
                table = new Dictionary<Guid, BaseEntity>();
                tables.Add(record.GetType(), table);
            }
            table.Add(record.Guid, record);
        }
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
    {
        if (!tables.TryGetValue(typeof(RecordType), out Dictionary<Guid, BaseEntity>? records))
            records = new Dictionary<Guid, BaseEntity>(0);

        return new DictionaryRepository<RecordType>(records);
    }

    public void Commit() => CommitCount++;
}
