using AethelgardGame.Models.Game;
using System.Text.RegularExpressions;
using AethelgardGame.Models.Story;
using Microsoft.AspNetCore.Mvc;

namespace AethelgardGame.Controllers;

/// <summary>Trang kiểm tra cho người làm game: kịch bản có đọc lỗi dòng nào không, còn thiếu file tài nguyên nào,
/// và chạy thử hàng trăm ván ngẫu nhiên để tìm dòng không bao giờ tới được. Mở tại /Check.</summary>
public sealed class CheckController : Controller
{
    readonly StoryLibrary lib;
    readonly GameEngine engine;
    readonly IWebHostEnvironment env;

    public CheckController(StoryLibrary lib, GameEngine engine, IWebHostEnvironment env)
    {
        this.lib = lib; this.engine = engine; this.env = env;
    }

    // Trang này lộ hết cốt truyện và đường đi, nên chỉ mở khi chạy ở chế độ Development (F5 trong Visual Studio).
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        if (!env.IsDevelopment()) context.Result = NotFound();
        base.OnActionExecuting(context);
    }

    public IActionResult Index()
    {
        lib.Refresh();
        return View(BuildReport());
    }

    public sealed class Report
    {
        public List<string> Warnings = new();
        public List<(string kind, string name, List<string> usedIn)> Missing = new();
        public List<(string file, string title, int instr, int says, int choices, int battles, int shards)> Chapters = new();
        public List<int> ShardsDefined = new();
        public List<int> ShardsNeverOpened = new();
    }

    Report BuildReport()
    {
        var r = new Report { Warnings = lib.Warnings.ToList() };
        string Dir(string sub) => Path.Combine(env.WebRootPath, "assets", sub);
        HashSet<string> Names(string sub) => Directory.Exists(Dir(sub))
            ? Directory.GetFiles(Dir(sub)).Select(f => Path.GetFileNameWithoutExtension(f)).ToHashSet()
            : new HashSet<string>();
        var have = new Dictionary<string, HashSet<string>>
        {
            ["Nền (BG)"] = Names("bg"), ["Nhạc (BGM)"] = Names("bgm"), ["Hiệu ứng (SE)"] = Names("se"), ["Sprite"] = Names("sprites"),
        };
        var used = new Dictionary<string, Dictionary<string, List<string>>>
        {
            ["Nền (BG)"] = new(), ["Nhạc (BGM)"] = new(), ["Hiệu ứng (SE)"] = new(), ["Sprite"] = new(),
        };
        foreach (var ch in lib.Chapters)
            foreach (var c in ch.Code.OfType<CmdI>())
            {
                if (c.Value == null) continue;
                var k = c.Kind switch { CmdKind.Bg => "Nền (BG)", CmdKind.Bgm => "Nhạc (BGM)", CmdKind.Se => "Hiệu ứng (SE)", CmdKind.Sprite => "Sprite", _ => null };
                if (k == null) continue;
                if (!used[k].TryGetValue(c.Value, out var l)) used[k][c.Value] = l = new();
                var where = ch.File + ":" + c.Line;
                if (l.Count < 3) l.Add(where);
            }
        foreach (var (k, dict) in used)
            foreach (var (name, where) in dict.OrderBy(x => x.Key))
                if (!have[k].Contains(name)) r.Missing.Add((k, name, where));

        foreach (var ch in lib.Chapters)
            r.Chapters.Add((ch.File, ch.Title, ch.Code.Count, ch.Code.OfType<SayI>().Count(),
                ch.Code.OfType<AskChoiceI>().Count(), ch.Code.OfType<AskBattleI>().Count(),
                ch.Code.OfType<OpenShardI>().Sum(o => o.Numbers.Count)));

        r.ShardsDefined = lib.Shards.Keys.OrderBy(x => x).ToList();
        var opened = new HashSet<int>();
        foreach (var ch in lib.Chapters)
        {
            foreach (var o in ch.Code.OfType<OpenShardI>()) foreach (var n in o.Numbers) opened.Add(n);
            foreach (var e in ch.Code.OfType<EffectI>()) foreach (var n in e.Shards) opened.Add(n);
        }
        opened.UnionWith(new[] { 6, 14, 15, 16 });   // mở bằng luật kết trận
        r.ShardsNeverOpened = r.ShardsDefined.Where(n => !opened.Contains(n)).ToList();
        return r;
    }

    // ───────────────────────────────────────── chạy thử ngẫu nhiên

    /// <summary>GET /Check/Simulate?runs=500&amp;strategy=random|a|c</summary>
    public IActionResult Simulate(int runs = 300, string strategy = "random", int seed = 1)
    {
        lib.Refresh();
        var rnd = new Random(seed);
        var reached = new HashSet<(int ch, int pc)>();
        engine.Trace = reached;
        var endings = new Dictionary<string, int>();
        var errors = new List<string>();
        var shardSeen = new HashSet<int>();
        var itemSeen = new HashSet<string>();
        var noteSeen = new HashSet<string>();
        int totalSteps = 0;

        for (int run = 0; run < runs; run++)
        {
            try
            {
                var b = engine.Start();
                for (int guard = 0; guard < 400; guard++)
                {
                    var st = GameEngine.Deser(b.State);
                    // ghi nhận mọi lệnh đã đi qua: chạy lại từ StartState không cần; dùng bước đã phát
                    totalSteps += b.Steps.Count;
                    foreach (var s in b.Steps)
                    {
                        if ((string)s["t"]! == "item") itemSeen.Add((string)s["name"]!);
                        if ((string)s["t"]! == "note") noteSeen.Add((string)s["name"]!);
                        if ((string)s["t"]! == "shard")
                            foreach (var sh in (List<Dictionary<string, object?>>)s["shards"]!) shardSeen.Add((int)sh["n"]!);
                    }
                    var type = (string)b.Pause["type"]!;
                    if (type == "end") { var t = (string)b.Pause["title"]!; endings[t] = endings.GetValueOrDefault(t) + 1; break; }
                    if (type == "finished") { endings["(hết nội dung)"] = endings.GetValueOrDefault("(hết nội dung)") + 1; break; }
                    if (type == "chapterEnd")
                    {
                        if (!(bool)b.Pause["hasNext"]!) { endings["(hết nội dung hiện có)"] = endings.GetValueOrDefault("(hết nội dung hiện có)") + 1; break; }
                        b = engine.Continue(st); continue;
                    }
                    int count = ((System.Collections.IList)b.Pause["options"]!).Count;
                    int pick = Pick(st, type, count, strategy, rnd);
                    b = engine.Choose(st, pick);
                }
            }
            catch (Exception e) { errors.Add($"ván {run}: {e.GetType().Name}: {e.Message}"); }
        }

        var unreached = new List<string>();
        foreach (var ch in lib.Chapters)
            for (int pc = 0; pc < ch.Code.Count; pc++)
                if (ch.Code[pc] is SayI say && !reached.Contains((ch.Index, pc)))
                    unreached.Add($"{ch.File}:{say.Line}  [{say.Kind}] {say.Name}: {Shorten(say.Text)}");

        return Json(new
        {
            runs, strategy, totalSteps, errors, endings,
            shardsSeen = shardSeen.OrderBy(x => x), itemsSeen = itemSeen.OrderBy(x => x), notesSeen = noteSeen.OrderBy(x => x),
            unreachedCount = unreached.Count, unreached,
        });
    }

    /// <summary>GET /Check/Coverage: mỗi dòng của file .md phải có chữ nằm trong game (trừ các ghi chú soạn thảo đã bỏ).
    /// Dòng nào in ra ở đây là dòng có chữ không tìm thấy trong lệnh đã biên dịch: có thể đã bị đọc sót.</summary>
    public IActionResult Coverage()
    {
        lib.Refresh();
        var sb = new System.Text.StringBuilder();
        var corpus = new System.Text.StringBuilder();
        foreach (var ch in lib.Chapters)
            foreach (var ins in ch.Code)
                switch (ins)
                {
                    case SayI s: corpus.AppendLine(s.Text); break;
                    case AddItemI i: corpus.AppendLine(i.Name); corpus.AppendLine(i.Desc); break;
                    case AddNoteI n: corpus.AppendLine(n.Name); break;
                    case EndScreenI e: corpus.AppendLine(e.Title); foreach (var l in e.Lesson) corpus.AppendLine(l); break;
                    case AskChoiceI a: foreach (var o in a.Options) corpus.AppendLine(o.Text); break;
                    case AskBattleI b: foreach (var o in b.Options) corpus.AppendLine(o.Text); break;
                    case CmdI c when c.Kind == CmdKind.Card: corpus.AppendLine(c.Value); break;
                }
        foreach (var sh in lib.Shards.Values) { corpus.AppendLine(sh.Title); foreach (var p in sh.Paras) corpus.AppendLine(p); }
        var all = corpus.ToString();
        var dir = Path.Combine(env.ContentRootPath, "Story");
        int bad = 0, total = 0;
        foreach (var ch in lib.Chapters)
        {
            var skippedLines = ch.Skipped.Select(x => int.Parse(x[..x.IndexOf(':')])).ToHashSet();
            var lines = System.IO.File.ReadAllLines(Path.Combine(dir, ch.File));
            for (int n = 0; n < lines.Length; n++)
            {
                var line = lines[n].Trim();
                if (line.Length == 0 || line.StartsWith('#') || line.StartsWith("**") || line.StartsWith('>') || skippedLines.Contains(n + 1)) continue;
                line = Regex.Replace(line, @"\s*\[(GIEO|TRẢ|GẶT)[^\]]*\]", "");
                line = Regex.Replace(line, @"^-\s+", "");
                line = Regex.Replace(line, @"^Nếu [^:.]*?(\s\([^()]*\))?[.:]\s*", "");     // "Nếu 1-C. ..." / "Nếu cầm X:"
                line = Regex.Replace(line, @"^(\[[A-Za-z]+(_[A-Za-z]+)+\]\s*)+", "");        // [Helena_Smile]
                line = Regex.Replace(line, @"^(\[(BG|BGM|SE|SPRITE):[^\]]*\]\s*)+", "");
                if (line.Length == 0 || line.StartsWith('→') || line.StartsWith('[')) continue;
                // tách "(ghi chú) Tên: lời" và "A: .. / B: .."
                var segs = new List<string>();
                var rest = line;
                while (rest.StartsWith('('))
                {
                    int d = 0, e = -1;
                    for (int k = 0; k < rest.Length; k++) { if (rest[k] == '(') d++; else if (rest[k] == ')' && --d == 0) { e = k; break; } }
                    if (e < 0) break;
                    segs.Add(rest[..(e + 1)]); rest = rest[(e + 1)..].Trim();
                }
                if (rest.Length > 0)
                    foreach (var part in Regex.Split(rest, @"\s/\s(?=[^\s/""][^:/\[\]""]{0,30}(\s\([^)]*\))?:\s)"))
                    {
                        var m = Regex.Match(part, @"^(?<n>[^\s\[\]()""][^:\[\]""]*?)(\s\((?<m>[^)]*)\))?:\s(?<t>.+)$");
                        if (m.Success) segs.Add(m.Groups["t"].Value.Trim()); else segs.Add(part);
                    }
                foreach (var s0 in segs)
                {
                    var s = Regex.Replace(s0, @"\s*\(lời hát tạm[^)]*\)", "").Trim();
                    if (s.Length > 1 && s[0] == '"' && s[^1] == '"' && s.Count(c => c == '"') == 2) s = s[1..^1];
                    if (s.StartsWith("(Ghi chú") || s.StartsWith("(Từ đây")) continue;
                    total++;
                    if (!all.Contains(s)) { bad++; sb.AppendLine($"{ch.File}:{n + 1}: «{(s.Length > 110 ? s[..110] + "…" : s)}»"); }
                }
            }
        }
        sb.Insert(0, $"Đoạn chữ đã kiểm: {total}; không tìm thấy trong game: {bad}\n");
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    /// <summary>GET /Check/Skipped: các dòng kịch bản bị coi là ghi chú soạn thảo và không hiện cho người chơi. Người viết rà lại.</summary>
    public IActionResult Skipped()
    {
        lib.Refresh();
        var sb = new System.Text.StringBuilder();
        foreach (var ch in lib.Chapters)
        {
            sb.AppendLine("== " + ch.File);
            foreach (var s in ch.Skipped) sb.AppendLine("  " + (s.Length > 160 ? s[..160] + "…" : s));
        }
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    /// <summary>GET /Check/Transcript?keys=A,C,B,A,A,C,D,...  Chơi theo danh sách phím và in ra toàn bộ chữ người chơi sẽ thấy.
    /// Điểm chọn: A/B/C. Đối chất: A/B/C/L(Lùi)/G(Giữ lời)/D(Đòn); thiếu phím thì chọn đáp án đầu tiên.</summary>
    public IActionResult Transcript(string keys = "", int chapters = 99)
    {
        lib.Refresh();
        var ks = new Queue<string>((keys ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        var sb = new System.Text.StringBuilder();
        var b = engine.Start();
        for (int guard = 0; guard < 400; guard++)
        {
            foreach (var s in b.Steps)
            {
                var t = (string)s["t"]!;
                switch (t)
                {
                    case "say":
                        var kind = (string)s["kind"]!;
                        s.TryGetValue("name", out var nm);
                        sb.AppendLine(kind switch
                        {
                            "narr" => "  Dẫn chuyện: " + s["text"],
                            "stage" => "  " + s["text"],
                            "board" => "  [BẢNG] " + s["text"],
                            _ => $"  {nm}{(kind == "think" ? " (nghĩ)" : kind == "loa" ? " (loa)" : "")}: {s["text"]}",
                        });
                        break;
                    case "bg": case "bgm": case "se": case "spr": sb.AppendLine($"  <{t} {s["v"]}>"); break;
                    case "item": sb.AppendLine($"  <VẬT PHẨM MỚI: {s["name"]}> {s["desc"]}"); break;
                    case "note": sb.AppendLine($"  <GHI CHÚ MỚI: {s["name"]}>"); break;
                    case "shard":
                        sb.AppendLine("  <MẢNH: " + string.Join(", ", ((List<Dictionary<string, object?>>)s["shards"]!).Select(x => x["n"])) + ">");
                        foreach (var sh in (List<Dictionary<string, object?>>)s["shards"]!)
                        {
                            if ((int)sh["n"]! != 6) continue;   // Mảnh 06 có số liệu động: in ra để kiểm tra
                            foreach (var p in (List<Dictionary<string, object?>>)sh["paras"]!)
                                sb.AppendLine($"      [{p["kind"]}{(p.TryGetValue("hi", out var hi) && hi != null ? ", hi=" + hi : "")}] {p["text"]}");
                        }
                        break;
                    case "bar": sb.AppendLine($"  <thanh Lung lay={s["l"]} Dao động={s["d"]}>"); break;
                    case "bars": sb.AppendLine($"  <hai thanh {(((bool)s["on"]!) ? "hiện" : "ẩn")}>"); break;
                    case "card": sb.AppendLine($"=== {s["text"]} ==="); break;
                    case "elev": sb.AppendLine($"  <thang {s["on"]}>"); break;
                }
            }
            var st = GameEngine.Deser(b.State);
            var type = (string)b.Pause["type"]!;
            if (type == "end") { sb.AppendLine("━━ " + b.Pause["title"] + " ━━"); foreach (var l in (List<string>)b.Pause["lesson"]!) sb.AppendLine("  > " + l); break; }
            if (type is "finished") break;
            if (type == "chapterEnd")
            {
                sb.AppendLine("── hết chương ──");
                if (!(bool)b.Pause["hasNext"]! || st.Chapter + 1 >= chapters) break;
                b = engine.Continue(st); continue;
            }
            var ins = lib.Chapters[st.Chapter].Code[st.Pc];
            string key = ks.Count > 0 ? ks.Dequeue() : "";
            int pick = 0;
            if (ins is AskChoiceI ac)
            {
                var vis = ac.Options.Where(o => o.Cond == null || o.Cond.Eval(st)).ToList();
                pick = Math.Max(0, vis.FindIndex(o => o.Key.ToString() == key));
                sb.AppendLine($"\n>>> LỰA CHỌN {ac.Id}: " + string.Join(" | ", vis.Select(o => o.Key + "=" + o.Text)) + $"  → chọn {vis[pick].Key}");
            }
            else if (ins is AskBattleI ab)
            {
                var el = GameEngine.Eligible(ab, st);
                string Map(string k) => k switch { "L" => "Lùi", "G" => "Giữ lời", "D" => "Đòn", _ => k };
                pick = Math.Max(0, el.FindIndex(o => o.Key == Map(key)));
                sb.AppendLine("\n>>> ĐÁP ÁN: " + string.Join(" | ", el.Select(o => o.Key + (o.Variant != null ? "/" + o.Variant : "") + "=" + Shorten(o.Text))) + $"  → chọn {el[pick].Key}");
            }
            b = engine.Choose(st, pick);
        }
        var fin = GameEngine.Deser(b.State);
        sb.AppendLine("\n[Biến] " + string.Join(", ", fin.Vars.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)));
        sb.AppendLine("[Cờ] " + string.Join(", ", fin.Flags.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)));
        sb.AppendLine("[Vật phẩm] " + string.Join("; ", fin.Items.Select(i => i.Name)));
        sb.AppendLine("[Ghi chú] " + string.Join("; ", fin.Notes));
        sb.AppendLine("[Mảnh] " + string.Join(",", fin.Shards));
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    static string Shorten(string s) => s.Length > 90 ? s[..90] + "…" : s;

    int Pick(GameState st, string type, int count, string strategy, Random rnd)
    {
        if (strategy == "random" || count <= 1) return rnd.Next(count);
        var ins = lib.Chapters[st.Chapter].Code[st.Pc];
        if (ins is AskChoiceI ac)
        {
            var vis = ac.Options.Where(o => o.Cond == null || o.Cond.Eval(st)).ToList();
            var want = strategy == "a" ? 'A' : strategy == "c" ? 'C' : 'B';
            var i = vis.FindIndex(o => o.Key == want);
            return i >= 0 ? i : rnd.Next(count);
        }
        if (ins is AskBattleI ab)
        {
            var el = GameEngine.Eligible(ab, st);
            // a: toàn đáp án [A] (Dao động cao). c: [C] rồi [Đòn] nếu có, không thì [Giữ lời]
            string[] order = strategy == "a" ? new[] { "A" } : new[] { "Đòn", "C", "Giữ lời", "Lùi" };
            foreach (var k in order) { var i = el.FindIndex(o => o.Key == k); if (i >= 0) return i; }
        }
        return rnd.Next(count);
    }
}
