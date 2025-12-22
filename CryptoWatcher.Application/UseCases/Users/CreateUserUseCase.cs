using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.DTOs.Responses;
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

    public async Task<UserResponse> ExecuteAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validar se email já existe
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException($"Usuário com email {request.Email} já existe");

        // Criar entidade (validações acontecem no construtor)
        var user = new User(request.Email, request.Name);

        // Persistir
        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        // Retornar DTO
        return new UserResponse(
            createdUser.Id,
            createdUser.Email,
            createdUser.Name,
            createdUser.CreatedAt
        );
    }
}