namespace SedPractice.WinForms.Forms;

public sealed class SubmitForApprovalForm : Form
{
    private readonly TextBox _approverTextBox = new() { Dock = DockStyle.Fill, Text = "petrov" };
    private readonly DateTimePicker _deadlinePicker = new() { Dock = DockStyle.Left, Width = 180, Format = DateTimePickerFormat.Short };

    public string ApproverLogin => _approverTextBox.Text.Trim();
    public DateTime Deadline => _deadlinePicker.Value.Date;

    public SubmitForApprovalForm()
    {
        Text = "Отправить на согласование";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(420, 160);
        _deadlinePicker.Value = DateTime.Today.AddDays(2);

        var okButton = new Button { Text = "Отправить", DialogResult = DialogResult.OK, Width = 120 };
        var cancelButton = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 120 };

        var form = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 3 };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        form.Controls.Add(new Label { Text = "Логин согласующего", AutoSize = true }, 0, 0);
        form.Controls.Add(_approverTextBox, 1, 0);
        form.Controls.Add(new Label { Text = "Срок согласования", AutoSize = true }, 0, 1);
        form.Controls.Add(_deadlinePicker, 1, 1);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);
        form.Controls.Add(buttons, 1, 2);

        Controls.Add(form);
        AcceptButton = okButton;
        CancelButton = cancelButton;
    }
}
