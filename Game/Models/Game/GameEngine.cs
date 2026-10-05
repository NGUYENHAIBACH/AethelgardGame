using System.Text.Json;
using System.Text.RegularExpressions;
using AethelgardGame.Models.Story;

namespace AethelgardGame.Models.Game;

public sealed class Batch
{
    public string StartState { get; set; } = "";
    public string State { get; set; } = "";
    public Dictionary<string, object?> Scene { get; set; } = new();
    public Dictionary<string, object?> Notebook { get; set; } = new();
    public List<Dictionary<string, object?>> Steps { get; set; } = new();
    public Dictionary<string, object?> Pause { get; set; } = new();
}

/// <summary>Chạy chương trình đã biên dịch. Mỗi lần gọi chạy tới khi cần người chơi quyết (lựa chọn, đáp án đối chất, màn kết cục, hết chương)
/// rồi trả về cả loạt dòng cần hiện ("batch") để trình duyệt tự phát, không phải hỏi máy chủ từng dòng.</summary>
public sealed class GameEngine
{
    readonly StoryLibrary lib;
    public static readonly JsonSerializerOptions J = new() { PropertyNamingPolicy = null };

    /// <summary>Chỉ dùng cho trang /Check: ghi lại các lệnh đã chạy qua.</summary>
    public HashSet<(int ch, int pc)>? Trace;

    public GameEngine(StoryLibrary lib) { this.lib = lib; }

    public static string Ser(GameState s) => JsonSerializer.Serialize(s, J);
    public static GameState Deser(string json) => JsonSerializer.Deserialize<GameState>(json, J) ?? new GameState();
    static GameState Clone(GameState s) => Deser(Ser(s));

    public GameState NewGame() => new();

    // ───────────────────────────────────────── API

    public Batch Start() => Run(NewGame());

    public Batch Resume(GameState s) => Run(s);

    public Batch Continue(GameState s)
    {
        if (CurrentInstr(s) is ChapterEndI && s.Chapter + 1 < lib.Chapters.Count) { s.Chapter++; s.Pc = 0; }
        return Run(s);
    }

    public Batch Choose(GameState s, int index)
    {
        var ins = CurrentInstr(s);
        if (ins is AskChoiceI ac)
        {
            var vis = VisibleChoices(ac, s);
            if (index < 0 || index >= vis.Count) throw new ArgumentOutOfRangeException(nameof(index));
            var o = vis[index];
            s.Flags["lua_chon_" + ac.Id] = o.Key.ToString();
            var sink = new List<Dictionary<string, object?>>();
            foreach (var e in o.Effs) Apply(s, e, sink);
            s.Pc++;
        }
        else if (ins is AskBattleI ab)
        {
            var el = Eligible(ab, s);
            if (index < 0 || index >= el.Count) throw new ArgumentOutOfRangeException(nameof(index));
            s.Pc = el[index].Addr;
        }
        else throw new InvalidOperationException("Trạng thái không đang chờ lựa chọn.");
        return Run(s);
    }

    /// <summary>"Quay lại điểm chọn" ở màn kết cục.</summary>
    public Batch Retry(GameState s, int choiceId)
    {
        if (!s.Checkpoints.TryGetValue(choiceId.ToString(), out var json)) throw new InvalidOperationException("Chưa có điểm chọn " + choiceId);
        var r = Deser(json);
        r.Checkpoints = s.Checkpoints.Where(kv => int.Parse(kv.Key) < choiceId).ToDictionary(kv => kv.Key, kv => kv.Value);
        return Run(r);
    }

    // ───────────────────────────────────────── chạy

    Instr? CurrentInstr(GameState s)
    {
        if (s.Chapter >= lib.Chapters.Count) return null;
        var code = lib.Chapters[s.Chapter].Code;
        return s.Pc < code.Count ? code[s.Pc] : null;
    }

