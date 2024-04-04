using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;
using System.Collections.Generic;

namespace Infrastructure.UnitOfWorks;

[Obsolete("Only use for tests!")]
public class DictionaryUnitOfWork : IUnitOfWork
{
    public Dictionary<Type, Dictionary<Guid, BaseEntity>> Tables { get; }
    public int CommitCount { get; private set; }
    public bool Disposed { get; private set; }

    public DictionaryUnitOfWork(IEnumerable<BaseEntity> records)
    {
        Tables = new Dictionary<Type, Dictionary<Guid, BaseEntity>>();

        foreach (BaseEntity record in records)
        {
            foreach (Type type in record.GetType().InheritancePathUpTo<BaseEntity>())
                if (!Tables.TryAdd(type, new Dictionary<Guid, BaseEntity>()))
                    break;
            Add(record);
        }

        CommitCount = 0;
        Disposed = false;
    }

    public void Add(BaseEntity record)
    {
        if (record.Guid == Guid.Empty)
            record.Guid = Guid.NewGuid();
        while (!TryAdd(record))
            record.Guid = Guid.NewGuid();
    }
    public bool TryAdd(BaseEntity record)
    {
        if (Tables[record.GetType().InheritancePathUpTo<BaseEntity>().Last()].ContainsKey(record.Guid))
            return false;

        foreach (Type type in record.GetType().InheritancePathUpTo<BaseEntity>())
            Tables[type].Add(record.Guid, record);

        return true;
    }

    public bool TryDelete(BaseEntity record)
    {
        if (!Tables[record.GetType()].ContainsKey(record.Guid))
            return false;

        foreach (Type type in record.GetType().InheritancePathUpTo<BaseEntity>())
            Tables[type].Remove(record.Guid);

        return true;
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
    {
        if (Disposed)
            throw new InvalidOperationException("UnitOfWork was disposed");

        foreach (Type type in typeof(RecordType).InheritancePathUpTo<BaseEntity>())
            if (!Tables.TryAdd(type, new Dictionary<Guid, BaseEntity>()))
                break;

        return new DictionaryRepository<RecordType>(this);
    }

    public void Commit() => CommitCount++;

    public void Dispose() => Disposed = true;
}
