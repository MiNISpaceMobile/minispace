using Domain.Abstractions;
using Domain.BaseTypes;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWorks;

public class DatabaseUnitOfWork : IUnitOfWork
{
    private DbContext dbContext;

    public DatabaseUnitOfWork(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IRepository<RecordType> Repository<RecordType>() where RecordType : notnull, BaseEntity
        => new DatabaseRepository<RecordType>(dbContext.Set<RecordType>());

    public void Commit() => dbContext.SaveChanges();
}
