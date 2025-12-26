using CryptoWatcher.Application.DTOs.Responses;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;

namespace CryptoWatcher.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginUseCase(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> ExecuteAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Buscar usuário
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        // Verificar senha
        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValidPassword)
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        // Gerar token JWT
        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.Name, token);
    }
}