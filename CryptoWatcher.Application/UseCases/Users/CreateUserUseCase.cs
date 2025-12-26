using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(string email, string name, CancellationToken cancellationToken = default)
    {
        // Verificar se email já existe
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Email {email} já está cadastrado");
        }

        // ⬅️ POR ENQUANTO: usar hash temporário (vamos mudar isso depois com o RegisterUseCase)
        var tempPasswordHash = BCrypt.Net.BCrypt.HashPassword("temp123");

        // Criar novo usuário
        var user = new User(email, name, tempPasswordHash);

        // Salvar no banco
        await _userRepository.CreateAsync(user, cancellationToken);

        return user;
    }
}