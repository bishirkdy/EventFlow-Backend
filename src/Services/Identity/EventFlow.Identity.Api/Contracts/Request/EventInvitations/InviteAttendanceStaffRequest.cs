using EventFlow.Identity.Domain.Enums;

namespace EventFlow.Identity.Api.Contracts.Request.EventInvitations;

public sealed record InviteAttendanceStaffRequest(
    string Email,
    InvitationScopeType ScopeType,
    Guid? ScopeId);
