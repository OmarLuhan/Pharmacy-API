using System.Net;
using Farma_api.Dto.Chart;
using Farma_api.Helpers;
using Farma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
public class ChartsController(IChartsService chartsService) : ControllerBase
{
    [HttpGet("BarChart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<BarChart>>> GetBarChart()
    {
        var response = new BasicResponse<BarChart>();
        try
        {
            var barChart = await chartsService.NewSalesChart();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = barChart;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("FrequencyGraph")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<FrequencyGraph>>> GetFrequencyGraph()
    {
        var response = new BasicResponse<FrequencyGraph>();
        try
        {
            var barChart = await chartsService.FrequencyGraph();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = barChart;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("DistributionGraph")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<DistributionGraph>>> GetDistributionGraph()
    {
        var response = new BasicResponse<DistributionGraph>();
        try
        {
            var distChart = await chartsService.DistributionGraph();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = distChart;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("ProductsZero")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<ProductZero>>> GetProductsZero()
    {
        var response = new BasicResponse<ProductZero>();
        try
        {
            var prodZero = await chartsService.ProductsZero();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = prodZero;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("LineChart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BasicResponse<LinesChart>>> GetLineChart()
    {
        var response = new BasicResponse<LinesChart>();
        try
        {
            var lineChart = await chartsService.RevenuesChart();
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = lineChart;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }
}