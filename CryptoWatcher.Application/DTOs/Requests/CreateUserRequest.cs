namespace CryptoWatcher.Application.DTOs.Requests;

public record CreateUserRequest(
    string Email,
    string Name
);