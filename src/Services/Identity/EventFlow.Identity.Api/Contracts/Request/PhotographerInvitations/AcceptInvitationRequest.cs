namespace EventFlow.Identity.Api.Contracts.Request.PhotographerInvitations;

public sealed record AcceptInvitationRequest(
    string Password,
    string UserName,
    string FirstName,
    string LastName);
