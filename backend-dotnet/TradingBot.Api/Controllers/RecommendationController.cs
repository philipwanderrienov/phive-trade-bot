using Microsoft.AspNetCore.Mvc;
using TradingBot.Engine.Services;

namespace TradingBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly SignalService _signalService;

    public RecommendationController(SignalService signalService)
    {
        _signalService = signalService;
    }

    [HttpGet]
    public IActionResult Get() => Ok(new[]
    {
        _signalService.Synthesize("AAPL"),
        _signalService.Synthesize("TSLA"),
        _signalService.Synthesize("BTC-USD")
    });
}
