using SedPractice.DAL.Repositories;
using SedPractice.Domain.Models;

namespace SedPractice.BLL.Services;

public sealed class AuditService
{
    private readonly AuditLogRepository _auditLogRepository;

    public AuditService(AuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public void Log(string userLogin, string action, string entityName, string entityKey, string description)
    {
        _auditLogRepository.Add(new AuditLog
        {
            CreatedAt = DateTime.UtcNow,
            UserLogin = userLogin,
            Action = action,
            EntityName = entityName,
            EntityKey = entityKey,
            Description = description
        });
    }

    public IReadOnlyList<AuditLog> GetRecent() => _auditLogRepository.GetRecent();
}
