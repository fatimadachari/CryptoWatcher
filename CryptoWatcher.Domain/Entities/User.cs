using CryptoWatcher.Domain.Common;

namespace CryptoWatcher.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; } // ⬅️ NOVO CAMPO
    public ICollection<CryptoAlert> Alerts { get; private set; }

    // EF Core
    private User()
    {
        Email = null!;
        Name = null!;
        PasswordHash = null!; // ⬅️ NOVO
        Alerts = new List<CryptoAlert>();
    }

    public User(string email, string name, string passwordHash) // ⬅️ ADICIONAR PARÂMETRO
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));

        if (!email.Contains("@"))
            throw new ArgumentException("Email inválido", nameof(email));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio", nameof(name));

        if (name.Length < 3)
            throw new ArgumentException("Nome deve ter no mínimo 3 caracteres", nameof(name));

        if (string.IsNullOrWhiteSpace(passwordHash)) // ⬅️ NOVO
            throw new ArgumentException("Hash da senha não pode ser vazio", nameof(passwordHash));

        Email = email;
        Name = name;
        PasswordHash = passwordHash; // ⬅️ NOVO
        Alerts = new List<CryptoAlert>();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio", nameof(name));

        if (name.Length < 3)
            throw new ArgumentException("Nome deve ter no mínimo 3 caracteres", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    // ⬅️ NOVO MÉTODO
    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash da senha não pode ser vazio", nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAlert(CryptoAlert alert)
    {
        Alerts.Add(alert);
    }
}