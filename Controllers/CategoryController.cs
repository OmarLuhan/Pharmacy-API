using System.Net;
using AutoMapper;
using Farma_api.Dto.Category;
using Farma_api.Dto.Specifications;
using Farma_api.Helpers;
using Farma_api.Models;
using Farma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
public class CategoryController(ICategoryService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<CategorySelectDto>>> GetAll()
    {
        var response = new BasicResponse<IEnumerable<CategorySelectDto>>();
        try
        {
            var categories = await service.List();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<CategorySelectDto>>(categories);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<CategoryDto>>> Paginated([FromQuery] SpecParams specParams)
    {
        var response = new Response<IEnumerable<CategoryDto>>();
        try
        {
            PageList<Categoria> categories = await service.List(specParams);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<CategoryDto>>(categories);
            response.TotalPages = categories.MetaData.TotalPages;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id:int}", Name = "CategoryById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<CategoryDto>>> GetById(int id)
    {
        var response = new BasicResponse<CategoryDto>();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id no válido");
            return BadRequest(response);
        }

        try
        {
            var category = await service.GetById(id);
            if (category == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Categoria no encontrada");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<CategoryDto>(category);
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
    public async Task<ActionResult<BasicResponse<CategoryDto>>> Create(CategoryCreateDto dto)
    {
        var response = new BasicResponse<CategoryDto>();

        try
        {
            if (!await service.IsUnique(dto.Descripcion))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("La categoria ya existe");
                return Conflict(response);
            }

            var model = await service.Create(mapper.Map<Categoria>(dto));
            if (model == null)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage.Add("No se pudo crear la categoria");
                return StatusCode(500, response);
            }

            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<CategoryDto>(model);
            return CreatedAtRoute("CategoryById", new { id = model.Id }, response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var response = new EmptyResponse();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id no válido");
            return BadRequest(response);
        }

        try
        {
            var category = await service.GetById(id);
            if (category == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Categoria no encontrada");
                return NotFound(response);
            }

            if (!await service.IsUnique(dto.Descripcion, id))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("El nombre de la categoria ya existe");
                return Conflict(response);
            }

            category.Descripcion = dto.Descripcion;
            category.Activo = dto.Activo;
            await service.Update(category);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EmptyResponse>> Delete(int id)
    {
        var response = new EmptyResponse();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id no válido");
            return BadRequest(response);
        }

        try
        {
            var category = await service.GetById(id);
            if (category == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Categoria no encontrada");
                return NotFound(response);
            }

            await service.Delete(category);
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("Search/{value:length(1,15)}")]
    public async Task<ActionResult<BasicResponse<IEnumerable<CategoryDto>>>> Search(string value)
    {
        var response = new BasicResponse<IEnumerable<CategoryDto>>();
        if (string.IsNullOrEmpty(value))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Valor de búsqueda no válido");
            return BadRequest(response);
        }

        try
        {
            IEnumerable<Categoria> categories = await service.Search(value);
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<CategoryDto>>(categories);
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