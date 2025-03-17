using System.Net;
using AutoMapper;
using Farma_api.Dto.Batch;
using Farma_api.Dto.Specifications;
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
public class BatchController(IBatchService service, IMapper mapper) : ControllerBase
{
    [HttpGet("batches_x_product/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Response<IEnumerable<BatchDto>>>> ProductsForBatch(int id,
        [FromQuery] SpecParams specParams)
    {
        var response = new Response<IEnumerable<BatchDto>>();
        if (id <= 0)
        {
            response.ErrorMessage.Add("El id del producto no puede ser menor o igual a 0");
            response.Status = HttpStatusCode.BadRequest;
            return BadRequest(response);
        }

        try
        {
            PageList<Lote> batches = await service.BatchesProduct(id, specParams);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<BatchDto>>(batches);
            response.TotalPages = batches.MetaData.TotalPages;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id:int}", Name = "BatchById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<BatchDto>>> GetById(int id)
    {
        var response = new BasicResponse<BatchDto>();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("codigo no válido");
            return BadRequest(response);
        }

        try
        {
            var batch = await service.BatchById(id);
            if (batch == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessage.Add("Lote no encontrado");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<BatchDto>(batch);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasicResponse<BatchDto>>> Create([FromBody] BatchCreateDto dto)
    {
        var response = new BasicResponse<BatchDto>();
        var validator = new BatchCreateValidator();
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            if (!await service.IsUniqueBatch(dto.ProductoId, dto.NumeroLote.Trim()))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("El lote ya en este producto");
                return Conflict(response);
            }

            var batch = mapper.Map<Lote>(dto);
            var created = await service.Create(batch);
            if (created == null)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage.Add("No se pudo crear el lote");
                return StatusCode(500, response);
            }

            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<BatchDto>(created);
            return CreatedAtRoute("BatchById", new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, BatchUpdateDto dto)
    {
        var response = new EmptyResponse();
        var validator = new BatchUpdateValidator();

        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id no válido");
            return BadRequest(response);
        }

        if (id != dto.Id)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("El id no coincide con el lote");
            return BadRequest(response);
        }

        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            var current = await service.BatchById(id, false);
            if (current == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Lote no encontrado");
                return NotFound(response);
            }

            var lot = mapper.Map<Lote>(dto);
            lot.ProductoId = current.ProductoId;
            await service.Update(lot);
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = new EmptyResponse();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("id no válido");
            return BadRequest(response);
        }

        try
        {
            var batch = await service.BatchById(id);
            if (batch == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Lote no encontrado");
                return NotFound(response);
            }

            if (batch.StockLote > 0)
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("No se puede eliminar el lote porque tiene stock");
                return Conflict(response);
            }

            await service.Delete(batch);
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
}