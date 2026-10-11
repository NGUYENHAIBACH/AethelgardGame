using System.Text.Json;
using AethelgardGame.Models.Game;
using AethelgardGame.Models.Story;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

// Không có giao diện Blazor nào: trang vẫn là HTML/JS của bản máy chủ. .NET chỉ đứng sau, trả lời các lời gọi của game.js.
await WebAssemblyHostBuilder.CreateDefault(args).Build().RunAsync();

/// <summary>Thay cho GameController của bản máy chủ. game.js gọi Call("Choose", json) thay vì POST /Game/Choose.</summary>
public static class GameApi
{
    static readonly Lazy<GameEngine> engine = new(() => new GameEngine(new StoryLibrary()));
    // cùng kiểu JSON với MVC (tên thuộc tính chữ thường đầu) để game.js không phải đổi
    static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    sealed class Req { public string? State { get; set; } public int Index { get; set; } public int Id { get; set; } }

    [JSInvokable]
    public static string Call(string action, string body)
    {
        var r = JsonSerializer.Deserialize<Req>(string.IsNullOrEmpty(body) ? "{}" : body, Web) ?? new Req();
        var e = engine.Value;
        Batch b = action switch
        {
            "Start" => e.Start(),
            "Resume" => e.Resume(GameEngine.Deser(r.State!)),
            "Choose" => e.Choose(GameEngine.Deser(r.State!), r.Index),
            "Continue" => e.Continue(GameEngine.Deser(r.State!)),
            "Retry" => e.Retry(GameEngine.Deser(r.State!), r.Id),
            _ => throw new ArgumentException("Lệnh lạ: " + action),
        };
        return JsonSerializer.Serialize(b, Web);
    }
}
