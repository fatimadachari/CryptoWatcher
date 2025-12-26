using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}