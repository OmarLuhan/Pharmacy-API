using System.Net;
using AutoMapper;
using Farma_api.Dto.Profile;
using Farma_api.Helpers;
using Farma_api.Helpers.Validations;
using Farma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = Statics.AuthSchema)]
public class ProfileController(IProfileService profileService, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BasicResponse<ProfileDto>>> GetProfile(Guid id)
    {
        var response = new BasicResponse<ProfileDto>();
        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Invalid id");
            return BadRequest(response);
        }

        try
        {
            var user = await profileService.GetProfile(id);
            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("User not found");
                return NotFound(response);
            }

            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<ProfileDto>(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(ex.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPut("uploadFile/{id:guid}")]
    public async Task<ActionResult<EmptyResponse>> UpdateProfile(Guid id, IFormFile file)
    {
        var response = new EmptyResponse();
        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Invalid id");
            return BadRequest(response);
        }

        switch (file.Length)
        {
            case 0:
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("File is empty");
                return BadRequest(response);
            case > 800 * 1024:
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("File size exceeds the limit of 800 KB");
                return BadRequest(response);
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".gif", ".png", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Invalid file type. Only JPG, GIF, and PNG files are allowed.");
            return BadRequest(response);
        }

        try
        {
            await profileService.UpdateFile(id, file);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.IsSuccess = false;
            response.ErrorMessage = [ex.Message];
            return StatusCode(500, response);
        }
    }

    [HttpDelete("RemoveFile/{id:guid}")]
    public async Task<ActionResult<EmptyResponse>> RemoveProfile(Guid id)
    {
        var response = new EmptyResponse();
        try
        {
            if (id == Guid.Empty)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Invalid id");
                return BadRequest(response);
            }
            await profileService.RemoveFile(id);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage = [ex.Message];
            return StatusCode(500, response);
        }
    }

    [HttpPut("ChangePassword/{id:guid}")]
    public async Task<ActionResult<EmptyResponse>> ChangePassword(Guid id, ChangeRequest req)
    {
        var response = new EmptyResponse();
        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Invalid id");
            return BadRequest(response);
        }

        var validator = new ChangeRequestValidator();
        var validationResult = await validator.ValidateAsync(req);
        if (!validationResult.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in validationResult.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            await profileService.ChangePassword(req);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(ex.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPut("UpdateProfile/{id:guid}")]
    public async Task<ActionResult<EmptyResponse>> UpdateProfile(Guid id, ProfileUpdateDto profile)
    {
        var response = new EmptyResponse();
        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Invalid id");
            return BadRequest(response);
        }

        var validator = new ProfileUpdateValidator();
        var validationResult = await validator.ValidateAsync(profile);
        if (!validationResult.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in validationResult.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            var user = await profileService.GetProfile(id);
            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("User not found");
                return NotFound(response);
            }

            user.Nombre = profile.Nombre;
            user.Correo = profile.Correo;
            await profileService.UpdateProfile(user);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(ex.Message);
            return StatusCode(500, response);
        }
    }
}