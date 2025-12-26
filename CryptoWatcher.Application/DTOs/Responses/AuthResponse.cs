namespace CryptoWatcher.Application.DTOs.Responses;

public record AuthResponse(
    int UserId,
    string Email,
    string Name,
    string Token
);