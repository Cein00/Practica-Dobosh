namespace SedPractice.WinForms.Helpers;

internal static class UiHelpers
{
    public static void ApplyGridStyle(DataGridView grid)
    {
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.RowHeadersVisible = false;
        grid.Dock = DockStyle.Fill;
    }

    public static void ShowError(string message) => MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    public static void ShowInfo(string message) => MessageBox.Show(message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