    public Batch Run(GameState s)
    {
        lib.Refresh();
        var b = new Batch { StartState = Ser(s) };
        b.Scene = SceneOf(s);
        b.Notebook = NotebookOf(s);
        var steps = b.Steps;
        int guard = 0;

        while (true)
        {
            if (++guard > 20000) throw new InvalidOperationException("Kịch bản chạy vòng lặp: chương " + s.Chapter + ", pc " + s.Pc);
            if (s.Chapter >= lib.Chapters.Count) { b.Pause = new() { ["type"] = "finished" }; break; }
            var ch = lib.Chapters[s.Chapter];
            if (s.Pc >= ch.Code.Count) { b.Pause = ChapterEndPause(s); break; }
            var ins = ch.Code[s.Pc];
            Trace?.Add((s.Chapter, s.Pc));

            switch (ins)
            {
                case SayI say:
                    steps.Add(SayStep(say)); s.Pc++; break;

                case CmdI cmd:
                    Cmd(s, cmd, steps); s.Pc++; break;

                case AddItemI it:
                    if (s.Items.All(x => x.Name != it.Name))
                    {
                        s.Items.Add(new ItemRec { Name = it.Name, Desc = it.Desc });
                        steps.Add(new() { ["t"] = "item", ["name"] = it.Name, ["desc"] = it.Desc });
                    }
                    s.Pc++; break;

                case AddNoteI n:
                    AddNote(s, n.Name, steps); s.Pc++; break;

                case OpenShardI os:
                    OpenShards(s, os.Numbers, steps); s.Pc++; break;

                case EffectI e:
                    foreach (var f in e.Effs) Apply(s, f, steps);
                    if (e.Shards.Count > 0) OpenShards(s, e.Shards, steps);
                    if (e.NoteToAdd != null) AddNote(s, e.NoteToAdd, steps);
                    s.Pc++; break;

                case JumpIfNotI j:
                    s.Pc = j.Cond.Eval(s) ? s.Pc + 1 : j.Target; break;

                case JumpI jmp:
                    s.Pc = jmp.Target; break;

                case SpecialI sp:
                    Special(s, ch, sp, steps); s.Pc++; break;

                case AskChoiceI ac:
                {
                    var snap = Clone(s); snap.Checkpoints = new();
                    s.Checkpoints[ac.Id.ToString()] = Ser(snap);
                    var vis = VisibleChoices(ac, s);
                    b.Pause = new()
                    {
                        ["type"] = "choice", ["id"] = ac.Id,
                        ["options"] = vis.Select((o, k) => new Dictionary<string, object?> { ["i"] = k, ["text"] = o.Text }).ToList(),
                    };
                    goto done;
                }

                case AskBattleI ab:
                {
                    var el = Eligible(ab, s);
                    b.Pause = new()
                    {
                        ["type"] = "battle",
                        ["options"] = el.Select((o, k) => new Dictionary<string, object?> { ["i"] = k, ["text"] = o.Text }).ToList(),
                    };
                    goto done;
                }

                case EndScreenI es:
                    b.Pause = new()
                    {
                        ["type"] = "end", ["title"] = es.Title, ["lesson"] = es.Lesson,
                        ["retry"] = ch.ChoiceIds.Select(id => new Dictionary<string, object?> { ["id"] = id, ["label"] = "Lựa chọn " + id }).ToList(),
                    };
                    goto done;

                case ChapterEndI:
                    b.Pause = ChapterEndPause(s); goto done;

                default: throw new InvalidOperationException("Lệnh lạ: " + ins.GetType().Name);
            }
        }
    done:
        b.State = Ser(s);
        return b;
    }

    Dictionary<string, object?> ChapterEndPause(GameState s)
    {
        var ch = lib.Chapters[s.Chapter];
        bool next = s.Chapter + 1 < lib.Chapters.Count;
        return new()
        {
            ["type"] = "chapterEnd", ["title"] = ch.Title, ["hasNext"] = next,
            ["nextTitle"] = next ? lib.Chapters[s.Chapter + 1].Title : null,
        };
    }

    // ───────────────────────────────────────── từng loại lệnh

    static Dictionary<string, object?> SayStep(SayI s)
    {
        var d = new Dictionary<string, object?> { ["t"] = "say", ["kind"] = s.Kind.ToString().ToLowerInvariant(), ["text"] = s.Text };
        if (s.Name.Length > 0) d["name"] = s.Name;
        if (s.HideSprite) d["hs"] = true;
        if (s.Loud) d["loud"] = true;
        if (s.Whisper) d["whisper"] = true;
        if (s.WristTeal) d["teal"] = true;
        return d;
    }

    static void Cmd(GameState s, CmdI c, List<Dictionary<string, object?>> steps)
    {
        switch (c.Kind)
        {
            case CmdKind.Bg:
                if (s.Elev) { s.Elev = false; steps.Add(new() { ["t"] = "elev", ["on"] = false }); }
                s.Bg = c.Value; steps.Add(new() { ["t"] = "bg", ["v"] = c.Value }); break;
            case CmdKind.Bgm:
                s.Bgm = c.Value; steps.Add(new() { ["t"] = "bgm", ["v"] = c.Value }); break;
            case CmdKind.Se:
                steps.Add(new() { ["t"] = "se", ["v"] = c.Value }); break;
            case CmdKind.Sprite:
                s.Sprite = c.Value; steps.Add(new() { ["t"] = "spr", ["v"] = c.Value }); break;
            case CmdKind.SceneStart:
                s.Sprite = null; steps.Add(new() { ["t"] = "spr", ["v"] = null }); break;
            case CmdKind.Card:
                steps.Add(new() { ["t"] = "card", ["text"] = c.Value }); break;
            case CmdKind.Elevator:
                s.Elev = true; steps.Add(new() { ["t"] = "elev", ["on"] = true }); break;
        }
    }

