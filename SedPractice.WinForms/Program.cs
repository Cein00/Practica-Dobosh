using System.Text;
using SedPractice.WinForms.Forms;

namespace SedPractice.WinForms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var databasePath = Path.Combine(AppContext.BaseDirectory, "sed_practice.db");
        var host = new AppHost(databasePath);

        using var loginForm = new LoginForm(host);
        if (loginForm.ShowDialog() != DialogResult.OK || loginForm.AuthenticatedUser is null)
        {
            return;
        }

        Application.Run(new MainForm(host, loginForm.AuthenticatedUser));
    }
}
