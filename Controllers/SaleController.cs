using System.Net;
using AutoMapper;
using Farma_api.Dto.Sales;
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
public class SaleController(ISaleService service, IMapper mapper) : ControllerBase
{
    [HttpGet("{saleNumber:length(6)}", Name = "SaleDetail")]
    public async Task<ActionResult<BasicResponse<SaleDto>>> SaleDetail(string saleNumber)
    {
        var response = new BasicResponse<SaleDto>();
        try
        {
            var sale = await service.Detail(saleNumber);
            if (sale == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("No se encontró el resgisto");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<SaleDto>(sale);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("SaleHistory")]
    public async Task<ActionResult<Response<IEnumerable<SaleHistoryDto>>>> SaleHistory(
        [FromQuery] SpecParams specParams, [FromQuery] SpecFilters specFilters)
    {
        var response = new Response<IEnumerable<SaleHistoryDto>>();
        try
        {
            var sales = await service.GetSale(specFilters, specParams);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<SaleHistoryDto>>(sales);
            response.TotalPages = sales.MetaData.TotalPages;
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
    public async Task<ActionResult<BasicResponse<IEnumerable<SaleHistoryDto>>>> Search([FromQuery]string value)
    {
        var response = new BasicResponse<IEnumerable<SaleHistoryDto>>();
        if (string.IsNullOrEmpty(value))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Valor de búsqueda no válido");
            return BadRequest(response);
        }

        try
        {
            var sales = await service.Search(value);
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<SaleHistoryDto>>(sales);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasicResponse<SaleDto>>> RegisterSale(SaleCreateDto dto)
    {
        var response = new BasicResponse<SaleDto>();
        var validator = new SaleCreateValidator();
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        if (!await service.IsStockSufficient(mapper.Map<IEnumerable<DetalleVenta>>(dto.DetalleVenta)))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Stock insuficiente");
            return BadRequest(response);
        }

        try
        {
            var created = await service.Create(mapper.Map<Venta>(dto));
            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<SaleDto>(created);
            return CreatedAtRoute("SaleDetail", new { saleNumber = created.Correlativo }, response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("Cancel/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmptyResponse>> CancelSale(int id)
    {
        var response = new EmptyResponse();

        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Id de venta inválido");
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

            var reversed = await service.ReverseSale(id);
            if (!reversed)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage.Add("No se pudo cancelar la venta");
                return StatusCode(500, response);
            }

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
}