using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.UseCases.Alerts;
using Microsoft.AspNetCore.Mvc;

namespace CryptoWatcher.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly CreateAlertUseCase _createAlertUseCase;
    private readonly GetActiveAlertsUseCase _getActiveAlertsUseCase;

    public AlertsController(
        CreateAlertUseCase createAlertUseCase,
        GetActiveAlertsUseCase getActiveAlertsUseCase)
    {
        _createAlertUseCase = createAlertUseCase;
        _getActiveAlertsUseCase = getActiveAlertsUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(
        [FromBody] CreateAlertRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _createAlertUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateAlert), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAlerts(CancellationToken cancellationToken)
    {
        var alerts = await _getActiveAlertsUseCase.ExecuteAsync(cancellationToken);
        return Ok(alerts);
    }
}