namespace SedPractice.WinForms.Forms;

public sealed class CreateDocumentForm : Form
{
    private readonly TextBox _titleTextBox = new() { Dock = DockStyle.Fill };
    private readonly ComboBox _typeComboBox = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dueDatePicker = new() { Dock = DockStyle.Left, Width = 200, Format = DateTimePickerFormat.Short, ShowCheckBox = true };
    private readonly TextBox _contentTextBox = new() { Multiline = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Height = 180 };

    public string DocumentTitle => _titleTextBox.Text.Trim();
    public string DocumentType => _typeComboBox.Text;
    public string DocumentContent => _contentTextBox.Text.Trim();
    public DateTime? DueDate => _dueDatePicker.Checked ? _dueDatePicker.Value.Date : null;

    public CreateDocumentForm()
    {
        Text = "Создание документа";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(640, 420);

        _typeComboBox.Items.AddRange(["Приказ", "Служебная записка", "Договор", "Регламент", "Заявка"]);
        _typeComboBox.SelectedIndex = 1;
        _dueDatePicker.Checked = false;

        var saveButton = new Button { Text = "Сохранить", DialogResult = DialogResult.OK, Width = 120 };
        var cancelButton = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 120 };

        var form = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 5 };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        form.Controls.Add(new Label { Text = "Тема документа", AutoSize = true }, 0, 0);
        form.Controls.Add(_titleTextBox, 1, 0);
        form.Controls.Add(new Label { Text = "Тип документа", AutoSize = true }, 0, 1);
        form.Controls.Add(_typeComboBox, 1, 1);
        form.Controls.Add(new Label { Text = "Срок исполнения", AutoSize = true }, 0, 2);
        form.Controls.Add(_dueDatePicker, 1, 2);
        form.Controls.Add(new Label { Text = "Содержание", AutoSize = true }, 0, 3);
        form.Controls.Add(_contentTextBox, 1, 3);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(cancelButton);
        form.Controls.Add(buttons, 1, 4);

        Controls.Add(form);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }
}
