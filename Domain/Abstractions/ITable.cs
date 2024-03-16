using Domain.BaseTypes;

namespace Domain.Abstractions;

public interface ITable<KeyType, RecordType> where KeyType : notnull where RecordType : notnull, BaseEntity
{
    
}
