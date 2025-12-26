namespace CryptoWatcher.Application.DTOs.Requests;

public record RegisterRequest(
    string Email,
    string Password,
    string Name
);