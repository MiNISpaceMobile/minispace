using Domain.BaseTypes;

namespace Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity;

    public void Commit();
}
