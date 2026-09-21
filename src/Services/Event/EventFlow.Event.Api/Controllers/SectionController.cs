using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.Sections;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Features.Sections.Commands.CreateSection;
using EventFlow.Event.Application.Features.Sections.Commands.DeleteSection;
using EventFlow.Event.Application.Features.Sections.Commands.UpdateSection;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionById;
using EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/section")]
    [Authorize]
    public class SectionController(ISender sender) : ControllerBase
    {
        [HttpPost("{eventId:guid}/sections")]
        public async Task<IActionResult> CreateSection(Guid eventId,CreateSectionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateSectionCommand(
                eventId,
                request.Name,
                request.Description,
                request.DisplayOrder
            );

            var sectionId = await sender.Send(command,cancellationToken);
            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Section created successfully.",
                Data = sectionId
            };

            return Ok(response);
        }

        [HttpGet("{eventId:guid}/sections")]
        public async Task<IActionResult> GetSectionsByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            var sections = await sender.Send(new GetSectionsByEventQuery(eventId),cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetSectionsByEventResponse>>
                {
                    Success = true,
                    Message = "Sections retrieved successfully.",
                    Data = sections
                };

            return Ok(response);
        }

        [HttpGet("sections/{id:guid}")]
        public async Task<IActionResult> GetSectionById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var section = await sender.Send(
                new GetSectionByIdQuery(id),
                cancellationToken);

            if (section is null)
                throw new NotFoundException("Section not found.");

            var response = new ApiResponse<GetSectionByIdResponse>
            {
                Success = true,
                Message = "Section retrieved successfully.",
                Data = section
            };

            return Ok(response);
        }

        [HttpPut("{eventId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> UpdateSection(Guid eventId,Guid id,UpdateSectionRequest request,CancellationToken cancellationToken)
        {
            var command = new UpdateSectionCommand(
                id,
                eventId,
                request.Name,
                request.Description,
                request.DisplayOrder
            );

            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Section updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        [HttpDelete("{eventId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> DeleteSection(Guid eventId,Guid id,CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteSectionCommand(id, eventId), cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Section deleted successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}