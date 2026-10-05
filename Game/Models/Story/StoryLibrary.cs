namespace AethelgardGame.Models.Story;

/// <summary>Đọc mọi file kịch bản trong thư mục Story (00_*.md, 01_*.md, ...) và biên dịch.
/// Tự đọc lại khi file đổi, nên người viết sửa .md là game đổi theo. Thêm 03_chuong_3.md là có Chương 3.</summary>
public sealed class StoryLibrary
{
    readonly string dir;
    readonly object gate = new();
    DateTime stamp = DateTime.MinValue;

    public List<Chapter> Chapters { get; private set; } = new();
    public Dictionary<int, ShardDef> Shards { get; private set; } = new();
    public List<string> Warnings { get; private set; } = new();
    public DateTime LoadedAt { get; private set; }

    public StoryLibrary(IWebHostEnvironment env)
    {
        dir = Path.Combine(env.ContentRootPath, "Story");
        Reload(force: true);
    }

    public void Refresh() => Reload(force: false);

    void Reload(bool force)
    {
        lock (gate)
        {
            var files = Directory.Exists(dir)
                ? Directory.GetFiles(dir, "*.md").OrderBy(f => Path.GetFileName(f), StringComparer.Ordinal).ToArray()
                : Array.Empty<string>();
            var newest = files.Length == 0 ? DateTime.MinValue : files.Max(File.GetLastWriteTimeUtc);
            if (!force && newest == stamp) return;

            var warnings = new List<string>();
            var shards = new Dictionary<int, ShardDef>();
            var chapters = new List<Chapter>();
            foreach (var f in files)
            {
                var text = File.ReadAllText(f);
                chapters.Add(Compiler.Compile(Path.GetFileName(f), chapters.Count, text, warnings, shards));
            }
            Chapters = chapters; Shards = shards; Warnings = warnings;
            stamp = newest; LoadedAt = DateTime.Now;
        }
    }
}
