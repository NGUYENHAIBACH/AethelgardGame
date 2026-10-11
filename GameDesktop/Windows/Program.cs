using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace AethelgardDesktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        try { Application.Run(new GameWindow()); }
        catch (Exception e) { MessageBox.Show(e.Message, "Aethelgard", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}

/// <summary>Cửa sổ game. Thư mục game/ được phát bằng máy chủ nhỏ ngay trong chương trình (chỉ máy này thấy),
/// WebView2 hiện nó trong một cửa sổ không có thanh địa chỉ. F11: toàn màn hình.</summary>
sealed class GameWindow : Form
{
    readonly WebView2 view = new() { Dock = DockStyle.Fill };
    readonly StaticServer server;
    bool full; FormBorderStyle oldBorder; FormWindowState oldState;

    public GameWindow()
    {
        Text = "Aethelgard";
        BackColor = Color.FromArgb(4, 6, 12);
        // cửa sổ 16:9, chiếm khoảng 85% vùng làm việc của màn hình (cỡ cố định sẽ quá nhỏ trên màn hình phóng 150%)
        var wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1280, 720);
        int w = (int)Math.Min(wa.Width * .85, wa.Height * .85 * 16 / 9);
        ClientSize = new Size(w, w * 9 / 16);
        MinimumSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;
        try { Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!); } catch { }
        server = new StaticServer(StaticServer.FindGameDir());
        Controls.Add(view);
        Load += async (_, _) => await Start();
    }

    async Task Start()
    {
        try
        {
            // dữ liệu của WebView2 (trong đó có bản lưu) để ở thư mục người dùng: thư mục game có thể nằm ở chỗ không ghi được
            var data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aethelgard");
            // cho nhạc phát ngay, không đợi người chơi bấm (trình duyệt thường chặn; đây là game của mình nên bỏ chặn)
            var opt = new CoreWebView2EnvironmentOptions("--autoplay-policy=no-user-gesture-required");
            var env = await CoreWebView2Environment.CreateAsync(null, data, opt);
            await view.EnsureCoreWebView2Async(env);
            var s = view.CoreWebView2.Settings;
            s.AreDefaultContextMenusEnabled = false; s.IsStatusBarEnabled = false; s.IsZoomControlEnabled = false;
            s.AreBrowserAcceleratorKeysEnabled = false;   // F5, Ctrl+R... không làm tải lại trang giữa lúc chơi
            view.CoreWebView2.DocumentTitleChanged += (_, _) => Text = view.CoreWebView2.DocumentTitle;
            view.CoreWebView2.NewWindowRequested += (_, e) =>
            {
                e.Handled = true;   // liên kết ra ngoài (nếu có) mở bằng trình duyệt của máy, không mở thêm cửa sổ game
                try { Process.Start(new ProcessStartInfo(e.Uri) { UseShellExecute = true }); } catch { }
            };
            view.KeyDown += (_, e) => { if (e.KeyCode == Keys.F11) ToggleFull(); };
            view.CoreWebView2.Navigate(server.Url + "index.html");
        }
        catch (Exception e)
        {
            // máy không có WebView2 (Windows 10 rất cũ): chơi bằng trình duyệt mặc định, chương trình này ở lại làm máy chủ
            Controls.Remove(view);
            Controls.Add(new Label
            {
                Dock = DockStyle.Fill, ForeColor = Color.Gainsboro, Font = new Font("Segoe UI", 11), TextAlign = ContentAlignment.MiddleCenter,
                Text = "Máy này chưa có WebView2 nên game được mở bằng trình duyệt.\n\nĐịa chỉ: " + server.Url +
                       "\n\nGiữ cửa sổ này mở trong lúc chơi; đóng nó là tắt game.\n\n(" + e.Message + ")",
            });
            ClientSize = new Size(640, 300);
            try { Process.Start(new ProcessStartInfo(server.Url) { UseShellExecute = true }); } catch { }
        }
    }

    void ToggleFull()
    {
        if (!full) { oldBorder = FormBorderStyle; oldState = WindowState; FormBorderStyle = FormBorderStyle.None; WindowState = FormWindowState.Normal; WindowState = FormWindowState.Maximized; }
        else { FormBorderStyle = oldBorder; WindowState = oldState; }
        full = !full;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F11) { ToggleFull(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
