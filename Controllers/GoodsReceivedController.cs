using System.Net;
using AutoMapper;
using Farma_api.Dto.Grn;
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
[Authorize(AuthenticationSchemes = Statics.AuthSchema)]
public class GoodsReceivedController(IGoodsReceivedService service, IMapper mapper) : ControllerBase
{
    [HttpGet("{grnNumber:length(6)}", Name = "GrnDetail")]
    public async Task<ActionResult<BasicResponse<GrnDto>>> SaleDetail(string grnNumber)
    {
        var response = new BasicResponse<GrnDto>();
        try
        {
            var entry = await service.Detail(grnNumber);
            if (entry == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("No se encontró el resgisto");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<GrnDto>(entry);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("GrnHistory")]
    public async Task<ActionResult<Response<IEnumerable<GrnHistoryDto>>>> GrnHistory([FromQuery] SpecParams specParams,
        [FromQuery] SpecFilters specFilters)
    {
        var response = new Response<IEnumerable<GrnHistoryDto>>();
        try
        {
            PageList<Entrada> entries = await service.GetGoodsReceived(specParams, specFilters);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<GrnHistoryDto>>(entries);
            response.TotalPages = entries.MetaData.TotalPages;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }
    [HttpGet("Search")]
    public async Task<ActionResult<BasicResponse<IEnumerable<GrnHistoryDto>>>> Search([FromQuery]string value)
    {
        var response = new BasicResponse<IEnumerable<GrnHistoryDto>>();
        if (string.IsNullOrEmpty(value))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Valor de búsqueda no válido");
            return BadRequest(response);
        }

        try
        {
            var grn = await service.Search(value);
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<GrnHistoryDto>>(grn);
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
    public async Task<ActionResult<GrnDto>> Create(GrnCreateDto dto)
    {
        var response = new BasicResponse<GrnDto>();
        var validator = new GrnValidator();
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            var created = await service.Register(mapper.Map<Entrada>(dto));
            if (created == null)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage.Add("No se pudo completar el registro");
                return StatusCode(500, response);
            }

            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<GrnDto>(created);
            return CreatedAtRoute("GrnDetail", new { grnNumber = created.Correlativo }, response);
        }
        catch (Exception e)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(e.Message);
            return StatusCode(500, response);
        }
    }

    [HttpPut("Reverse/{id:int}")]
    public async Task<ActionResult<EmptyResponse>> ReverseGrn(int id)
    {
        var response = new EmptyResponse();

        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id de registro inválido");
            return BadRequest(response);
        }

        try
        {
            if (!await service.IsReversible(id))
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Este registro ya no puede ser revertido");
                return BadRequest(response);
            }

            var reversed = await service.ReverseGrn(id);
            if (!reversed)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage.Add("No se pudo cancelar la venta");
                return StatusCode(500, response);
            }

            response.Status = HttpStatusCode.OK;
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
}