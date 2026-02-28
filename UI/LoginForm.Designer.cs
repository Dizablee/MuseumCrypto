namespace MuseumCrypto.UI;

public sealed partial class LoginForm : Form
{
    private System.ComponentModel.IContainer? components = null;

    private TableLayoutPanel tlp = null!;
    private Label lblTitle = null!;
    private Label lblSub = null!;
    private TextBox txtLogin = null!;
    private TextBox txtPass = null!;
    private Button btnLogin = null!;
    private Label lblMsg = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        Text = "Вход";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(380, 260);

        tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 7
        };
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // title
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); // sub
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 16)); // spacer
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // login
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // pass
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 46)); // button
        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // msg 

        lblTitle = new Label
        {
            Dock = DockStyle.Fill,
            Text = "MuseumCrypto",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold)
        };

        lblSub = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Вход в систему",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(90, 90, 90),
            Font = new Font("Segoe UI", 10F)
        };

        txtLogin = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            PlaceholderText = "Логин"
        };

        txtPass = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            PlaceholderText = "Пароль",
            UseSystemPasswordChar = true
        };

        btnLogin = new Button
        {
            Dock = DockStyle.Fill,
            Text = "Войти",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold)
        };

        lblMsg = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopCenter,
            ForeColor = Color.Maroon,
            Font = new Font("Segoe UI", 10F),
            AutoSize = false
        };

        tlp.Controls.Add(lblTitle, 0, 0);
        tlp.Controls.Add(lblSub, 0, 1);
        tlp.Controls.Add(new Label { Dock = DockStyle.Fill }, 0, 2);
        tlp.Controls.Add(txtLogin, 0, 3);
        tlp.Controls.Add(txtPass, 0, 4);
        tlp.Controls.Add(btnLogin, 0, 5);
        tlp.Controls.Add(lblMsg, 0, 6);

        Controls.Add(tlp);

        AcceptButton = btnLogin;

        ResumeLayout(false);
    }
}