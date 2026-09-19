using TimeZoneConverter;

namespace EventFlow.Event.Application.Common;

public static class TimeZoneHelper
{
    public static bool TryGetTimeZone(string? timeZoneId, out TimeZoneInfo? timeZone)
    {
        timeZone = null;

        if (string.IsNullOrWhiteSpace(timeZoneId))
            return false;

        try
        {
            timeZone = TZConvert.GetTimeZoneInfo(timeZoneId.Trim());
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static TimeZoneInfo GetTimeZone(string timeZoneId)
    {
        if (!TryGetTimeZone(timeZoneId, out var timeZone) || timeZone is null)
            throw new TimeZoneNotFoundException($"Time zone '{timeZoneId}' was not found.");

        return timeZone;
    }
}