    static void Apply(GameState s, Eff e, List<Dictionary<string, object?>> steps)
    {
        if (e.If != null && !e.If.Eval(s)) return;
        if (e.Unless != null && e.Unless.Eval(s)) return;
        if (e.SetFlagValue != null) { s.Flags[e.Var] = e.SetFlagValue; return; }
        s.Add(e.Var, e.Delta);
        if (e.Var == "lung_lay") s.Vars[e.Var] = Math.Clamp(s.Var(e.Var), 0, s.LMax > 0 ? s.LMax : int.MaxValue);
        if (e.Var == "dao_dong") s.Vars[e.Var] = Math.Clamp(s.Var(e.Var), 0, s.DMax > 0 ? s.DMax : int.MaxValue);
        if (e.Var is "lung_lay" or "dao_dong")
            steps.Add(new() { ["t"] = "bar", ["l"] = s.Var("lung_lay"), ["d"] = s.Var("dao_dong") });
    }

    static void AddNote(GameState s, string name, List<Dictionary<string, object?>> steps)
    {
        if (s.Notes.Contains(name)) return;
        s.Notes.Add(name);
        if (name.StartsWith("Rian đã nghe")) s.Flags["rian_da_nghe"] = "co";
        steps.Add(new() { ["t"] = "note", ["name"] = name });
    }

    void OpenShards(GameState s, IEnumerable<int> numbers, List<Dictionary<string, object?>> steps)
    {
        var fresh = numbers.Where(n => !s.Shards.Contains(n)).Distinct().ToList();
        if (fresh.Count == 0) return;
        s.Shards.AddRange(fresh);
        // Mảnh 06 chốt số liệu ngay lúc mở (các bộ đếm lập trường không đổi sau trận Helena)
        steps.Add(new() { ["t"] = "shard", ["shards"] = fresh.Select(n => ShardView(n, s)).ToList() });
    }

    void Special(GameState s, Chapter ch, SpecialI sp, List<Dictionary<string, object?>> steps)
    {
        switch (sp.Name)
        {
            case "bars_on":
            {
                bool vane = ch.BattleKind == "vane";
                s.LMax = vane ? 10 : 8; s.DMax = vane ? 11 : 8;     // Chương 1: 0..8 / 0..8. Chương 2: Lung lay 0..10, Dao động 0..11
                s.Vars["lung_lay"] = 0;
                s.Vars["dao_dong"] = vane && s.Flag("lua_chon_5") == "A" ? 1 : 0;   // 5-A vào trận với Dao động +1
                s.Bars = true;
                steps.Add(new() { ["t"] = "bars", ["on"] = true, ["lmax"] = s.LMax, ["dmax"] = s.DMax, ["who"] = vane ? "Vane" : "Helena" });
                steps.Add(new() { ["t"] = "bar", ["l"] = s.Var("lung_lay"), ["d"] = s.Var("dao_dong") });
                break;
            }
            case "resolve_helena":
            {
                OpenShards(s, new[] { 6 }, steps);   // Mảnh 06 mở trước ở mọi cách kết, kể cả kết cục sớm
                int dd = s.Var("dao_dong"), ll = s.Var("lung_lay");
                string ket;
                if (dd >= 5 && s.Flag("lua_chon_0") == "A" && s.Flag("lua_chon_1") == "A" && s.Flag("lua_chon_2") == "A") ket = "ket_cuc_som";
                else if (ll >= 6) ket = "thuyet_phuc";
                else if (dd >= 5) ket = "bi_thuyet_phuc";
                else ket = "bat_phan";
                s.Flags["ket_doi_chat_helena"] = ket;
                s.Flags["giay_to"] = s.GiayTo;
                EndBattle(s, "Helena_Neutral", steps);
                break;
            }
            case "resolve_vane":
            {
                OpenShards(s, new[] { 14, 15, 16 }, steps);   // một thông báo cho cả ba mảnh
                int dd = s.Var("dao_dong"), ll = s.Var("lung_lay");
                string v;
                if (dd >= 6 && s.Flag("lua_chon_5") == "A") v = "ket_cuc_som";
                else if (ll >= 6) v = "dong_minh";
                else if (dd >= 6) v = "chu";
                else v = "dung_ngoai";
                s.Flags["vane"] = v;
                EndBattle(s, "Vane_Neutral", steps);
                break;
            }
            default: throw new InvalidOperationException("Lệnh đặc biệt lạ: " + sp.Name);
        }
    }

    static void EndBattle(GameState s, string sprite, List<Dictionary<string, object?>> steps)
    {
        s.Bars = false;
        steps.Add(new() { ["t"] = "bars", ["on"] = false });
        s.Sprite = sprite;   // về sprite thường ở đoạn kết
        steps.Add(new() { ["t"] = "spr", ["v"] = sprite });
    }

