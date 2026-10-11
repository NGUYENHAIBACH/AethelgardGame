using System.Diagnostics;
using AethelgardDesktop;

// Bản cho Mac: phát thư mục game/ ngay trên máy rồi mở trình duyệt mặc định. Đóng cửa sổ Terminal này là tắt game.
try
{
    var server = new StaticServer(StaticServer.FindGameDir());
    Console.WriteLine("Aethelgard đang chạy tại " + server.Url);
    Console.WriteLine("Trình duyệt sẽ tự mở. Chơi xong thì đóng cửa sổ này.");
    try
    {
        if (OperatingSystem.IsMacOS()) Process.Start("open", server.Url);
        else if (OperatingSystem.IsWindows()) Process.Start(new ProcessStartInfo(server.Url) { UseShellExecute = true });
        else Process.Start("xdg-open", server.Url);
    }
    catch { Console.WriteLine("Không tự mở được trình duyệt. Hãy mở địa chỉ trên bằng tay."); }
    await Task.Delay(Timeout.Infinite);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.ReadLine();
}
