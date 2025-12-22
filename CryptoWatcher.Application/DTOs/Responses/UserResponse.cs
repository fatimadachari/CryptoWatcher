namespace CryptoWatcher.Application.DTOs.Responses;

public record UserResponse(
    int Id,
    string Email,
    string Name,
    DateTime CreatedAt
);