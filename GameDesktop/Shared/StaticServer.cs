using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AethelgardDesktop;

/// <summary>Máy chủ file tĩnh nhỏ xíu, chỉ nghe trên chính máy này (127.0.0.1): phát thư mục game/ cho trình duyệt.
/// Tự viết trên TcpListener để không cần quyền quản trị và chạy như nhau trên Windows lẫn Mac.
/// Có hỗ trợ Range (trình duyệt cần nó để phát và tua nhạc).</summary>
public sealed class StaticServer
{
    // cổng cố định: bản lưu của trình duyệt gắn với địa chỉ trang, đổi cổng là mất bản lưu. Bận thì thử vài cổng kế.
    const int FirstPort = 47350;

    static readonly Dictionary<string, string> Types = new(StringComparer.OrdinalIgnoreCase)
    {
        [".html"] = "text/html; charset=utf-8", [".js"] = "text/javascript; charset=utf-8", [".css"] = "text/css; charset=utf-8",
        [".json"] = "application/json; charset=utf-8", [".wasm"] = "application/wasm", [".dat"] = "application/octet-stream",
        [".png"] = "image/png", [".webp"] = "image/webp", [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg", [".svg"] = "image/svg+xml",
        [".ico"] = "image/x-icon", [".mp3"] = "audio/mpeg", [".ogg"] = "audio/ogg", [".wav"] = "audio/wav",
    };

    readonly string root;
    readonly TcpListener listener;
    public int Port { get; }
    public string Url => $"http://127.0.0.1:{Port}/";

    public StaticServer(string root)
    {
        this.root = Path.GetFullPath(root);
        for (int p = FirstPort; ; p++)
        {
            try { listener = new TcpListener(IPAddress.Loopback, p); listener.Start(); Port = p; break; }
            catch (SocketException) when (p < FirstPort + 20) { }
        }
        _ = Task.Run(AcceptLoop);
    }

    async Task AcceptLoop()
    {
        while (true)
        {
            TcpClient c;
            try { c = await listener.AcceptTcpClientAsync(); } catch { return; }
            _ = Task.Run(() => Serve(c));
        }
    }

    async Task Serve(TcpClient client)
    {
        using var _ = client;
        try
        {
            client.NoDelay = true;
            using var net = client.GetStream();
            var buf = new byte[16384];
            while (true)   // một kết nối phục vụ nhiều yêu cầu liên tiếp (keep-alive)
            {
                int len = 0, end = -1;
                while (end < 0)
                {
                    int n = await net.ReadAsync(buf.AsMemory(len, buf.Length - len));
                    if (n <= 0) return;
                    len += n;
                    end = Encoding.ASCII.GetString(buf, 0, len).IndexOf("\r\n\r\n", StringComparison.Ordinal);
                    if (end < 0 && len == buf.Length) return;
                }
                var lines = Encoding.ASCII.GetString(buf, 0, end).Split("\r\n");
                var first = lines[0].Split(' ');
                if (first.Length < 2) return;
                string method = first[0], target = first[1];
                string? range = lines.Skip(1).FirstOrDefault(l => l.StartsWith("Range:", StringComparison.OrdinalIgnoreCase))?[6..].Trim();
                bool head = method == "HEAD";
                if (method != "GET" && !head) { await Head(net, "405 Method Not Allowed", "text/plain", 0, null); return; }

                var path = Uri.UnescapeDataString(target.Split('?', '#')[0]).TrimStart('/');
                if (path.Length == 0) path = "index.html";
                var full = Path.GetFullPath(Path.Combine(root, path));
                // không cho đi ra ngoài thư mục game
                if (!full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) || !File.Exists(full))
                {
                    var msg = Encoding.UTF8.GetBytes("Không tìm thấy.");
                    await Head(net, "404 Not Found", "text/plain; charset=utf-8", msg.Length, null);
                    if (!head) await net.WriteAsync(msg);
                    continue;
                }

                using var fs = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read);
                long size = fs.Length, from = 0, to = size - 1;
                bool partial = false;
                if (range != null && range.StartsWith("bytes=", StringComparison.Ordinal))
                {
                    var r = range[6..].Split('-');
                    if (r.Length == 2)
                    {
                        if (r[0].Length == 0 && long.TryParse(r[1], out var suffix)) { from = Math.Max(0, size - suffix); partial = true; }
                        else if (long.TryParse(r[0], out from))
                        {
                            if (r[1].Length > 0 && long.TryParse(r[1], out var t)) to = Math.Min(t, size - 1);
                            partial = true;
                        }
                    }
                    if (partial && (from > to || from >= size))
                    {
                        await Head(net, "416 Range Not Satisfiable", "text/plain", 0, $"Content-Range: bytes */{size}\r\n");
                        continue;
                    }
                }
                long count = to - from + 1;
                var type = Types.GetValueOrDefault(Path.GetExtension(full), "application/octet-stream");
                await Head(net, partial ? "206 Partial Content" : "200 OK", type, count,
                    "Accept-Ranges: bytes\r\nCache-Control: no-cache\r\n" + (partial ? $"Content-Range: bytes {from}-{to}/{size}\r\n" : ""));
                if (head) continue;
                fs.Position = from;
                var chunk = new byte[81920];
                while (count > 0)
                {
                    int n = await fs.ReadAsync(chunk.AsMemory(0, (int)Math.Min(chunk.Length, count)));
                    if (n <= 0) break;
                    await net.WriteAsync(chunk.AsMemory(0, n));
                    count -= n;
                }
            }
        }
        catch { /* trình duyệt đóng kết nối giữa chừng: bỏ qua */ }
    }

    static Task Head(NetworkStream net, string status, string type, long length, string? extra) =>
        net.WriteAsync(Encoding.ASCII.GetBytes(
            $"HTTP/1.1 {status}\r\nContent-Type: {type}\r\nContent-Length: {length}\r\n{extra}Connection: keep-alive\r\n\r\n")).AsTask();

    /// <summary>Thư mục game/ nằm cạnh chương trình.</summary>
    public static string FindGameDir()
    {
        var exe = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
        foreach (var d in new[] { Path.Combine(exe, "game"), Path.Combine(AppContext.BaseDirectory, "game") })
            if (File.Exists(Path.Combine(d, "index.html"))) return d;
        throw new DirectoryNotFoundException("Không thấy thư mục \"game\" nằm cạnh chương trình. Hãy giải nén cả gói rồi chạy lại.");
    }
}
