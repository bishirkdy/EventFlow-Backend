using System.Globalization;

namespace EventFlow.Registration.Application.Common.Certificates;

public static class CertificateEventText
{
    public static string FormatEventDates(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date == endDate.Date)
        {
            return startDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
        }

        if (startDate.Year == endDate.Year &&
            startDate.Month == endDate.Month)
        {
            return $"{startDate.Day} - {endDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}";
        }

        if (startDate.Year == endDate.Year)
        {
            return $"{startDate.ToString("d MMM", CultureInfo.InvariantCulture)} - {endDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}";
        }

        return $"{startDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)} - {endDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)}";
    }
}
