using SedPractice.DAL.Repositories;
using SedPractice.Domain.Enums;
using SedPractice.Domain.Models;

namespace SedPractice.BLL.Services;

public sealed class DocumentService
{
    private readonly DocumentRepository _documentRepository;
    private readonly ApprovalTaskRepository _taskRepository;
    private readonly AuditService _auditService;

    public DocumentService(
        DocumentRepository documentRepository,
        ApprovalTaskRepository taskRepository,
        AuditService auditService)
    {
        _documentRepository = documentRepository;
        _taskRepository = taskRepository;
        _auditService = auditService;
    }

    public int CreateDocument(string authorLogin, string title, string documentType, string content, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(authorLogin))
        {
            throw new ArgumentException("Автор обязателен.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Тема документа обязательна.");
        }

        var number = $"DOC-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var document = new Document
        {
            RegistrationNumber = number,
            Title = title.Trim(),
            DocumentType = documentType.Trim(),
            AuthorLogin = authorLogin.Trim(),
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate,
            Status = DocumentStatus.Draft,
            CurrentVersion = "1.0",
            Content = content.Trim()
        };

        var id = _documentRepository.Create(document);
        _auditService.Log(authorLogin, "CREATE", "Documents", id.ToString(), $"Создан документ {number}.");
        return id;
    }

    public void SubmitForApproval(int documentId, string userLogin, string approverLogin, DateTime deadline)
    {
        var document = _documentRepository.GetById(documentId)
            ?? throw new InvalidOperationException("Документ не найден.");

        if (document.Status != DocumentStatus.Draft)
        {
            throw new InvalidOperationException("На согласование можно отправить только черновик.");
        }

        _documentRepository.UpdateStatus(documentId, DocumentStatus.Submitted, "1.1");
        _taskRepository.Create(new ApprovalTask
        {
            DocumentId = documentId,
            ApproverLogin = approverLogin,
            CreatedAt = DateTime.UtcNow,
            Deadline = deadline,
            IsCompleted = false
        });

        _auditService.Log(userLogin, "SUBMIT", "Documents", documentId.ToString(), $"Документ {document.RegistrationNumber} отправлен на согласование.");
    }

    public void Approve(int documentId, string managerLogin, string? comment)
    {
        var task = _taskRepository.GetOpenTaskByDocumentId(documentId)
            ?? throw new InvalidOperationException("Открытая задача согласования не найдена.");

        _taskRepository.Complete(task.Id, "APPROVED", comment);
        _documentRepository.UpdateStatus(documentId, DocumentStatus.Approved, "1.2");
        _auditService.Log(managerLogin, "APPROVE", "Documents", documentId.ToString(), "Документ согласован.");
    }

    public void Reject(int documentId, string managerLogin, string? comment)
    {
        var task = _taskRepository.GetOpenTaskByDocumentId(documentId)
            ?? throw new InvalidOperationException("Открытая задача согласования не найдена.");

        _taskRepository.Complete(task.Id, "REJECTED", comment);
        _documentRepository.UpdateStatus(documentId, DocumentStatus.Rejected, "1.2");
        _auditService.Log(managerLogin, "REJECT", "Documents", documentId.ToString(), "Документ отклонен.");
    }

    public IReadOnlyList<Document> GetAll() => _documentRepository.GetAll();

    public Document? GetById(int id) => _documentRepository.GetById(id);

    public IReadOnlyList<Document> Search(string term) => _documentRepository.Search(term);

    public IReadOnlyList<ApprovalTask> GetOverdueTasks() => _taskRepository.GetOverdueTasks();
}
