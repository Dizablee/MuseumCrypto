namespace MuseumCrypto.UI;

public partial class MainForm : Form
{
    private System.ComponentModel.IContainer? components = null;

    private TabControl tabs = null!;
    private TabPage tabOps = null!;
    private TabPage tabLog = null!;

    private TableLayoutPanel rootOps = null!;
    private GroupBox gbOps = null!;
    private GroupBox gbHint = null!;

    private TableLayoutPanel ops = null!;
    private FlowLayoutPanel rightButtons = null!;
    private Label lblHint = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Window
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1020, 680);
        MinimumSize = new Size(900, 580);
        Font = new Font("Segoe UI", 10F);

        // Tabs
        tabs = new TabControl { Dock = DockStyle.Fill };
        tabOps = new TabPage("Операции");
        tabLog = new TabPage("Журнал");
        tabs.TabPages.Add(tabOps);
        tabs.TabPages.Add(tabLog);
        Controls.Add(tabs);

        // Root layout in Operations tab (2 blocks: ops + hint)
        rootOps = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 2
        };
        rootOps.RowStyles.Add(new RowStyle(SizeType.Absolute, 290));  // верх фикс
        rootOps.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // низ остаток
        tabOps.Controls.Add(rootOps);

        // Group: Operations
        gbOps = new GroupBox
        {
            Text = "Операция с файлом",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };
        rootOps.Controls.Add(gbOps, 0, 0);

        // Group: Hint
        gbHint = new GroupBox
        {
            Text = "Подсказка",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };
        rootOps.Controls.Add(gbHint, 0, 1);

        // Hint text
        lblHint = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(8),
            Text =
                "1) Нажми «Выбрать файл»\n" +
                "2) Выбери операцию Encrypt / Decrypt\n" +
                "3) Введи пароль/фразу\n" +
                "4) Нажми «Выполнить»\n\n" +
                "Результат сохранится рядом с исходным файлом."
        };
        gbHint.Controls.Add(lblHint);

        // Operations layout inside gbOps
        ops = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4
        };

        // Fixed+percent columns => не съезжает
        ops.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        ops.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        ops.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        ops.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));

        ops.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); // labels
        ops.RowStyles.Add(new RowStyle(SizeType.Absolute, 46)); // file
        ops.RowStyles.Add(new RowStyle(SizeType.Absolute, 66)); // op + pass
        ops.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // status + buttons

        // Labels
        var lblFile = new Label
        {
            Text = "Файл",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            ForeColor = Color.FromArgb(80, 80, 80)
        };
        var lblOp = new Label
        {
            Text = "Операция",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            ForeColor = Color.FromArgb(80, 80, 80)
        };
        var lblPass = new Label
        {
            Text = "Пароль/фраза",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            ForeColor = Color.FromArgb(80, 80, 80)
        };

        ops.Controls.Add(lblFile, 0, 0);
        ops.SetColumnSpan(lblFile, 2);
        ops.Controls.Add(lblOp, 2, 0);
        ops.Controls.Add(lblPass, 3, 0);

        // Style your controls
        _file.Dock = DockStyle.Fill;
        _file.Font = new Font("Segoe UI", 10.5F);
        _file.Margin = new Padding(0, 0, 10, 0);

        _pick.Dock = DockStyle.Fill;
        _pick.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _pick.Margin = new Padding(0);

        _op.Dock = DockStyle.Fill;
        _op.Font = new Font("Segoe UI", 10.5F);
        _op.Margin = new Padding(0, 0, 10, 0);

        _passphrase.Dock = DockStyle.Fill;
        _passphrase.Font = new Font("Segoe UI", 10.5F);
        _passphrase.Margin = new Padding(0);

        _status.Dock = DockStyle.Fill;
        _status.AutoSize = false;
        _status.TextAlign = ContentAlignment.MiddleLeft;
        _status.ForeColor = Color.FromArgb(20, 20, 20);
        _status.Padding = new Padding(6, 0, 0, 0);

        // Row 1 file + pick + op/pass (op/pass starting row 1 and spanning to row 2)
        ops.Controls.Add(_file, 0, 1);
        ops.Controls.Add(_pick, 1, 1);

        ops.Controls.Add(_op, 2, 1);
        ops.SetRowSpan(_op, 2);

        ops.Controls.Add(_passphrase, 3, 1);
        ops.SetRowSpan(_passphrase, 2);

        // Right buttons (fixed column)
        rightButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        _run.Text = "Выполнить";
        _reportBtn.Text = "Отчёт CSV";
        _usersBtn.Text = "Пользователи";

        _run.Width = 180;
        _reportBtn.Width = 180;
        _usersBtn.Width = 180;

        _run.Height = 42;
        _reportBtn.Height = 34;
        _usersBtn.Height = 34;

        _run.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _reportBtn.Font = new Font("Segoe UI", 10F);
        _usersBtn.Font = new Font("Segoe UI", 10F);

        rightButtons.Controls.Add(_run);
        rightButtons.Controls.Add(_reportBtn);
        rightButtons.Controls.Add(_usersBtn);

        // Status box
        var gbStatus = new GroupBox
        {
            Text = "Статус",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        gbStatus.Controls.Add(_status);

        ops.Controls.Add(gbStatus, 0, 3);
        ops.SetColumnSpan(gbStatus, 3);
        ops.Controls.Add(rightButtons, 3, 3);

        gbOps.Controls.Add(ops);

        // Log tab
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.RowHeadersVisible = false;

        tabLog.Controls.Add(_grid);

        ResumeLayout(false);
    }
}