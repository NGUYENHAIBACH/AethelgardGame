using System.Reflection;

namespace AethelgardGame.Models.Story;

/// <summary>Bản của StoryLibrary cho trình duyệt: sáu file kịch bản được nhúng sẵn vào assembly lúc biên dịch
/// (trong trình duyệt không có ổ đĩa để đọc), biên dịch một lần khi game khởi động.</summary>
public sealed class StoryLibrary
{
    public List<Chapter> Chapters { get; } = new();
    public Dictionary<int, ShardDef> Shards { get; } = new();
    public List<string> Warnings { get; } = new();
    public DateTime LoadedAt { get; } = DateTime.Now;

    public StoryLibrary()
    {
        var asm = Assembly.GetExecutingAssembly();
        var names = asm.GetManifestResourceNames().Where(n => n.StartsWith("Story/") && n.EndsWith(".md"))
            .OrderBy(n => n, StringComparer.Ordinal);
        foreach (var n in names)
        {
            using var r = new StreamReader(asm.GetManifestResourceStream(n)!);
            Chapters.Add(Compiler.Compile(n["Story/".Length..], Chapters.Count, r.ReadToEnd(), Warnings, Shards));
        }
    }

    public void Refresh() { }
}
