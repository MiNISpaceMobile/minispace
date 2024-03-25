using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWorks;

[Obsolete("Only use for tests!")]
public class DictionaryUnitOfWork : IUnitOfWork
{
    private Dictionary<Type, IEnumerable<BaseEntity>> records;

    public int CommitCount { get; private set; }

    public DictionaryUnitOfWork(Dictionary<Type, IEnumerable<BaseEntity>> records)
    {
        this.records = records;
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
    {
        if (!records.TryGetValue(typeof(RecordType), out IEnumerable<BaseEntity>? entities))
            entities = Enumerable.Empty<RecordType>();

        return new DictionaryRepository<RecordType>((IEnumerable<RecordType>)entities);
    }

    public void Commit() => CommitCount++;
}
