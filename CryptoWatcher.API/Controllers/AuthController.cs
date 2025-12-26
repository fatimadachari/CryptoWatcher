using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CryptoWatcher.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RegisterUserUseCase registerUseCase,
        LoginUseCase loginUseCase,
        ILogger<AuthController> logger)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _registerUseCase.ExecuteAsync(
                request.Email,
                request.Password,
                request.Name,
                cancellationToken);

            _logger.LogInformation("✅ Usuário registrado: {Email}", request.Email);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("❌ Validação falhou: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("❌ Email duplicado: {Email}", request.Email);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(
                request.Email,
                request.Password,
                cancellationToken);

            _logger.LogInformation("✅ Login realizado: {Email}", request.Email);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("❌ Login falhou: {Email}", request.Email);
            return Unauthorized(new { error = ex.Message });
        }
    }
}