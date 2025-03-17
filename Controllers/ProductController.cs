using System.Net;
using AutoMapper;
using Farma_api.Dto.Grn;
using Farma_api.Dto.Product;
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
public class ProductController(IProductService service, IMapper mapper, IBatchService batchService)
    : ControllerBase
{
    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    public async Task<ActionResult<Response<IEnumerable<ProductDto>>>> Paginated([FromQuery] SpecParams specParams)
    {
        var response = new Response<IEnumerable<ProductDto>>();
        try
        {
            PageList<Producto> list = await service.List(specParams);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<ProductDto>>(list);
            response.TotalPages = list.MetaData.TotalPages;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [Authorize(AuthenticationSchemes = Statics.AuthSchema)]
    [HttpGet("product_for_sale/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<ProductSaleDto>>> ProductForSale(string code)
    {
        var response = new BasicResponse<ProductSaleDto>();
        try
        {
            var prod = await service.ProductForSale(code);
            if (prod == null)
            {
                response.ErrorMessage.Add("Producto no encontrado");
                response.Status = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<ProductSaleDto>(prod);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [Authorize(AuthenticationSchemes = Statics.AuthSchema)]
    [HttpGet("product_for_sale_name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<IEnumerable<BasicResponse<ProductSaleDto>>>>>
        ProductForSaleName(string name)
    {
        var response = new BasicResponse<IEnumerable<ProductSaleDto>>();
        try
        {
            var prod = await service.ProductForSaleName(name);
            if (prod == null)
            {
                response.ErrorMessage.Add("Producto no encontrado");
                response.Status = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<ProductSaleDto>>(prod);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpGet("product_for_grn_name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    public async Task<ActionResult<BasicResponse<IEnumerable<BasicResponse<ProductGrnDto>>>>>
        ProductForGrnName(string name)
    {
        var response = new BasicResponse<IEnumerable<ProductGrnDto>>();
        try
        {
            var prod = await service.ProductByName(name);
            if (prod == null)
            {
                response.ErrorMessage.Add("Producto no encontrado");
                response.Status = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<IEnumerable<ProductGrnDto>>(prod);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    [HttpGet("product_for_grn/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<ProductGrnDto>>> ProductForGrn(string code)
    {
        var response = new BasicResponse<ProductGrnDto>();
        try
        {
            var prod = await service.GetByCode(code);
            if (prod == null)
            {
                response.ErrorMessage.Add("Producto no encontrado");
                response.Status = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<ProductGrnDto>(prod);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    [HttpGet("{code}", Name = "ProductByCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<ProductWithBatchDto>>> GetByCode(string code)
    {
        var response = new BasicResponse<ProductWithBatchDto>();
        if (string.IsNullOrEmpty(code))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("codigo no válido");
            return BadRequest(response);
        }

        try
        {
            var product = await service.GetByCode(code);
            if (product == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessage.Add("Producto no encontrado");
                return NotFound(response);
            }

            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = mapper.Map<ProductWithBatchDto>(product);
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
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasicResponse<ProductDto>>> Create([FromBody] ProductCreateDto dto)
    {
        var response = new BasicResponse<ProductDto>();
        var validator = new ProductCreateValidator();
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            response.Status = HttpStatusCode.BadRequest;
            foreach (var error in result.Errors) response.ErrorMessage.Add(error.ErrorMessage);
            return BadRequest(response);
        }

        try
        {
            if (!await service.IsUnique(dto.CodigoEan13))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("El código de producto ya existe");
                return Conflict(response);
            }

            var product = mapper.Map<Producto>(dto);
            var created = await service.Create(product);
            if (created == null)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("No se pudo crear el producto");
                return BadRequest(response);
            }

            response.Status = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Data = mapper.Map<ProductDto>(created);
            return CreatedAtRoute("ProductByCode", new { code = created.CodigoEan13 }, response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var response = new EmptyResponse();
        var validator = new ProductUpdateValidator();

        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("codigo no válido");
            return BadRequest(response);
        }

        if (id != dto.Id)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("El código no coincide con el producto");
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
            var actual = await service.GetById(id, false);
            if (actual == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Producto no encontrado");
                return NotFound(response);
            }

            if (!await service.IsUnique(dto.CodigoEan13, dto.Id))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("El código ya pertenece a otro producto");
                return Conflict(response);
            }

            var product = mapper.Map<Producto>(dto);
            product.CodigoEan13 = actual.CodigoEan13;
            product.FechaRegistro = actual.FechaRegistro;
            await service.Update(product);
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
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = new EmptyResponse();
        if (id <= 0)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("codigo no válido");
            return BadRequest(response);
        }

        try
        {
            var product = await service.GetById(id);
            if (product == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.ErrorMessage.Add("Producto no encontrado");
                return NotFound(response);
            }

            if (await batchService.HasBatches(id))
            {
                response.Status = HttpStatusCode.Conflict;
                response.ErrorMessage.Add("No se puede eliminar el producto porque tiene lotes asociados");
                return Conflict(response);
            }

            await service.Delete(product);
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
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    public async Task<ActionResult<BasicResponse<IEnumerable<ProductDto>>>> Search(string value)
    {
        var response = new BasicResponse<IEnumerable<ProductDto>>();
        if (string.IsNullOrEmpty(value))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Valor de búsqueda no válido");
            return BadRequest(response);
        }

        try
        {
            var products = await service.Search(value);
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.Status = HttpStatusCode.InternalServerError;
            return StatusCode(500, response);
        }
    }

    [HttpGet("Download")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    [Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
    public async Task<ActionResult<ProductDto>> Download()
    {
        var response = new BasicResponse<IEnumerable<ProductDto>>();
        try
        {
            var products = await service.List();
            response.IsSuccess = true;
            response.Status = HttpStatusCode.OK;
            response.Data = mapper.Map<IEnumerable<ProductDto>>(products);
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