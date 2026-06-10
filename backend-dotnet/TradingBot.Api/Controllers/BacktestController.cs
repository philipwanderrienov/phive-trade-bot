using Microsoft.AspNetCore.Mvc;
using TradingBot.Core.Models;
using TradingBot.Engine.Services;

namespace TradingBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BacktestController : ControllerBase
{
    private readonly BacktestingEngine _backtestingEngine;

    public BacktestController(BacktestingEngine backtestingEngine)
    {
        _backtestingEngine = backtestingEngine;
    }

    [HttpPost]
    public IActionResult Run([FromBody] BacktestRequest request) => Ok(_backtestingEngine.Run(request));
}
