using AethelgardGame.Models.Game;
using AethelgardGame.Models.Story;
using Microsoft.AspNetCore.Mvc;

namespace AethelgardGame.Controllers;

public sealed class GameRequest
{
    public string? State { get; set; }
    public int Index { get; set; }
    public int Id { get; set; }
}

/// <summary>Controller của game. Máy chủ không giữ trạng thái: mỗi action nhận chuỗi JSON GameState, chạy kịch bản
/// tới điểm dừng kế tiếp và trả về loạt dòng cần hiện + trạng thái mới.</summary>
public sealed class GameController : Controller
{
    readonly GameEngine engine;
    readonly StoryLibrary lib;
    readonly IWebHostEnvironment env;

    public GameController(GameEngine engine, StoryLibrary lib, IWebHostEnvironment env)
    {
        this.engine = engine; this.lib = lib; this.env = env;
    }

    /// <summary>Màn hình chơi (View).</summary>
    [HttpGet]
    public IActionResult Play() => View();

    [HttpPost] public IActionResult Start() => Json(engine.Start());

    [HttpPost]
    public IActionResult Resume([FromBody] GameRequest r) => Json(engine.Resume(GameEngine.Deser(r.State!)));

    [HttpPost]
    public IActionResult Choose([FromBody] GameRequest r) => Json(engine.Choose(GameEngine.Deser(r.State!), r.Index));

    [HttpPost]
    public IActionResult Continue([FromBody] GameRequest r) => Json(engine.Continue(GameEngine.Deser(r.State!)));

    [HttpPost]
    public IActionResult Retry([FromBody] GameRequest r) => Json(engine.Retry(GameEngine.Deser(r.State!), r.Id));

    /// <summary>Danh sách file tài nguyên có thật; trình duyệt dùng hình/âm thay thế cho cái nào chưa có.</summary>
    [HttpGet]
    public IActionResult Manifest()
    {
        string[] Scan(string sub, params string[] exts)
        {
            var d = Path.Combine(env.WebRootPath, "assets", sub);
            if (!Directory.Exists(d)) return Array.Empty<string>();
            return Directory.GetFiles(d).Where(f => exts.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => Path.GetFileName(f)).OrderBy(x => x).ToArray();
        }
        return Json(new
        {
            bg = Scan("bg", ".jpg", ".jpeg", ".png", ".webp", ".svg"),   // .svg: nền tạm vẽ bằng nét
            sprites = Scan("sprites", ".png", ".webp"),
            bgm = Scan("bgm", ".mp3", ".ogg", ".wav"),
            se = Scan("se", ".mp3", ".ogg", ".wav"),
            ending = Scan("ending", ".png", ".webp"),
            chapters = lib.Chapters.Select(c => c.Title).ToArray(),
        });
    }
}
