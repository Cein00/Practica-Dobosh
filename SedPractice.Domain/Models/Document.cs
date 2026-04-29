using SedPractice.Domain.Enums;

namespace SedPractice.Domain.Models;

public sealed class Document
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string AuthorLogin { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DocumentStatus Status { get; set; }
    public string CurrentVersion { get; set; } = "1.0";
    public string Content { get; set; } = string.Empty;
}
