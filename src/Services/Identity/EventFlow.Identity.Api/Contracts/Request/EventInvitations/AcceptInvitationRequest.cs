namespace EventFlow.Identity.Api.Contracts.Request.EventInvitations;

public sealed record AcceptInvitationRequest(
    string Password,
    string UserName,
    string FirstName,
    string LastName);
