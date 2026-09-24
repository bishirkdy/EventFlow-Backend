using AutoMapper;
using EventFlow.Event.Api.Common.Models;
using EventFlow.Event.Api.Requests.PageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection;
using EventFlow.Event.Application.Features.PageSection.Commands.ReorderPageSections;
using EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection;
using EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections;
using EventFlow.Infrastructure.Storage.Cloudinary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.Event.Api.Controllers
{
    [ApiController]
    [Route("api/v1/page-section")]
    [Authorize]
    public class PageSectionController(ISender sender, ICloudinaryStorage cloudinaryStorage) : ControllerBase
    {
        //Create event sections
        [HttpPost("{pageId:guid}/sections")]
        public async Task<IActionResult> CreatePageSection(Guid pageId,[FromForm] CreatePageSectionRequest request,CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            string? imagePublicId = null;

            if (request.Image is not null)
            {
                await using var stream = request.Image.OpenReadStream();

                var uploadedFile = await cloudinaryStorage.UploadAsync(
                    stream,
                    request.Image.FileName,
                    request.Image.ContentType,
                    $"events/{pageId}/page-sections",
                    cancellationToken);

                imageUrl = uploadedFile.Url;
                imagePublicId = uploadedFile.PublicId;
            }

            var command = new CreatePageSectionCommand(
                pageId,
                request.SectionType,
                request.Title,
                request.Content,
                imageUrl,
                imagePublicId,
                request.DisplayOrder,
                request.Configuration);

            var sectionId = await sender.Send(command, cancellationToken);

            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message = "Page section created successfully.",
                Data = sectionId
            });
        }

        //Get page sections
        [AllowAnonymous]
        [HttpGet("{pageId:guid}/sections")]
        public async Task<IActionResult> GetPageSections(Guid pageId, CancellationToken cancellationToken)
        {
            // Send query
            var sections = await sender.Send(new GetPageSectionsQuery(pageId),cancellationToken);

            var response = new ApiResponse<IReadOnlyList<GetPageSectionsResponse>>
                {
                    Success = true,
                    Message = "Page sections retrieved successfully.",
                    Data = sections
                };

            return Ok(response);
        }

        //Update event sections by id
        [HttpPut("{pageId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> UpdatePageSection(Guid pageId, Guid id,
           [FromForm] UpdatePageSectionRequest request,CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            string? imagePublicId = null;

            if (request.Image is not null)
            {
                await using var stream = request.Image.OpenReadStream();

                var uploadedFile = await cloudinaryStorage.UploadAsync(
                    stream,
                    request.Image.FileName,
                    request.Image.ContentType,
                    $"events/{pageId}/page-sections",
                    cancellationToken);

                imageUrl = uploadedFile.Url;
                imagePublicId = uploadedFile.PublicId;
            }

            var command = new UpdatePageSectionCommand(
                id,
                pageId,
                request.SectionType,
                request.Title,
                request.Content,
                imageUrl,
                imagePublicId,
                request.DisplayOrder,
                request.IsVisible,
                request.Configuration);

            var oldImagePublicId = await sender.Send(command,cancellationToken);

            // New image replaced the old one
            if (!string.IsNullOrWhiteSpace(oldImagePublicId))
            {
                await cloudinaryStorage.DeleteAsync(oldImagePublicId, cancellationToken);
            }

            return Ok(new ApiResponse<object?>
            {
                Success = true,
                Message = "Page section updated successfully.",
                Data = null
            });
        }

        //Delete page section
        [HttpDelete("{pageId:guid}/sections/{id:guid}")]
        public async Task<IActionResult> DeletePageSection(Guid pageId,Guid id,CancellationToken cancellationToken)
        {
            var imagePublicId = await sender.Send(new DeletePageSectionCommand(pageId, id),cancellationToken);

            if (!string.IsNullOrWhiteSpace(imagePublicId))
            {
                await cloudinaryStorage.DeleteAsync(imagePublicId,cancellationToken);
            }

            return Ok(new ApiResponse<object?>
            {
                Success = true,
                Message = "Page section deleted successfully.",
                Data = null
            });
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
