using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.DTOs.Responses;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.UseCases.Alerts;

public class CreateAlertUseCase
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUserRepository _userRepository;

    public CreateAlertUseCase(
        IAlertRepository alertRepository,
        IUserRepository userRepository)
    {
        _alertRepository = alertRepository;
        _userRepository = userRepository;
    }

    public async Task<AlertResponse> ExecuteAsync(
        CreateAlertRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validar se usuário existe
        var userExists = await _userRepository.ExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
            throw new InvalidOperationException($"Usuário {request.UserId} não encontrado");

        // Criar entidade
        var alert = new CryptoAlert(
            request.UserId,
            request.CryptoSymbol,
            request.TargetPrice,
            request.Condition
        );

        // Persistir
        var createdAlert = await _alertRepository.CreateAsync(alert, cancellationToken);

        // Retornar DTO
        return new AlertResponse(
            createdAlert.Id,
            createdAlert.UserId,
            createdAlert.CryptoSymbol,
            createdAlert.TargetPrice,
            createdAlert.Condition,
            createdAlert.Status,
            createdAlert.CreatedAt,
            createdAlert.TriggeredAt
        );
    }
}