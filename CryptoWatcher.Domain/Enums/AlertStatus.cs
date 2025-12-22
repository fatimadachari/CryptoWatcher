namespace CryptoWatcher.Domain.Enums;

public enum AlertStatus
{
    Active = 1,      // Alerta ativo, sendo monitorado
    Triggered = 2,   // Alerta disparou (condição atingida)
    Cancelled = 3    // Usuário cancelou
}