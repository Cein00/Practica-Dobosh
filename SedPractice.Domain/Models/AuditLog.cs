namespace SedPractice.Domain.Models;

public sealed class AuditLog
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserLogin { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
