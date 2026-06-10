using Microsoft.AspNetCore.Mvc;
using TradingBot.Engine.Services;

namespace TradingBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportingService _reportingService;

    public ReportController(ReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_reportingService.GetSummary());
}
