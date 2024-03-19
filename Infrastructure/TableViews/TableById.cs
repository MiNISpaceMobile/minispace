using Domain.Abstractions;
using Domain.BaseTypes;

namespace Infrastructure.TableViews;

public class TableById<RecordType> : ITable<ulong, RecordType> where RecordType : notnull, BaseEntity
{

}
