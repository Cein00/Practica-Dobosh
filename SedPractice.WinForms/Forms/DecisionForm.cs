namespace SedPractice.WinForms.Forms;

public sealed class DecisionForm : Form
{
    private readonly TextBox _commentTextBox = new() { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
    public string? Comment => string.IsNullOrWhiteSpace(_commentTextBox.Text) ? null : _commentTextBox.Text.Trim();

    public DecisionForm(string caption)
    {
        Text = caption;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(420, 220);

        var okButton = new Button { Text = "Подтвердить", DialogResult = DialogResult.OK, Width = 120 };
        var cancelButton = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 120 };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label { Text = "Комментарий", AutoSize = true });
        layout.Controls.Add(_commentTextBox);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);
        layout.Controls.Add(buttons);

        Controls.Add(layout);
        AcceptButton = okButton;
        CancelButton = cancelButton;
    }
}
