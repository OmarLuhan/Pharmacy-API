using System.Net;
using AutoMapper;
using Farma_api.Dto.Reports;
using Farma_api.Dto.Specifications;
using Farma_api.Helpers;
using Farma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Statics.Admin, AuthenticationSchemes = Statics.AuthSchema)]
public class ReportController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService, IMapper mapper)
    {
        _reportService = reportService;
        _mapper = mapper;
    }

    [HttpGet("sales")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Response<SalesReportDto>>> GetSalesReport([FromQuery] SpecParams specParams,
        [FromQuery] DateFilter dateFilter)
    {
        var response = new Response<IEnumerable<SalesReportDto>>();
        try
        {
            var salesReport = await _reportService.SalesReport(specParams, dateFilter);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<SalesReportDto>>(salesReport);
            response.TotalPages = salesReport.MetaData.TotalPages;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(ex.ToString());
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("sales/download")]
    public async Task<ActionResult<BasicResponse<SalesReportDto>>> DownloadSalesReport(
        [FromQuery] DateFilter dateFilter)
    {
        var response = new BasicResponse<IEnumerable<SalesReportDto>>();
        try
        {
            var salesReport = await _reportService.DownloadSalesReport(dateFilter);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<SalesReportDto>>(salesReport);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("entries/download")]
    public async Task<ActionResult<BasicResponse<GrnReportDto>>> DownloadEntriesReport(
        [FromQuery] DateFilter dateFilter)
    {
        var response = new BasicResponse<IEnumerable<GrnReportDto>>();
        try
        {
            var entriesReport = await _reportService.DownloadEntriesReport(dateFilter);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<GrnReportDto>>(entriesReport);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.ErrorMessage.Add(ex.Message);
            response.IsSuccess = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("grn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Response<GrnReportDto>>> GetGrnReport([FromQuery] SpecParams specParams,
        [FromQuery] DateFilter dateFilter)
    {
        var response = new Response<IEnumerable<GrnReportDto>>();
        try
        {
            var grnReport = await _reportService.EntriesReport(specParams, dateFilter);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<GrnReportDto>>(grnReport);
            response.TotalPages = grnReport.MetaData.TotalPages;
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