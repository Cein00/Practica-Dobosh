namespace SedPractice.Domain.Models;

public sealed class ApprovalTask
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string ApproverLogin { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public string? Resolution { get; set; }
    public string? Comment { get; set; }
}
