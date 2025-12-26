using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.UseCases.Users;
using Microsoft.AspNetCore.Mvc;

namespace CryptoWatcher.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;

    public UsersController(CreateUserUseCase createUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _createUserUseCase.ExecuteAsync(request.Email, request.Name, cancellationToken);
            return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
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
}