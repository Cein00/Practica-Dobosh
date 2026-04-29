using SedPractice.Domain.Models;
using SedPractice.WinForms.Helpers;

namespace SedPractice.WinForms.Forms;

public sealed class LoginForm : Form
{
    private readonly AppHost _host;
    private readonly TextBox _loginTextBox = new() { PlaceholderText = "Логин", Width = 260 };
    private readonly TextBox _passwordTextBox = new() { PlaceholderText = "Пароль", Width = 260, UseSystemPasswordChar = true };
    private readonly Label _hintLabel = new()
    {
        AutoSize = true,
        Text = "Демо-учетные записи: admin/admin123, ivanov/12345, petrov/12345",
        MaximumSize = new Size(320, 0)
    };

    public User? AuthenticatedUser { get; private set; }

    public LoginForm(AppHost host)
    {
        _host = host;
        Text = "СЭД - Вход";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 220);

        var title = new Label
        {
            Text = "Система электронного документооборота",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            AutoSize = true
        };

        var loginButton = new Button { Text = "Войти", Width = 120, Height = 32 };
        loginButton.Click += (_, _) => PerformLogin();

        AcceptButton = loginButton;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            RowCount = 5,
            ColumnCount = 1
        };
        layout.RowStyles.Clear();
        for (var i = 0; i < 5; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(title);
        layout.Controls.Add(_loginTextBox);
        layout.Controls.Add(_passwordTextBox);
        layout.Controls.Add(_hintLabel);
        layout.Controls.Add(loginButton);

        Controls.Add(layout);
    }

    private void PerformLogin()
    {
        var result = _host.AuthService.Login(_loginTextBox.Text, _passwordTextBox.Text);
        if (!result.IsSuccess || result.User is null)
        {
            UiHelpers.ShowError(result.Message);
            return;
        }

        AuthenticatedUser = result.User;
        DialogResult = DialogResult.OK;
        Close();
    }
}
