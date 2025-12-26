using CryptoWatcher.Application.DTOs.Responses;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.UseCases.Auth;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> ExecuteAsync(
        string email,
        string password,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validar senha
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        if (password.Length < 6)
            throw new ArgumentException("Senha deve ter no mínimo 6 caracteres", nameof(password));

        // Verificar se email já existe
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
            throw new InvalidOperationException($"Email {email} já está cadastrado");

        // Hash da senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Criar usuário
        var user = new User(email, name, passwordHash);

        // Salvar no banco
        await _userRepository.CreateAsync(user, cancellationToken);

        // Gerar token JWT
        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.Name, token);
    }
}