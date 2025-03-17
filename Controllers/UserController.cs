using System.Net;
using AutoMapper;
using Farma_api.Dto.Product;
using Farma_api.Dto.User;
using Farma_api.Helpers;
using Farma_api.Helpers.Validations;
using Farma_api.Models;
using Farma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
public class UserController(IUserService service, IMapper mapper, ISaleService saleService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<IEnumerable<UserDto>>>> GetAll()
    {
        var response = new BasicResponse<IEnumerable<UserDto>>();
        try
        {
            IEnumerable<Usuario> users = await service.List();
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage = [ex.Message];
            return StatusCode((int)response.Status, response);
        }
    }

    [HttpGet("{id:guid}", Name = "UserById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<UserDto>>> GetByGuid(Guid id)
    {
        var response = new BasicResponse<UserDto>();
        try
        {
            var user = await service.GetByGuid(id);
            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessage.Add("Usuario no encontrado");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<UserDto>(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasicResponse<UserDto>>> Create([FromBody] UserCreateDto dto)
    {
        var response = new BasicResponse<UserDto>();
        var validator = new UserCreateValidator();
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(response);
        }

        try
        {
            if (!await service.IsUnique(dto.Correo))
            {
                response.Status = HttpStatusCode.Conflict;
                response.IsSuccess = false;
                response.ErrorMessage.Add("Correo ya registrado");
                return Conflict(response);
            }

            var user = mapper.Map<Usuario>(dto);
            user = await service.Create(user);
            if (user == null)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("No se pudo crear el usuario");
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<UserDto>(user);
            return CreatedAtRoute("UserById", new { id = user.Id }, response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
     [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, UserUpdateDto dto)
    {
        var response = new EmptyResponse();
        var validator = new UserUpdateValidator();

        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.NotAcceptable;
            response.ErrorMessage.Add("guid no válido");
            return BadRequest(response);
        }

        if (id != dto.Id)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("guid no coincide con el id del usuario");
            return BadRequest(response);
        }

        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.NotAcceptable;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            if (!await service.IsUnique(dto.Correo, id))
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("El correo pertenece a otro usuario");
                return BadRequest(response);
            }

            var currentUser = await service.GetByGuid(id);
            if (currentUser == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Usuario no encontrado");
                return NotFound(response);
            }

            currentUser.Nombre = dto.Nombre;
            currentUser.Correo = dto.Correo;
            currentUser.Activo = dto.Activo;
            currentUser.RolId = dto.RolId;

            await service.Update(currentUser);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
     [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = new EmptyResponse();
        if (id == Guid.Empty)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("guid no válido");
            return BadRequest(response);
        }

        if (await saleService.HasUserSale(id))
        {
            response.Status = HttpStatusCode.Conflict;
            response.ErrorMessage.Add("El usuario tiene ventas registradas");
            return Conflict(response);
        }

        try
        {
            var user = await service.GetByGuid(id);
            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Usuario no encontrado");
                return NotFound(response);
            }

            await service.Delete(user);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpGet("Search/{value:length(1,20)}")]
    public async Task<ActionResult<Response<IEnumerable<ProductDto>>>> Search(string value)
    {
        var response = new Response<IEnumerable<UserDto>>();
        try
        {
            IEnumerable<Usuario> users = await service.Search(value);
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}