using SedPractice.Domain.Enums;
using SedPractice.Domain.Models;
using SedPractice.WinForms.Helpers;

namespace SedPractice.WinForms.Forms;

public sealed class MainForm : Form
{
    private readonly AppHost _host;
    private readonly User _currentUser;

    private readonly Label _userLabel = new() { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
    private readonly TextBox _searchTextBox = new() { Width = 220, PlaceholderText = "Поиск по номеру, теме, автору" };
    private readonly DataGridView _documentsGrid = new();
    private readonly DataGridView _overdueGrid = new();
    private readonly DataGridView _auditGrid = new();
    private readonly RichTextBox _documentPreview = new() { Dock = DockStyle.Fill, ReadOnly = true };

    public MainForm(AppHost host, User currentUser)
    {
        _host = host;
        _currentUser = currentUser;

        Text = "СЭД для малого предприятия - Windows Forms";
        WindowState = FormWindowState.Maximized;

        _userLabel.Text = $"Пользователь: {_currentUser.FullName} | Роль: {_currentUser.Role} | Подразделение: {_currentUser.Department}";

        UiHelpers.ApplyGridStyle(_documentsGrid);
        UiHelpers.ApplyGridStyle(_overdueGrid);
        UiHelpers.ApplyGridStyle(_auditGrid);

        _documentsGrid.SelectionChanged += (_, _) => ShowSelectedDocument();
        _documentsGrid.CellDoubleClick += (_, _) => ShowSelectedDocument();

        Controls.Add(BuildLayout());
        Load += (_, _) => RefreshAllData();
    }

    private Control BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(_userLabel, 0, 0);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildDocumentsPage());
        tabs.TabPages.Add(BuildOverduePage());
        tabs.TabPages.Add(BuildAuditPage());
        root.Controls.Add(tabs, 0, 1);
        return root;
    }

    private TabPage BuildDocumentsPage()
    {
        var page = new TabPage("Документы");

        var createButton = new Button { Text = "Создать" };
        createButton.Click += (_, _) => CreateDocument();

        var submitButton = new Button { Text = "Отправить" };
        submitButton.Click += (_, _) => SubmitSelectedDocument();

        var approveButton = new Button { Text = "Согласовать" };
        approveButton.Click += (_, _) => ApproveSelectedDocument();

        var rejectButton = new Button { Text = "Отклонить" };
        rejectButton.Click += (_, _) => RejectSelectedDocument();

        var refreshButton = new Button { Text = "Обновить" };
        refreshButton.Click += (_, _) => RefreshDocuments();

        var searchButton = new Button { Text = "Найти" };
        searchButton.Click += (_, _) => SearchDocuments();

        createButton.Enabled = _currentUser.Role is UserRole.Employee or UserRole.Administrator;
        submitButton.Enabled = _currentUser.Role is UserRole.Employee or UserRole.Administrator;
        approveButton.Enabled = _currentUser.Role is UserRole.Manager or UserRole.Administrator;
        rejectButton.Enabled = _currentUser.Role is UserRole.Manager or UserRole.Administrator;

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8) };
        toolbar.Controls.Add(createButton);
        toolbar.Controls.Add(submitButton);
        toolbar.Controls.Add(approveButton);
        toolbar.Controls.Add(rejectButton);
        toolbar.Controls.Add(_searchTextBox);
        toolbar.Controls.Add(searchButton);
        toolbar.Controls.Add(refreshButton);

        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 330 };
        split.Panel1.Controls.Add(_documentsGrid);
        split.Panel2.Controls.Add(_documentPreview);

        page.Controls.Add(split);
        page.Controls.Add(toolbar);
        return page;
    }

    private TabPage BuildOverduePage()
    {
        var page = new TabPage("Просроченные задачи");
        var refreshButton = new Button { Text = "Обновить список", Dock = DockStyle.Top, Height = 40 };
        refreshButton.Click += (_, _) => RefreshOverdue();
        page.Controls.Add(_overdueGrid);
        page.Controls.Add(refreshButton);
        return page;
    }

    private TabPage BuildAuditPage()
    {
        var page = new TabPage("Журнал действий");
        var refreshButton = new Button { Text = "Обновить журнал", Dock = DockStyle.Top, Height = 40 };
        refreshButton.Click += (_, _) => RefreshAudit();
        page.Controls.Add(_auditGrid);
        page.Controls.Add(refreshButton);
        return page;
    }

    private void RefreshAllData()
    {
        RefreshDocuments();
        RefreshOverdue();
        RefreshAudit();
    }

    private void RefreshDocuments()
    {
        var rows = _host.DocumentService.GetAll()
            .Select(d => new
            {
                d.Id,
                d.RegistrationNumber,
                d.Title,
                d.DocumentType,
                Author = d.AuthorLogin,
                CreatedAt = d.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                DueDate = d.DueDate?.ToString("dd.MM.yyyy") ?? "-",
                Status = d.Status.ToString(),
                Version = d.CurrentVersion
            })
            .ToList();

        _documentsGrid.DataSource = rows;
        if (_documentsGrid.Columns["Id"] is not null)
        {
            _documentsGrid.Columns["Id"].Width = 60;
        }
        ShowSelectedDocument();
    }

    private void SearchDocuments()
    {
        var term = _searchTextBox.Text.Trim();
        var documents = string.IsNullOrWhiteSpace(term)
            ? _host.DocumentService.GetAll()
            : _host.DocumentService.Search(term);

        _documentsGrid.DataSource = documents.Select(d => new
        {
            d.Id,
            d.RegistrationNumber,
            d.Title,
            d.DocumentType,
            Author = d.AuthorLogin,
            CreatedAt = d.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
            DueDate = d.DueDate?.ToString("dd.MM.yyyy") ?? "-",
            Status = d.Status.ToString(),
            Version = d.CurrentVersion
        }).ToList();

        ShowSelectedDocument();
    }

    private void RefreshOverdue()
    {
        _overdueGrid.DataSource = _host.DocumentService.GetOverdueTasks()
            .Select(t => new
            {
                t.Id,
                t.DocumentId,
                t.ApproverLogin,
                CreatedAt = t.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                Deadline = t.Deadline.ToString("dd.MM.yyyy"),
                Status = t.IsCompleted ? "Закрыта" : "Открыта",
                Resolution = t.Resolution ?? "-"
            })
            .ToList();
    }

    private void RefreshAudit()
    {
        _auditGrid.DataSource = _host.AuditService.GetRecent()
            .Select(a => new
            {
                a.Id,
                CreatedAt = a.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                a.UserLogin,
                a.Action,
                a.EntityName,
                a.EntityKey,
                a.Description
            })
            .ToList();
    }

    private void CreateDocument()
    {
        using var dialog = new CreateDocumentForm();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var id = _host.DocumentService.CreateDocument(_currentUser.Login, dialog.DocumentTitle, dialog.DocumentType, dialog.DocumentContent, dialog.DueDate);
            UiHelpers.ShowInfo($"Документ создан. Идентификатор: {id}");
            RefreshAllData();
        }
        catch (Exception ex)
        {
            UiHelpers.ShowError(ex.Message);
        }
    }

    private void SubmitSelectedDocument()
    {
        var documentId = GetSelectedDocumentId();
        if (documentId is null)
        {
            UiHelpers.ShowError("Сначала выберите документ.");
            return;
        }

        using var dialog = new SubmitForApprovalForm();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _host.DocumentService.SubmitForApproval(documentId.Value, _currentUser.Login, dialog.ApproverLogin, dialog.Deadline);
            UiHelpers.ShowInfo("Документ отправлен на согласование.");
            RefreshAllData();
        }
        catch (Exception ex)
        {
            UiHelpers.ShowError(ex.Message);
        }
    }

    private void ApproveSelectedDocument()
    {
        ChangeDecision(isApprove: true);
    }

    private void RejectSelectedDocument()
    {
        ChangeDecision(isApprove: false);
    }

    private void ChangeDecision(bool isApprove)
    {
        var documentId = GetSelectedDocumentId();
        if (documentId is null)
        {
            UiHelpers.ShowError("Сначала выберите документ.");
            return;
        }

        using var dialog = new DecisionForm(isApprove ? "Согласование документа" : "Отклонение документа");
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            if (isApprove)
            {
                _host.DocumentService.Approve(documentId.Value, _currentUser.Login, dialog.Comment);
                UiHelpers.ShowInfo("Документ согласован.");
            }
            else
            {
                _host.DocumentService.Reject(documentId.Value, _currentUser.Login, dialog.Comment);
                UiHelpers.ShowInfo("Документ отклонен.");
            }

            RefreshAllData();
        }
        catch (Exception ex)
        {
            UiHelpers.ShowError(ex.Message);
        }
    }

    private int? GetSelectedDocumentId()
    {
        if (_documentsGrid.CurrentRow?.Cells["Id"].Value is null)
        {
            return null;
        }

        return Convert.ToInt32(_documentsGrid.CurrentRow.Cells["Id"].Value);
    }

    private void ShowSelectedDocument()
    {
        var documentId = GetSelectedDocumentId();
        if (documentId is null)
        {
            _documentPreview.Clear();
            return;
        }

        var document = _host.DocumentService.GetById(documentId.Value);
        if (document is null)
        {
            _documentPreview.Clear();
            return;
        }

        _documentPreview.Text = $"Номер: {document.RegistrationNumber}{Environment.NewLine}" +
                                $"Тема: {document.Title}{Environment.NewLine}" +
                                $"Тип: {document.DocumentType}{Environment.NewLine}" +
                                $"Автор: {document.AuthorLogin}{Environment.NewLine}" +
                                $"Статус: {document.Status}{Environment.NewLine}" +
                                $"Версия: {document.CurrentVersion}{Environment.NewLine}" +
                                $"Срок: {document.DueDate?.ToString("dd.MM.yyyy") ?? "не задан"}{Environment.NewLine}" +
                                $"Создан: {document.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}{Environment.NewLine}{Environment.NewLine}" +
                                $"Содержание:{Environment.NewLine}{document.Content}";
    }
}
