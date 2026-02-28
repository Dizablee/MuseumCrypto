using MuseumCrypto.Models;
using MuseumCrypto.Services;

namespace MuseumCrypto.UI;

public partial class UserManagementForm : Form
{
    private readonly UserManagementService _svc = new();

    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly TextBox _login = new() { PlaceholderText = "Логин" };
    private readonly TextBox _pass = new() { PlaceholderText = "Пароль", UseSystemPasswordChar = true };
    private readonly ComboBox _role = new() { DropDownStyle = ComboBoxStyle.DropDownList };

    private readonly Button _btnAdd = new() { Text = "Создать" };
    private readonly Button _btnToggle = new() { Text = "Актив./Блок." };
    private readonly Button _btnRole = new() { Text = "Сменить роль" };
    private readonly Label _msg = new() { AutoSize = true };

    public UserManagementForm()
    {
        Text = "Управление пользователями";
        Width = 900;
        Height = 500;

        _role.Items.Add(RoleType.User);
        _role.Items.Add(RoleType.Manager);
        _role.Items.Add(RoleType.Admin);
        _role.SelectedIndex = 0;

        _btnAdd.Click += (_, __) => Create();
        _btnToggle.Click += (_, __) => ToggleActive();
        _btnRole.Click += (_, __) => ChangeRole();

        var top = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 70,
            Padding = new Padding(10),
            WrapContents = false
        };

        _login.Width = 160;
        _pass.Width = 160;
        _role.Width = 120;

        top.Controls.Add(_login);
        top.Controls.Add(_pass);
        top.Controls.Add(_role);
        top.Controls.Add(_btnAdd);
        top.Controls.Add(_btnToggle);
        top.Controls.Add(_btnRole);
        top.Controls.Add(_msg);

        Controls.Add(_grid);
        Controls.Add(top);

        LoadUsers();
    }

    private void LoadUsers()
    {
        var users = _svc.GetAll();
        _grid.DataSource = users.Select(u => new
        {
            u.UserId,
            u.Login,
            u.IsActive,
            Role = u.Role.ToString(),
            u.CreatedAt
        }).ToList();

        _grid.Columns["UserId"].Visible = false;
    }

    private string? SelectedUserId()
    {
        if (_grid.CurrentRow?.DataBoundItem is null) return null;
        return _grid.CurrentRow.Cells["UserId"].Value?.ToString();
    }

    private void Create()
    {
        _msg.Text = "";
        try
        {
            _svc.CreateUser(_login.Text, _pass.Text, (RoleType)_role.SelectedItem!);
            _login.Text = "";
            _pass.Text = "";
            _msg.Text = "Пользователь создан.";
            LoadUsers();
        }
        catch (Exception ex)
        {
            _msg.Text = "Ошибка: " + ex.Message;
        }
    }

    private void ToggleActive()
    {
        _msg.Text = "";
        var id = SelectedUserId();
        if (id is null) { _msg.Text = "Выбери пользователя."; return; }

        bool isActive = Convert.ToBoolean(_grid.CurrentRow!.Cells["IsActive"].Value);
        _svc.SetActive(id, !isActive);
        LoadUsers();
        _msg.Text = !isActive ? "Разблокирован." : "Заблокирован.";
    }

    private void ChangeRole()
    {
        _msg.Text = "";
        var id = SelectedUserId();
        if (id is null) { _msg.Text = "Выбери пользователя."; return; }

        var role = (RoleType)_role.SelectedItem!;
        _svc.SetRole(id, role);
        LoadUsers();
        _msg.Text = "Роль изменена.";
    }
}