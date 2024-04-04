using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;
using System.Collections.Generic;

namespace Infrastructure.UnitOfWorks;

[Obsolete("Only use for tests!")]
public class DictionaryUnitOfWork : IUnitOfWork
{
    public Dictionary<Type, Dictionary<Guid, BaseEntity>> tables;

    public int CommitCount { get; private set; }
    public bool Disposed { get; private set; }

    public DictionaryUnitOfWork(IEnumerable<BaseEntity> records)
    {
        tables = new Dictionary<Type, Dictionary<Guid, BaseEntity>>();
        foreach (BaseEntity record in records)
        {
            if (!tables.TryGetValue(record.GetType(), out Dictionary<Guid, BaseEntity>? table))
                tables.Add(record.GetType(), table = new Dictionary<Guid, BaseEntity>());
            table.Add(record.Guid, record);
        }

        CommitCount = 0;
        Disposed = false;
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
    {
        if (Disposed)
            throw new InvalidOperationException("UnitOfWork was disposed");

        tables.TryAdd(typeof(RecordType), new Dictionary<Guid, BaseEntity>());

        return new DictionaryRepository<RecordType>(this);
    }

    public void Commit() => CommitCount++;

    public void Dispose() => Disposed = true;
}
