using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Common.Certificates;

public static class CertificateRules
{
    public static List<string> Evaluate(
        bool requireApproved,
        double? minAttendancePercent,
        RegistrationStatus status,
        bool hasParticipant,
        double? attendancePercent)
    {
        var reasons = new List<string>();

        if (!hasParticipant)
        {
            reasons.Add("No participant profile.");
        }

        if (status is RegistrationStatus.Cancelled or RegistrationStatus.Rejected)
        {
            reasons.Add(
                status == RegistrationStatus.Cancelled
                    ? "Registration was cancelled."
                    : "Registration was rejected.");
        }
        else if (requireApproved &&
                 status != RegistrationStatus.Approved)
        {
            reasons.Add("Registration is not approved.");
        }

        if (minAttendancePercent.HasValue)
        {
            if (attendancePercent is null)
            {
                reasons.Add("Attendance could not be verified.");
            }
            else if (attendancePercent.Value < minAttendancePercent.Value)
            {
                reasons.Add(
                    $"Attendance {attendancePercent.Value:0.#}% is below the required {minAttendancePercent.Value:0.#}%.");
            }
        }

        return reasons;
    }
}
