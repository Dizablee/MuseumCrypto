using MuseumCrypto.Models;
using MuseumCrypto.Services;

namespace MuseumCrypto.UI;

public partial class MainForm : Form
{
    private readonly User _user;

    private readonly AccessService _access = new();
    private readonly CryptoService _crypto = new();
    private readonly StorageService _storage = new();
    private readonly AuditService _audit = new();

    private readonly TextBox _file = new() { ReadOnly = true };
    private readonly Button _pick = new() { Text = "Выбрать файл" };

    private readonly ComboBox _op = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _passphrase = new() { UseSystemPasswordChar = true, PlaceholderText = "Пароль/фраза" };

    private readonly Button _run = new() { Text = "Выполнить" };
    private readonly Label _status = new() { AutoSize = true };

    private readonly DataGridView _grid = new() { ReadOnly = true, AllowUserToAddRows = false };

    private readonly Button _usersBtn = new() { Text = "Пользователи" };
    private readonly Button _reportBtn = new() { Text = "Отчёт CSV" };
    private readonly ReportService _report = new();

    public MainForm(User user)
    {
        _user = user;

        InitializeComponent(); // <-- важно: теперь UI создаётся в Designer

        Text = $"MuseumCrypto — {_user.Login} ({_user.Role})";

        _op.Items.Add(CryptoOperationType.Encrypt);
        _op.Items.Add(CryptoOperationType.Decrypt);
        _op.SelectedIndex = 0;

        _pick.Click += PickFile;
        _run.Click += RunOperation;

        _reportBtn.Click += (_, __) => ExportReport();

        _usersBtn.Click += (_, __) =>
        {
            using var f = new UserManagementForm();
            f.ShowDialog();
        };

        _usersBtn.Enabled = _access.CanManageUsers(_user);
        _usersBtn.Visible = _access.CanManageUsers(_user);

        // журнал грузим при переключении вкладки
        // tabs находится в Designer-части, но событие удобно повесить тут:
        // (если вкладка называется "Журнал" — подхватится)
        foreach (Control c in Controls)
        {
            if (c is TabControl tc)
            {
                tc.Selected += (_, __) =>
                {
                    if (tc.SelectedTab?.Text.StartsWith("Журнал") == true)
                        LoadLogs();
                };
            }
        }
    }

    private void PickFile(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        if (ofd.ShowDialog() == DialogResult.OK)
            _file.Text = ofd.FileName;
    }

    private void RunOperation(object? sender, EventArgs e)
    {
        _status.Text = "";
        var path = _file.Text;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            _status.Text = "Выберите файл.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_passphrase.Text))
        {
            _status.Text = "Введите пароль/фразу.";
            return;
        }

        var op = (CryptoOperationType)_op.SelectedItem!;

        if (op == CryptoOperationType.Encrypt && !_access.CanEncrypt(_user))
        {
            Deny(op, "Отказ: нет прав на шифрование.");
            return;
        }

        if (op == CryptoOperationType.Decrypt && !_access.CanDecrypt(_user))
        {
            Deny(op, "Отказ: нет прав на дешифрование.");
            return;
        }

        var outPath = _storage.BuildOutputPath(path, op);

        try
        {
            if (op == CryptoOperationType.Encrypt)
                _crypto.Encrypt(path, outPath, _passphrase.Text);
            else
                _crypto.Decrypt(path, outPath, _passphrase.Text);

            _audit.Write(_user, op, Path.GetFileName(path), OperationStatus.Success,
                $"Output: {Path.GetFileName(outPath)}");

            _status.Text = $"Готово: {outPath}";
        }
        catch (Exception ex)
        {
            _audit.Write(_user, op, Path.GetFileName(path), OperationStatus.Fail, ex.Message);
            _status.Text = "Ошибка: " + ex.Message;
        }
    }

    private void Deny(CryptoOperationType op, string msg)
    {
        _audit.Write(_user, op, Path.GetFileName(_file.Text), OperationStatus.Fail, msg);
        _status.Text = msg;
    }

    private void LoadLogs()
    {
        var canAll = _access.CanViewLogs(_user);
        var data = _audit.GetFor(_user, canAll);

        _grid.DataSource = data.Select(x => new
        {
            x.Timestamp,
            x.UserLogin,
            Operation = x.Operation.ToString(),
            x.FileName,
            Status = x.Status.ToString(),
            x.Details
        }).ToList();
    }

    private void ExportReport()
    {
        try
        {
            var canAll = _access.CanViewLogs(_user);
            var logs = _audit.GetFor(_user, canAll);

            using var sfd = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "audit_report.csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            _report.ExportCsv(sfd.FileName, logs);
            _status.Text = "Отчёт сохранён: " + sfd.FileName;
        }
        catch (Exception ex)
        {
            _status.Text = "Ошибка отчёта: " + ex.Message;
        }
    }
}