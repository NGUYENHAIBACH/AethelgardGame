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
    public static GameState Deser(string json)
    {
        var s = JsonSerializer.Deserialize<GameState>(json, J) ?? new GameState();
        s.UpgradeLegacy();
        return s;
    }
    static GameState Clone(GameState s) => Deser(Ser(s));

    public GameState NewGame() => new();

    // ───────────────────────────────────────── API

    public Batch Start() => Run(NewGame());

    public Batch Resume(GameState s) => Run(s);

    public Batch Continue(GameState s)
    {
        if (CurrentInstr(s) is ChapterEndI && s.Chapter + 1 < lib.Chapters.Count)
        {
            s.Chapter++; s.Pc = 0;
            s.ClearStage(); s.Bgm = null;   // sang chương mới: gỡ hết, tắt nhạc
        }
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
        else if (ins is AskNoteI an) return Run(s, ChooseNote(s, an, index));
        else if (ins is AskLinkI al) return Run(s, ChooseLink(s, al, index));
        else if (ins is AskCallI call)
        {
            if (index < 0 || index >= call.Names.Count) throw new ArgumentOutOfRangeException(nameof(index));
            var name = call.Names[index];
            s.Flags["goi"] = name;
            // chỉ dùng ở đoạn "Thắng": người thắng dù gọi sai ở Câu 5 (HUONG_DAN_DEV_CHUONG_5.md mục 3)
            if (call.Cau == 5) s.Flags["sai_cau_5"] = name == call.Correct ? "khong" : "co";
            s.Pc++;
        }
        else throw new InvalidOperationException("Trạng thái không đang chờ lựa chọn.");
        return Run(s);
    }

    // ───────────────────────────────────────── Chương 3: màn xếp sổ và màn gắn lời kể

    const string NoneText = "Không có gì", NotYetText = "Chưa gắn được vào đâu", NoMatchText = "Không khớp với thứ mình đang giữ";

    static bool Holds(GameState s, PickAnswer a) => a.Pseudo != null ? a.Pseudo.Eval(s) : s.HasNote(a.Key) || s.HasItem(a.Key);

    /// <summary>Danh sách chọn của màn xếp sổ: mọi ghi chú và vật phẩm đang có, dòng tự thêm (nếu có), rồi "Không có gì".</summary>
    public static List<string> NoteOptions(AskNoteI a, GameState s)
    {
        var l = s.Notes.Concat(s.Items.Select(x => x.Name)).ToList();
        l.AddRange(a.Answers.Where(x => x.Pseudo != null && x.Pseudo.Eval(s)).Select(x => x.Key));
        l.Add(NoneText);
        return l;
    }

    List<Dictionary<string, object?>> VeritasSays(GameState s, params (string who, string text)[] lines)
    {
        var pre = new List<Dictionary<string, object?>>();
        foreach (var (who, text) in lines)
        {
            if (text.Length == 0) continue;
            // bản trên: hai người đang ngồi giữa xưởng lưu trữ, chỉ thì thầm
            var say = new SayI { Kind = SayKind.Say, Name = who, Text = text, Whisper = s.Flag("ban") == "tren" };
            var sig = StageSig(s);
            var lit = Speak(s, say);
            if (StageSig(s) != sig) pre.Add(StageStep(s));
            var d = SayStep(say, s);
            if (lit != null) d["sp"] = lit;
            pre.Add(d);
        }
        return pre;
    }

    List<Dictionary<string, object?>>? ChooseNote(GameState s, AskNoteI a, int index)
    {
        var opts = NoteOptions(a, s);
        if (index < 0 || index >= opts.Count) throw new ArgumentOutOfRangeException(nameof(index));
        var pick = opts[index];
        string mx = "mat_xich_" + a.Slot;
        if (pick == NoneText)
        {
            bool holds = a.Answers.Any(x => Holds(s, x));
            // mắt xích 1 và 2 không bỏ trống được; mắt xích khác: đang giữ thứ khớp thì Veritas nhắc một lần
            if (a.NoneAddr < 0 || (holds && s.Flag("nhac_" + a.Slot) != "co"))
            {
                if (a.NoneAddr >= 0) s.Flags["nhac_" + a.Slot] = "co";
                return VeritasSays(s, ("Veritas", a.Remind));
            }
            s.Flags[mx] = "khong";
            s.Pc = a.NoneAddr;
            return null;
        }
        var ans = a.Answers.FirstOrDefault(x => x.Pseudo != null ? pick == x.Key : pick.StartsWith(x.Key, StringComparison.Ordinal));
        if (ans == null)
        {
            int k = s.Var("chon_sai"); s.Add("chon_sai", 1);
            return VeritasSays(s, ("Veritas", a.Wrong.Count > 0 ? a.Wrong[k % a.Wrong.Count] : ""));
        }
        s.Flags[mx] = "co";
        s.Add("so_cai", 1);
        s.Pc = ans.Addr < 0 ? s.Pc + 1 : ans.Addr;
        return null;
    }

    /// <summary>Danh sách chọn của màn gắn: các mắt xích của Cảnh 2 (5 và 6 chỉ hiện khi có; 3 và 4 bỏ trống thì mờ), rồi hai nút.</summary>
    public static List<(string text, int link, bool dim)> LinkOptions(AskLinkI a, GameState s)
    {
        var l = new List<(string, int, bool)>();
        for (int n = 1; n <= a.Links.Count; n++)
        {
            bool joined = s.Flag("mat_xich_" + n) == "co";
            if (n >= 5 && !joined) continue;
            l.Add((joined ? a.Links[n - 1] : a.Links[n - 1] + " (còn trống)", n, !joined));
        }
        l.Add((NotYetText, 0, false));
        l.Add((NoMatchText, -1, false));
        return l;
    }

    List<Dictionary<string, object?>>? ChooseLink(GameState s, AskLinkI a, int index)
    {
        var opts = LinkOptions(a, s);
        if (index < 0 || index >= opts.Count) throw new ArgumentOutOfRangeException(nameof(index));
        var (_, link, dim) = opts[index];
        bool mx3 = s.Flag("mat_xich_3") == "co", mx4 = s.Flag("mat_xich_4") == "co";
        string? result = null;      // giá trị của dieu_N khi đáp đúng
        switch (a.Slot)
        {
            case 1:   // chỉ chọn một lần: bác ngay hay không
                s.Flags["bac_dung"] = link == -1 ? "co" : "khong";
                s.Pc++; return null;
            case 2: if (link == 2) result = "2"; break;
            case 3: if (mx3 ? link == 3 : link == 0) result = mx3 ? "gan" : "treo"; break;
            case 4: if (link == 1 || (link == 4 && mx4)) result = link.ToString(); break;
            case 5: if (link == 0) result = "treo"; else if (link == 2) result = "2"; break;
        }
        if (result != null) { s.Flags["dieu_" + a.Slot] = result; s.Pc++; return null; }

        if (link == -1) return VeritasSays(s, ("Veritas", a.NoMatchV), ("Kael", a.NoMatchK));
        if (link > 0 && dim) return VeritasSays(s, ("Veritas", a.Empty));
        if (link == 0) return VeritasSays(s, ("Veritas", a.HasPlace));
        int k = s.Var("gan_sai"); s.Add("gan_sai", 1);
        return VeritasSays(s, ("Veritas", a.Wrong.Count > 0 ? a.Wrong[k % a.Wrong.Count] : ""));
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

    public Batch Run(GameState s, List<Dictionary<string, object?>>? pre = null)
    {
        lib.Refresh();
        var b = new Batch { StartState = Ser(s) };
        b.Scene = SceneOf(s);
        b.Notebook = NotebookOf(s);
        var steps = b.Steps;
        if (pre != null) steps.AddRange(pre);
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
                {
                    var sig = StageSig(s);
                    var lit = Speak(s, say);
                    if (StageSig(s) != sig) steps.Add(StageStep(s));
                    var d = SayStep(say, s);
                    if (lit != null) d["sp"] = lit;
                    steps.Add(d); s.Pc++; break;
                }

                case CmdI cmd:
                {
                    var sig = StageSig(s);
                    Cmd(s, cmd, steps);
                    if (StageSig(s) != sig) steps.Add(StageStep(s));
                    s.Pc++; break;
                }

                case AddItemI it:
                    if (s.Items.All(x => x.Name != it.Name))
                    {
                        s.Items.Add(new ItemRec { Name = it.Name, Desc = it.Desc });
                        steps.Add(new() { ["t"] = "item", ["name"] = it.Name, ["desc"] = it.Desc });
                    }
                    s.Pc++; break;

                case AddNoteI n:
                    AddNote(s, n.Name, steps); s.Pc++; break;

                case RemoveItemI ri:
                    if (s.Items.RemoveAll(x => x.Name.StartsWith(ri.Name, StringComparison.Ordinal)) > 0)
                        steps.Add(new() { ["t"] = "unitem", ["name"] = ri.Name });
                    s.Pc++; break;

                case GotoI go:
                    if (go.ClearStack) s.Ret.Clear();
                    s.Pc = go.Target; break;

                case CallI call:
                    s.Ret.Add(s.Pc + 1); s.Pc = call.Target; break;

                case ReturnI:
                    if (s.Ret.Count > 0) { s.Pc = s.Ret[^1]; s.Ret.RemoveAt(s.Ret.Count - 1); }
                    else s.Pc++;
                    break;

                case CheckpointI cp:
                {
                    var snap = Clone(s); snap.Checkpoints = new();
                    s.Checkpoints[cp.Id.ToString()] = Ser(snap);
                    // trận cuối bắt đầu: hiện thanh điểm và ba chốt (người viết yêu cầu 08/10/2026)
                    if (cp.Id == Compiler.BattleCheckpoint) { s.Duel = true; steps.Add(DuelStep(s)); }
                    s.Pc++; break;
                }

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
                {
                    var sig = StageSig(s);
                    Special(s, ch, sp, steps);
                    if (StageSig(s) != sig) steps.Add(StageStep(s));
                    s.Pc++; break;
                }

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

                case AskNoteI an:
                    b.Pause = new()
                    {
                        ["type"] = "pick", ["how"] = "note",
                        ["options"] = NoteOptions(an, s).Select((o, k) => new Dictionary<string, object?> { ["i"] = k, ["text"] = o }).ToList(),
                    };
                    goto done;

                case AskLinkI al:
                    b.Pause = new()
                    {
                        ["type"] = "pick", ["how"] = "link",
                        ["options"] = LinkOptions(al, s).Select((o, k) => new Dictionary<string, object?> { ["i"] = k, ["text"] = o.text, ["dim"] = o.dim }).ToList(),
                    };
                    goto done;

                case AskCallI ask:
                    b.Pause = new()
                    {
                        ["type"] = "call",
                        ["options"] = ask.Names.Select((o, k) => new Dictionary<string, object?>
                        {
                            ["i"] = k, ["text"] = o, ["hint"] = CallHints.GetValueOrDefault(o),
                        }).ToList(),
                    };
                    goto done;

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
                    if (s.Stage.Count > 0 || s.Holo != null || s.Shadow != null) { s.ClearStage(); steps.Add(StageStep(s)); }
                    b.Pause = new()
                    {
                        ["type"] = "end", ["title"] = es.Title, ["lesson"] = es.Lesson,
                        ["retry"] = RetryIds(s, ch, es).Select(id => new Dictionary<string, object?>
                        {
                            ["id"] = id, ["label"] = id == Compiler.BattleCheckpoint ? "Đầu trận với A.L.I.C.E" : "Lựa chọn " + id,
                        }).ToList(),
                        ["trend"] = TrendBlock(s),
                        ["final"] = es.NoRetry ? FinalLayers(s) : null,
                        ["shards"] = s.Shards.Count, ["shardsTotal"] = lib.Shards.Count,
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

    /// <summary>Điểm quay lại ở màn kết cục, theo quyết định trong từng file hướng dẫn dev.</summary>
    static List<int> RetryIds(GameState s, Chapter ch, EndScreenI es)
    {
        List<int> ids;
        if (es.NoRetry) ids = new();                                                            // 7/7: không có nút quay lại
        else if (es.Title.StartsWith("KẾT CỤC 1/7")) ids = new() { 0, 1, 2 };                   // HUONG_DAN_DEV.md mục 12, điểm 9
        else if (ch.Code.Any(x => x is CheckpointI)) ids = new() { Compiler.BattleCheckpoint };   // Chương 5 mục 14, điểm 1: về đầu Câu 1
        else if (ch.Title.StartsWith("Chương 3") || ch.Title.StartsWith("Chương 4")) ids = new() { ch.ChoiceIds.Max() };   // về Lựa chọn 6 / Lựa chọn 9
        else ids = ch.ChoiceIds.ToList();
        return ids.Where(id => s.Checkpoints.ContainsKey(id.ToString())).ToList();
    }

    // ───────────────────────────────────────── khối "Lối bạn hay chọn" (HUONG_DAN_DEV_CHUONG_5.md mục 10)

    static readonly string[] Trends = { "lam_dung_lenh", "long_tin", "con_so", "ngon_lua", "kiem_chung" };

    static readonly Dictionary<string, string> TrendMain = new()
    {
        ["lam_dung_lenh"] = "Trên chặng đường này, bạn hay chọn làm cho xong việc được giao và để phần còn lại cho người có trách nhiệm. Việc được giao thường đúng. Chỗ hỏng là khi không ai còn hỏi việc ấy đang giữ cho cái gì đứng.",
        ["long_tin"] = "Trên chặng đường này, bạn hay chọn giữ cho người ta yên lòng trước đã. Lòng người yên thì ca vẫn chạy. Nhưng cái ống không đọc được lòng người: nó mòn theo cách của nó.",
        ["con_so"] = "Trên chặng đường này, bạn hay chọn theo phép tính: đủ hay thiếu, được hay mất bao nhiêu. Con số không nói dối. Nó chỉ không nói ai đã quyết phần nào về tay ai.",
        ["ngon_lua"] = "Trên chặng đường này, bạn hay chọn đứng về phía người đang chịu thiệt, và làm ngay. Không có cái nóng ấy thì không ai dừng tay. Nhưng dừng tay mới là nửa đầu; nửa sau là dựng cái gì vào chỗ ấy.",
        ["kiem_chung"] = "Trên chặng đường này, bạn hay chọn tự đi xem, tự đo, rồi mới nói. Điều bạn nói vì thế có thứ chống lưng. Nhưng biết đúng mới là một nửa; nửa kia là có bao nhiêu người cùng làm.",
    };

    static readonly Dictionary<string, string> TrendPair = new()
    {
        ["lam_dung_lenh+long_tin"] = "Bạn làm đúng việc, và tin người giao việc. Hai thứ ấy đỡ nhau rất êm, nên cũng khó thấy nhất lúc cả hai cùng sai.",
        ["con_so+lam_dung_lenh"] = "Bạn làm đúng việc và tính đủ số: một ca làm không ai chê được. Câu còn thiếu là việc ấy, con số ấy, do ai đặt ra.",
        ["lam_dung_lenh+ngon_lua"] = "Có lúc bạn làm theo, có lúc bạn gạt phăng. Đáng hỏi lại: ở chỗ nào thì bạn đổi, và cái gì làm bạn đổi.",
        ["kiem_chung+lam_dung_lenh"] = "Bạn làm việc được giao, nhưng có đi xem trước. Từ chỗ ấy tới chỗ hỏi lại chính cái lệnh chỉ còn một bước.",
        ["con_so+long_tin"] = "Bạn giữ lòng người và giữ sổ sách: một thành phố được giữ yên bằng đúng hai thứ ấy. Yên cho ai thì chưa có trong thứ nào.",
        ["long_tin+ngon_lua"] = "Bạn tin người, và nóng thay cho người. Cả hai đều bắt đầu từ tấm lòng; thứ cần thêm là một cái gì đo được.",
        ["kiem_chung+long_tin"] = "Bạn muốn người ta yên lòng, mà cũng muốn biết thật. Chỗ khó là lúc hai điều ấy không đi cùng nhau được, và bạn đã phải chọn.",
        ["con_so+ngon_lua"] = "Bạn tính ra ai đang thiệt, rồi đứng về phía họ. Phép tính cho biết đập vào đâu; nó chưa cho biết dựng lại bằng gì.",
        ["con_so+kiem_chung"] = "Bạn tin thứ đếm được, và tự đi đếm. Con số bạn có là số thật; việc còn lại là hỏi nó từ tay ai mà ra.",
        ["kiem_chung+ngon_lua"] = "Bạn biết đúng, và không ngồi yên. Chỉ còn thiếu một thứ: đủ người cùng làm.",
    };

    /// <summary>Đoạn chính của khuynh hướng cao nhất; thêm câu ghép nếu khuynh hướng thứ hai có từ 2 điểm. Hòa thì lấy cái cộng gần nhất.</summary>
    static List<string>? TrendBlock(GameState s)
    {
        var ranked = Trends.OrderByDescending(t => s.Var(t)).ThenByDescending(t => s.TrendOrder.LastIndexOf(t)).ToList();
        if (s.Var(ranked[0]) <= 0) return null;
        var l = new List<string> { TrendMain[ranked[0]] };
        if (s.Var(ranked[1]) >= 2)
        {
            var key = string.Join("+", new[] { ranked[0], ranked[1] }.OrderBy(x => x, StringComparer.Ordinal));
            if (TrendPair.TryGetValue(key, out var p)) l.Add(p);
        }
        return l;
    }

    /// <summary>Các lớp của hình cuối, theo thứ tự từ trong ra ngoài (HUONG_DAN_DEV_CHUONG_5.md mục 9).</summary>
    static List<string> FinalLayers(GameState s)
    {
        var l = new List<string> { "KT_Kael_Me", "KT_Rian", "KT_Tho_xuong_4" };
        if (s.Flag("tao_len") == "co") l.Add("KT_Tho_be_tao");
        if (s.Flag("vane_len") == "co") l.Add("KT_Vane");
        if (s.Var("tang_trung") >= 1 || s.Flag("nha_len") == "kip") l.Add("KT_Tang_Trung");
        if (s.Flag("giu_lo") == "kip") l.Add("KT_Doran");
        if (s.Flag("helena") == "dong_minh") l.Add("KT_Helena");
        return l;
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

    static Dictionary<string, object?> SayStep(SayI s, GameState st)
    {
        var d = new Dictionary<string, object?> { ["t"] = "say", ["kind"] = s.Kind.ToString().ToLowerInvariant(), ["text"] = s.Text };
        if (s.Name.Length > 0) d["name"] = s.Name;
        if (s.Loud) d["loud"] = true;
        if (s.Whisper || (s.WhisperTren && st.Flag("ban") == "tren")) d["whisper"] = true;
        if (s.WristTeal) d["teal"] = true;
        return d;
    }

    // ───────────────────────────────────────── sân khấu (HUONG_DAN_DEV_SAN_KHAU.md)

    /// <summary>Ô tên trong kịch bản → tiền tố file sprite. So nguyên chữ.</summary>
    public static readonly Dictionary<string, string> Actors = new()
    {
        ["Kael"] = "Kael", ["Helena"] = "Helena", ["Vane"] = "Vane", ["Rian"] = "Rian", ["Mẹ"] = "Me", ["Doran"] = "Doran",
        ["Soren"] = "Soren", ["Ilsa"] = "Ilsa",   // Chương 5: tự lên khi nói, như Vane
    };

    /// <summary>Người phụ có bóng: ô tên → file.</summary>
    public static readonly Dictionary<string, string> Shadows = new()
    {
        ["Thợ già"] = "Bong_Tho_gia", ["Thợ trẻ"] = "Bong_Tho_tre", ["Chị thợ"] = "Bong_Chi_tho",
        ["Người gác thang"] = "Bong_Nguoi_gac_thang", ["Thợ bể tảo"] = "Bong_Tho_be_tao", ["Chị thợ tảo"] = "Bong_Chi_tho_tao",
        // Chương 5 (HUONG_DAN_DEV_CHUONG_5.md mục 2)
        ["Cố vấn Corvin"] = "Bong_Co_van", ["Corvin"] = "Bong_Co_van", ["Người làm thuốc"] = "Bong_Nguoi_lam_thuoc",
        ["Thợ van"] = "Bong_Tho_van", ["Người nhà"] = "Bong_Nguoi_nha", ["Lính"] = "Bong_Linh",
        // người phụ chờ vẽ bóng (lời tả ở tai_nguyen_sua/prompt_nen.md bên kho truyện): chưa có file thì không hiện gì, có file là tự hiện
        ["Thư ký"] = "Bong_Thu_ky", ["Người bốc hàng"] = "Bong_Nguoi_boc_hang", ["Người dỡ hàng"] = "Bong_Nguoi_do_hang",
        ["Bà cụ"] = "Bong_Ba_cu", ["Người phát suất"] = "Bong_Nguoi_phat_suat", ["Thợ học việc"] = "Bong_Tho_hoc_viec",
        ["Người trong đám đông"] = "Bong_Dam_dong", ["Công nhân"] = "Bong_Dam_dong", ["Lính gác"] = "Bong_Linh_gac",
    };

    static string StageSig(GameState s) =>
        string.Join(",", s.Stage.Select(a => a.Name + "_" + a.Expr)) + "|" + s.Holo + "|" + s.Shadow;

    static Dictionary<string, object?> StageView(GameState s) => new()
    {
        ["v"] = s.Stage.Select(a => new Dictionary<string, object?> { ["n"] = a.Name, ["e"] = a.Expr }).ToList(),
        ["holo"] = s.Holo, ["shadow"] = s.Shadow,
    };

    static Dictionary<string, object?> StageStep(GameState s)
    {
        var d = StageView(s); d["t"] = "stage"; return d;
    }

    /// <summary>Người nói tự lên sân khấu; trả về thứ cần làm sáng (tiền tố sprite, "holo", "shadow") hoặc null nếu không ai bị làm tối.</summary>
    static string? Speak(GameState s, SayI say)
    {
        if (say.Kind is not (SayKind.Say or SayKind.Think)) return null;   // dẫn chuyện, dòng tả, loa: không đụng sân khấu
        if (Shadows.TryGetValue(say.Name, out var shade)) { s.Shadow = shade; return "shadow"; }
        s.Shadow = null;   // một người khác nói: bóng lùi đi
        if (Actors.TryGetValue(say.Name, out var who)) { s.Spoke(who); return who; }
        if (say.Name == "Veritas" && s.Holo != null) return "holo";
        if (say.Name == "A.L.I.C.E" && s.OnStage("ALICE") != null) return "ALICE";   // Chương 5 Cảnh 3: nó chỉ lên bằng thẻ
        return null;
    }

    // ───────────────────────────────────────── trận cuối "Gọi ai?"

    static Dictionary<string, object?> DuelView(GameState s) => new()
    {
        ["on"] = s.Duel, ["vung"] = Math.Clamp(s.Var("vung"), 0, 10), ["max"] = 10, ["lost"] = Math.Clamp(s.Var("mat_chot"), 0, 3),
    };

    static Dictionary<string, object?> DuelStep(GameState s) { var d = DuelView(s); d["t"] = "duel"; return d; }

    /// <summary>Lời giải thích hiện khi người chơi trỏ vào từng nút "Gọi ai?" (người viết yêu cầu 08/10/2026: trận quá khó đoán).
    /// Chữ do bên làm game soạn, chờ người viết sửa. Chỉ nói đó là ai và họ biết chuyện gì; không nói nút nào đúng.</summary>
    public static readonly Dictionary<string, string> CallHints = new()
    {
        ["Tầng Đáy"] = "Thợ đúc, thợ bể tảo, mẹ Kael: những người làm ra cái ăn và đồ sắt. Họ biết suất bị bớt mấy lần, cái hòm thuốc về lúc nào, và vì sao họ dừng tay.",
        ["Tầng Trung"] = "Thợ van, kỹ sư, mười hai nhà có người bỏng. Họ biết cái ống vỡ ra sao, và thứ gì đã bị xóa khỏi sổ sau hôm ấy.",
        ["Người ký và người gác"] = "Những người làm theo lời đề nghị của cái loa: người ký danh sách, người đứng gác, người xếp hòm. Họ biết ai mới là người ra tay.",
        ["Veritas"] = "Kho gốc của thành phố, hai trăm tuổi. Cô giữ con số của ngày xưa, thứ duy nhất đem ra so được với hôm nay.",
        ["Cái lô chiều nay"] = "Lô tệp vừa bị gắn nhãn xóa của chính hôm nay. Trong ấy là những gì mới được ghi lại từ sáng, chưa ai mở ra đọc.",
    };

    static (string name, string expr) SplitSprite(string v)
    {
        int k = v.IndexOf('_');
        return k <= 0 ? (v, "Neutral") : (v[..k], v[(k + 1)..]);
    }

    static void Cmd(GameState s, CmdI c, List<Dictionary<string, object?>> steps)
    {
        switch (c.Kind)
        {
            case CmdKind.Bg:
                if (s.Elev) { s.Elev = false; steps.Add(new() { ["t"] = "elev", ["on"] = false }); }
                s.ClearStage();   // thẻ [BG] nào cũng gỡ hết, kể cả khi trùng nền đang hiện
                if (s.Amb != null) { s.Amb = null; steps.Add(new() { ["t"] = "amb", ["v"] = null }); }
                s.Bg = c.Value; steps.Add(new() { ["t"] = "bg", ["v"] = c.Value }); break;
            case CmdKind.Bgm:
                s.Bgm = c.Value; steps.Add(new() { ["t"] = "bgm", ["v"] = c.Value }); break;
            case CmdKind.Se:
                // SE07_Mua là hiệu ứng chạy lặp: phát tới thẻ [BG] kế tiếp (HUONG_DAN_DEV_CHUONG_5.md mục 7)
                if (c.Value == "SE07_Mua") { s.Amb = c.Value; steps.Add(new() { ["t"] = "amb", ["v"] = c.Value }); }
                else steps.Add(new() { ["t"] = "se", ["v"] = c.Value });
                break;
            case CmdKind.Sprite:
            {
                var (n, e) = SplitSprite(c.Value!);
                if (n == "Veritas") s.HoloOn(e); else s.Enter(n, e);
                break;
            }
            case CmdKind.SpriteOff:
                if (c.Value == null) s.ClearStage();
                else if (c.Value == "Veritas") s.Holo = null;
                else s.Leave(c.Value);
                break;
            case CmdKind.Expr:
            {
                var (n, e) = SplitSprite(c.Value!);
                s.SetExpr(n, e); break;
            }
            case CmdKind.SceneStart:
                s.ClearStage(); break;
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
        if (e.SetFlagValue != null)
        {
            s.Flags[e.Var] = e.SetFlagValue;
            if (e.Var == "tran" && s.Duel) { s.Duel = false; steps.Add(DuelStep(s)); }   // hết trận: cất thanh điểm
            return;
        }
        s.Add(e.Var, e.Delta);
        if (s.Duel && e.Var is "vung" or "mat_chot") steps.Add(DuelStep(s));
        if (e.Delta > 0 && Trends.Contains(e.Var)) { s.TrendOrder.Remove(e.Var); s.TrendOrder.Add(e.Var); }
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
        // Chương 3 mục 9: tấm nhãn Kael giữ ở bản trên có thêm dãy số sau đoạn "Tấm nhãn"
        if (name.StartsWith("Tệp 4406-16"))
            foreach (var it in s.Items.Where(x => x.Name == "Nhãn hòm thuốc" && !x.Desc.Contains("4406-16"))) it.Desc = (it.Desc + " Dãy số: 4406-16.").Trim();
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
                EndBattle(s, "Helena", steps);
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
                EndBattle(s, "Vane", steps);
                break;
            }
            case "init_c3":
            {
                // HUONG_DAN_DEV_CHUONG_3.md mục 3: nơi Kael đứng và người đang cầm anh khi vào Chương 3
                bool lose = s.Flag("vane") is "chu" or "ket_cuc_som";
                s.Flags["ban"] = s.BanTren ? "tren" : "duoi";
                s.Flags["loi_vao"] = lose ? "D3" : s.GiayTo switch
                {
                    "thong_hanh" => "T1", "dac_phai" => "T2",
                    _ => s.Flag("vane") == "dong_minh" ? "D1" : "D2",
                };
                s.Flags["thang_vane"] = s.Flag("vane") == "dong_minh" ? "co" : "khong";   // kết trận Vane ở Chương 2 (Chương 5 đọc lại)
                break;
            }
            case "init_c4":
                // HUONG_DAN_DEV_CHUONG_4.md mục 3: biến helena, để Chương 5 dùng
                s.Flags["helena"] = s.Flag("ket_doi_chat_helena") switch { "thuyet_phuc" => "dong_minh", "bat_phan" => "dung_ngoai", _ => "chu" };
                break;
            case "c4_scene3":
                s.Flags["lo_truoc_c3"] = s.Flag("lo") == "co" ? "co" : "khong";
                break;
            case "resolve_veritas":
                s.Flags["veritas"] = s.Flag("bac_dung") == "co" && s.Flag("dieu_3") == "gan" ? "dong_hanh" : "do_du";
                break;
            default: throw new InvalidOperationException("Lệnh đặc biệt lạ: " + sp.Name);
        }
    }

    static void EndBattle(GameState s, string who, List<Dictionary<string, object?>> steps)
    {
        s.Bars = false;
        steps.Add(new() { ["t"] = "bars", ["on"] = false });
        s.SetExpr(who, "Neutral");   // về biểu cảm thường ở đoạn kết, không đưa ai lên
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
        ["bg"] = s.Bg, ["bgm"] = s.Bgm, ["amb"] = s.Amb, ["duel"] = DuelView(s), ["stage"] = StageView(s), ["elev"] = s.Elev,
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
