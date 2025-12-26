using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;

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

    // ⬅️ AGORA RECEBE USERID COMO PARÂMETRO
    public async Task<CryptoAlert> ExecuteAsync(
        int userId,
        string cryptoSymbol,
        decimal targetPrice,
        PriceCondition condition,
        CancellationToken cancellationToken = default)
    {
        // Verificar se usuário existe
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"Usuário {userId} não encontrado");
        }

        // Criar alerta
        var alert = new CryptoAlert(userId, cryptoSymbol, targetPrice, condition);

        // Salvar no banco
        await _alertRepository.CreateAsync(alert, cancellationToken);

        return alert;
    }
}