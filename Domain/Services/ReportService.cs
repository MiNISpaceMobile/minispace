
using Domain.Abstractions;
using Domain.DataModel;

namespace Domain.Services;

public class ReportService(IUnitOfWork unitOfWork)
{
    public IEnumerable<ReportType> GetAll<ReportType>() where ReportType : Report
    {
        return unitOfWork.Repository<ReportType>().GetAll();
    }
    public ReportType GetByGuid<ReportType>(Guid guid) where ReportType : Report
    {
        var report = unitOfWork.Repository<ReportType>().Get(guid);
        // TODO: Throw custom exception
        return report is not null ? report : throw new Exception("Invalid guid");
    }

}
