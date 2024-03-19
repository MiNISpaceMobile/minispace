using Domain.Abstractions;
using Domain.BaseTypes;

namespace Infrastructure.TableViews;

public class FakeTable<KeyType, RecordType> : ITable<KeyType, RecordType> where KeyType : notnull where RecordType : notnull, BaseEntity
{

}
