using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO.Reports;

public record ReportDto(
    Guid Guid,
    UserDto? Author,
    UserDto? Responder,
    Guid TargetId,
    string Title,
    string Details,
    DateTime CreationDate,
    DateTime UpdateDate,
    string? Feedback,
    bool Open,
    ReportType ReportType);
