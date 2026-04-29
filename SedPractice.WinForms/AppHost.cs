using SedPractice.BLL.Services;
using SedPractice.DAL.Infrastructure;
using SedPractice.DAL.Repositories;

namespace SedPractice.WinForms;

public sealed class AppHost
{
    public AuthService AuthService { get; }
    public DocumentService DocumentService { get; }
    public AuditService AuditService { get; }

    public AppHost(string databasePath)
    {
        var factory = new SqliteConnectionFactory(databasePath);
        var initializer = new DatabaseInitializer(factory);
        initializer.Initialize();

        var userRepository = new UserRepository(factory);
        var documentRepository = new DocumentRepository(factory);
        var approvalTaskRepository = new ApprovalTaskRepository(factory);
        var auditLogRepository = new AuditLogRepository(factory);

        AuditService = new AuditService(auditLogRepository);
        AuthService = new AuthService(userRepository);
        DocumentService = new DocumentService(documentRepository, approvalTaskRepository, AuditService);
    }
}
