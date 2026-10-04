using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Certificates;
using MediatR;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;

public sealed record GetMyCertificatesAcrossEventsQuery
    : IRequest<ApiResponse<List<CertificateDto>>>;
