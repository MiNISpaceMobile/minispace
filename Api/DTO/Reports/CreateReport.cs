﻿using Domain.DataModel;

namespace Api.DTO.Reports;

public record CreateReport(
    Guid TargetId,
    string Title,
    string Details,
    ReportType ReportType);
