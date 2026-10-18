using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.Abstractions.Services;
using EventFlow.Identity.Domain.Entities;
using EventFlow.Identity.Domain.Enums;
using EventFlow.Messaging.Outbox;
using EventFlow.Security.Configuration;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace EventFlow.Identity.Application.Features.EventInvitations.Commands.AcceptEventInvitation;

public class AcceptEventInvitationCommandHandler : IRequestHandler<AcceptEventInvitationCommand, AcceptEventInvitationResponse>
{
    private readonly IEventInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserEventRoleRepository _userEventRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly IOutbox _outbox;

    public AcceptEventInvitationCommandHandler(
        IEventInvitationRepository invitationRepository,
        IUserRepository userRepository,
        IUserEventRoleRepository userEventRoleRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtOptions> jwtOptions,
        IOutbox outbox)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _userEventRoleRepository = userEventRoleRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions.Value;
        _outbox = outbox;
    }

    public async Task<AcceptEventInvitationResponse> Handle(AcceptEventInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (invitation is null)
        {
            throw new NotFoundException("Invalid invitation token.");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new ConflictException("Invitation has already been processed or revoked.");
        }

        if (invitation.IsExpired)
        {
            throw new ConflictException("Invitation has expired.");
        }

        var user = await _userRepository.GetByEmailAsync(invitation.Email, cancellationToken);
        Guid userId;
        string? accessToken = null;
        string? refreshToken = null;

        if (user is null)
        {
            var passwordHash = _passwordHasher.Hash(request.Password);
            user = new User(request.UserName, invitation.Email, request.FirstName, request.LastName, passwordHash);
            await _userRepository.AddAsync(user, cancellationToken);
            userId = user.Id;

            // A brand new account is signed in right away so the invitation link
            // takes the invitee straight to their dashboard. The existing account
            // branch stays token-less and must use the authenticated accept flow.
            accessToken = _jwtService.GenerateAccessToken(user);
            refreshToken = _refreshTokenGenerator.Generate();
            await _refreshTokenRepository.AddAsync(
                new RefreshToken(
                    user.Id,
                    refreshToken,
                    DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)),
                cancellationToken);
        }
        else
        {
            userId = user.Id;
            user.UpdateName(request.FirstName, request.LastName);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        await EventInvitationAcceptance.AssignRoleAsync(
            _userEventRoleRepository,
            invitation,
            userId,
            cancellationToken);

        invitation.Accept(userId);
        EventInvitationAcceptance.EnqueueAcceptedIfAttendance(_outbox, invitation, userId);
        await _invitationRepository.UpdateAsync(invitation, cancellationToken);

        return new AcceptEventInvitationResponse(userId, invitation.Id, accessToken, refreshToken);
    }
}
