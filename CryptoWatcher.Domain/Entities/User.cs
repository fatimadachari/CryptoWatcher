using CryptoWatcher.Domain.Common;

namespace CryptoWatcher.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string Name { get; private set; }

    // Relacionamento: 1 User tem N Alerts
    private readonly List<CryptoAlert> _alerts = new();
    public IReadOnlyCollection<CryptoAlert> Alerts => _alerts.AsReadOnly();

    // Construtor privado para EF Core
    private User() { }

    // Construtor de criação (factory method)
    public User(string email, string name)
    {
        ValidateEmail(email);
        ValidateName(name);

        Email = email;
        Name = name;
    }

    // Métodos de domínio
    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName;
        SetUpdatedAt();
    }

    public void AddAlert(CryptoAlert alert)
    {
        _alerts.Add(alert);
    }

    // Validações privadas
    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Email inválido", nameof(email));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio", nameof(name));

        if (name.Length < 3)
            throw new ArgumentException("Nome deve ter no mínimo 3 caracteres", nameof(name));
    }
}