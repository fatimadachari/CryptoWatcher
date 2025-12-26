using System.Security.Claims;
using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.UseCases.Alerts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoWatcher.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ⬅️ PROTEGE TODOS OS ENDPOINTS
public class AlertsController : ControllerBase
{
    private readonly CreateAlertUseCase _createAlertUseCase;
    private readonly GetActiveAlertsUseCase _getActiveAlertsUseCase;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(
        CreateAlertUseCase createAlertUseCase,
        GetActiveAlertsUseCase getActiveAlertsUseCase,
        ILogger<AlertsController> logger)
    {
        _createAlertUseCase = createAlertUseCase;
        _getActiveAlertsUseCase = getActiveAlertsUseCase;
        _logger = logger;
    }

    // ⬅️ MÉTODO HELPER PARA PEGAR USERID DO TOKEN
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Token inválido");
        }
        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(
        [FromBody] CreateAlertRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ⬅️ PEGA USERID DO TOKEN (não da request!)
            var userId = GetUserIdFromToken();

            var alert = await _createAlertUseCase.ExecuteAsync(
                userId, // ⬅️ USA O USERID DO TOKEN
                request.CryptoSymbol,
                request.TargetPrice,
                request.Condition,
                cancellationToken);

            _logger.LogInformation(
                "✅ Alerta criado: User {UserId}, {Symbol} {Condition} ${Price}",
                userId, request.CryptoSymbol, request.Condition, request.TargetPrice
            );

            return CreatedAtAction(nameof(GetActiveAlerts), new { id = alert.Id }, alert);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("❌ Validação falhou: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("❌ Operação inválida: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAlerts(CancellationToken cancellationToken)
    {
        try
        {
            // ⬅️ PEGA USERID DO TOKEN
            var userId = GetUserIdFromToken();

            // ⬅️ BUSCA APENAS OS ALERTAS DO USUÁRIO LOGADO
            var alerts = await _getActiveAlertsUseCase.ExecuteAsync(userId, cancellationToken);

            _logger.LogInformation("✅ Alertas ativos retornados: User {UserId}, Count: {Count}", userId, alerts.Count());
            return Ok(alerts);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}