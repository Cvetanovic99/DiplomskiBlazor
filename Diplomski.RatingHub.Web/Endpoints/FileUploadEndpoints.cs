using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.RatingHub.Web.Endpoints;

public static class FileUploadEndpoints
{
    public static IEndpointRouteBuilder MapFileUploadEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/upload/image/company-image", async (
            [FromServices] IFileService fileService,
            [FromForm] IFormFile? file) =>
        {
            try
            {
                var response = await fileService.UploadImageAsync(file, "companyImages");
                return Results.Ok(response);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }).DisableAntiforgery();;

        endpoints.MapDelete("/api/upload/image/company-image", (
            [FromServices] IFileService fileService,
            [FromQuery] string path) =>
        {
            try
            {
                fileService.DeleteImage(path);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        
        endpoints.MapPost("/api/upload/image/review-image", async (
            [FromServices] IFileService fileService,
            [FromForm] IFormFile? file) =>
        {
            try
            {
                var response = await fileService.UploadImageAsync(file, "reviewImages");
                return Results.Ok(response);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }).DisableAntiforgery();;

        endpoints.MapDelete("/api/upload/image/review-image", (
            [FromServices] IFileService fileService,
            [FromQuery] string path) =>
        {
            try
            {
                fileService.DeleteImage(path);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });

        return endpoints;
    }
}