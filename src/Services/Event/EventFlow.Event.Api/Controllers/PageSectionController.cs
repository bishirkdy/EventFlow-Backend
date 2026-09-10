using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.PageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.ReorderPageSections;
using EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection;
using EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/page-section")]
    [Authorize]
    public class PageSectionController(ISender sender, IMapper mapper) : ControllerBase
    {
        //Create event sections
        [HttpPost("{pageId:guid}/sections")]
        public async Task<IActionResult> CreatePageSection(Guid pageId, CreatePageSectionRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<CreatePageSectionCommand>(request) with
            {
                PageId = pageId
            };

            // Send command
            var sectionId = await sender.Send(command, cancellationToken);

            var response = new ApiResponse<Guid>
            {
                Success = true,
                Message = "Page section created successfully.",
                Data = sectionId
            };

            return Ok(response);
        }

        //Get page sections
        [HttpGet("{pageId:guid}/sections")]
        public async Task<IActionResult> GetPageSections(Guid pageId, CancellationToken cancellationToken)
        {
            // Send query
            var sections = await sender.Send(new GetPageSectionsQuery(pageId),cancellationToken);

            var response =
                new ApiResponse<IReadOnlyList<GetPageSectionsResponse>>
                {
                    Success = true,
                    Message = "Page sections retrieved successfully.",
                    Data = sections
                };

            return Ok(response);
        }

        //Update event sections by id
        [HttpPut("{pageId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> UpdatePageSection(Guid pageId,Guid id,UpdatePageSectionRequest request, CancellationToken cancellationToken)
        {
            // Map request to command
            var command = mapper.Map<UpdatePageSectionCommand>(request) with
            {
                PageId = pageId,
                Id = id
            };

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Page section updated successfully.",
                Data = null
            };

            return Ok(response);
        }

        [HttpDelete("{pageId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> DeletePageSection(Guid pageId,Guid id, CancellationToken cancellationToken)
        {
            // Send command
            await sender.Send(
                new DeletePageSectionCommand(pageId, id),
                cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Page section deleted successfully.",
                Data = null
            };

            return Ok(response);
        }

        //Changing the order in which sections appear on the page
        [HttpPut("{pageId:guid}/sections/reorder")]
        public async Task<IActionResult> ReorderPageSections(Guid pageId, [FromBody] IReadOnlyList<Guid> sectionIds, CancellationToken cancellationToken)
        {
            var command = new ReorderPageSectionsCommand(pageId , sectionIds);

            // Send command
            await sender.Send(command, cancellationToken);

            var response = new ApiResponse<object?>
            {
                Success = true,
                Message = "Page sections reordered successfully.",
                Data = null
            };

            return Ok(response);
        }
    }
}
