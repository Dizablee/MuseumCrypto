using MuseumCrypto.Services;

namespace MuseumCrypto.UI;

public sealed partial class LoginForm : Form
{
    private readonly AuthService _auth = new();

    public LoginForm()
    {
        InitializeComponent();
        btnLogin.Click += OnLogin;
    }

    private void OnLogin(object? sender, EventArgs e)
    {
        lblMsg.Text = "";
        var user = _auth.Login(txtLogin.Text.Trim(), txtPass.Text);
        if (user is null)
        {
            lblMsg.Text = "Ошибка входа. Проверь логин/пароль.";
            return;
        }

        Hide();
        using var main = new MainForm(user);
        main.ShowDialog();
        Close();
    }
}