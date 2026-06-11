using Microsoft.AspNetCore.Mvc;
using TradingBot.Core.Models;
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
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok(await _signalService.GetRecommendationsAsync(cancellationToken));
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetBySymbol(string symbol, CancellationToken cancellationToken)
    {
        return Ok(await _signalService.SynthesizeAsync(symbol, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSignalRequest request, CancellationToken cancellationToken)
    {
        var signal = await _signalService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetBySymbol), new { symbol = signal.Symbol }, signal);
    }
}