    // ───────────────────────────────────────── lựa chọn hiển thị

    static List<ChoiceOption> VisibleChoices(AskChoiceI a, GameState s) =>
        a.Options.Where(o => o.Cond == null || o.Cond.Eval(s)).ToList();

    /// <summary>Đáp án đối chất người chơi được thấy. Đáp án [C] có bản mạnh (cần ghi chú) và bản yếu: chỉ hiện một bản.
    /// [Đòn] ẩn hẳn nếu thiếu ghi chú. Hai bản cùng đủ điều kiện thì lấy bản đứng trước trong kịch bản.</summary>
    public static List<BattleOption> Eligible(AskBattleI a, GameState s)
    {
        var keep = new HashSet<BattleOption>();
        foreach (var grp in a.Options.GroupBy(o => o.Key))
        {
            var list = grp.ToList();
            bool hasWeak = list.Any(o => o.Variant == "yếu");
            var ok = list.Where(o => o.RequiresNote == null || s.HasNote(o.RequiresNote)).ToList();
            if (list.All(o => o.RequiresNote == null && o.Variant == null)) { foreach (var o in list) keep.Add(o); continue; }
            var strong = ok.FirstOrDefault(o => o.RequiresNote != null);
            if (strong != null) keep.Add(strong);
            else if (hasWeak) keep.Add(list.First(o => o.Variant == "yếu"));
            // [Đòn] không có ghi chú: không hiện gì
        }
        return a.Options.Where(keep.Contains).ToList();
    }

    // ───────────────────────────────────────── dữ liệu gửi cho trình duyệt

    Dictionary<string, object?> SceneOf(GameState s) => new()
    {
        ["bg"] = s.Bg, ["bgm"] = s.Bgm, ["sprite"] = s.Sprite, ["elev"] = s.Elev,
        ["bars"] = s.Bars, ["lmax"] = s.LMax, ["dmax"] = s.DMax,
        ["who"] = s.Chapter < lib.Chapters.Count && lib.Chapters[s.Chapter].BattleKind == "vane" ? "Vane" : "Helena",
        ["l"] = s.Var("lung_lay"), ["d"] = s.Var("dao_dong"),
        ["chapter"] = s.Chapter < lib.Chapters.Count ? lib.Chapters[s.Chapter].Title : "",
    };

    Dictionary<string, object?> NotebookOf(GameState s) => new()
    {
        ["items"] = s.Items.Select(i => new Dictionary<string, object?> { ["name"] = i.Name, ["desc"] = i.Desc }).ToList(),
        ["notes"] = s.Notes,
        ["shards"] = s.Shards.Select(n => ShardView(n, s)).ToList(),
    };

    static readonly (string head, string var)[] StanceHeads =
    {
        ("Duy tâm", "duy_tam"), ("Duy vật máy móc", "duy_vat_may_moc"), ("Bất khả tri", "bat_kha_tri"),
        ("Siêu hình", "sieu_hinh"), ("Duy vật biện chứng", "duy_vat_bien_chung"),
    };

    Dictionary<string, object?> ShardView(int n, GameState s)
    {
        if (!lib.Shards.TryGetValue(n, out var def))
            return new() { ["n"] = n, ["title"] = "Mảnh " + n.ToString("00"), ["paras"] = new List<object>() };

        var paras = new List<Dictionary<string, object?>>();
        if (def.Paras.Any(p => p.Contains("[số]")))
        {
            // Mảnh có số liệu động (Mảnh 06): thay [số], làm nổi mục nhiều nhất (hoặc tất cả các mục bằng nhau ở mức cao nhất)
            var counts = StanceHeads.ToDictionary(h => h.head, h => s.Var(h.var));
            int max = counts.Values.Max();
            bool? hi = null; bool inGroup = false;
            foreach (var p in def.Paras)
            {
                var hm = Regex.Match(p, @"^(.+?) · bạn đã chọn: \[số\] lần$");
                if (hm.Success && counts.TryGetValue(hm.Groups[1].Value, out var c))
                {
                    hi = max > 0 && c == max; inGroup = true;
                    paras.Add(new() { ["kind"] = "head", ["text"] = p.Replace("[số]", c.ToString()), ["hi"] = hi });
                }
                else if (p.StartsWith("Muốn đọc thêm")) { inGroup = false; paras.Add(new() { ["kind"] = "foot", ["text"] = p }); }
                else paras.Add(new() { ["kind"] = "p", ["text"] = p, ["hi"] = inGroup ? hi : null });
            }
        }
        else
        {
            foreach (var p in def.Paras)
                paras.Add(new() { ["kind"] = p.StartsWith("Muốn đọc thêm") ? "foot" : "p", ["text"] = p });
        }
        return new() { ["n"] = n, ["title"] = def.Title, ["paras"] = paras };
    }
}
