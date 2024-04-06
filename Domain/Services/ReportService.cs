
using Domain.Abstractions;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork unitOfWork)
{
    public IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report
    {
        return unitOfWork.Repository<ReportType>().GetAll();
    }
}
