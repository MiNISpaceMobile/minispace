namespace Api.DTO.Reports;

public class ReportUpdateDateComparer : IComparer<ReportDto>
{
    public static ReportUpdateDateComparer Instance(bool ascending)
        => new(ascending);

    private bool Ascending { get; }
    private ReportUpdateDateComparer(bool ascending)
    {
        Ascending = ascending;
    }

    public int Compare(ReportDto? x, ReportDto? y)
    {
        var result = x!.UpdateDate.CompareTo(y.UpdateDate);
        return Ascending ? result : -result;
    }
}
