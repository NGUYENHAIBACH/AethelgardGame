using System.Text.RegularExpressions;

namespace AethelgardGame.Models.Story;

/// <summary>Biên dịch một file kịch bản .md thành danh sách lệnh phẳng.
/// Quy ước đọc kịch bản lấy theo HUONG_DAN_DEV.md (mục 2) và HUONG_DAN_DEV_CHUONG_2.md (mục 3).
/// Chữ trong kịch bản được giữ nguyên; thứ gì không hiểu thì ghi vào Warnings chứ không tự bịa.</summary>
public sealed class Compiler
{
    readonly List<Block> B;
    readonly Chapter ch;
    readonly List<Instr> code;
    readonly List<string> warn;
    readonly Dictionary<int, ShardDef> shards;
    int i;
    bool defRegion, endEmitted, stanceOk;

    Compiler(List<Block> blocks, Chapter chapter, List<string> warnings, Dictionary<int, ShardDef> shardDefs)
    {
        B = blocks; ch = chapter; code = chapter.Code; warn = warnings; shards = shardDefs;
    }

    public static Chapter Compile(string file, int index, string text, List<string> warnings, Dictionary<int, ShardDef> shardDefs)
    {
        var ch = new Chapter { File = file, Index = index };
        var blocks = BlockReader.Read(text.Replace("\r\n", "\n").Split('\n'));
        var c = new Compiler(blocks, ch, warnings, shardDefs);
        c.Run();
        return ch;
    }

    void Warn(int line, string msg) => warn.Add($"{ch.File}:{line}: {msg}");

    // ──────────────────────────────────────────── vòng chính

    void Run()
    {
        while (i < B.Count)
        {
            var b = B[i];
            if (defRegion) { DefRegion(); continue; }
            switch (b.Type)
            {
                case BT.H1: H1(b); i++; break;
                case BT.H2: H2(b); break;
                case BT.H3: H3(b); i++; break;
                case BT.Bold:
                {
                    var c = BoldCond(b);
                    bool isBranch = b.Text.StartsWith("Nhánh ");
                    if (c == null) CloseScope();                              // nhãn không điều kiện: các nhánh chạy chung từ đây
                    else if (isBranch || !InBranch) OpenScope(c, 1, isBranch);
                    else OpenScope(c, 2);                                      // điều kiện con bên trong một nhánh
                    i++; break;
                }
                case BT.Bullet: Bullets(); break;
                case BT.Quote: QuoteBlock(b); i++; break;
                case BT.Plain: Plain(b); break;
            }
        }
        CloseScope();
        if (!endEmitted) { code.Add(new ChapterEndI()); endEmitted = true; }
        FillItemFallbacks();
    }

    // Mô tả vật phẩm mà kịch bản chưa viết nhưng người viết đã chốt trong hướng dẫn dev (Chương 2, mục 14, điểm 8).
    static readonly Dictionary<string, string> ItemDescFallback = new()
    {
        ["Nhãn hòm thuốc"] = "Một dãy số và một cái ngày. Không có tên.",
    };

    void FillItemFallbacks()
    {
        foreach (var it in code.OfType<AddItemI>())
            if (it.Desc.Length == 0 && ItemDescFallback.TryGetValue(it.Name, out var d)) it.Desc = d;
    }

    // ──────────────────────────────────────────── phạm vi điều kiện của tiêu đề

    // Phạm vi điều kiện của tiêu đề. Cấp 1: "Nhánh X", "Nếu ..." đứng một mình, tiêu đề chữ thường.
    // Cấp 2: nhãn "**Nếu ...**" nằm TRONG một "**Nhánh X**" (vd. Nhánh 4-B, rồi bên trong Nếu 1-B hoặc 1-C).
    readonly List<(int jump, bool branch, int level)> scopes = new();

    bool InBranch => scopes.Any(s => s.branch);

    void OpenScope(Cond c, int level = 1, bool branch = false)
    {
        CloseTo(level);
        code.Add(new JumpIfNotI { Cond = c, Target = -1 });
        scopes.Add((code.Count - 1, branch, level));
    }

    void CloseTo(int level)
    {
        while (scopes.Count > 0 && scopes[^1].level >= level)
        {
            ((JumpIfNotI)code[scopes[^1].jump]).Target = code.Count;
            scopes.RemoveAt(scopes.Count - 1);
        }
    }

    void CloseScope() => CloseTo(1);

    Cond? ParseCond(string text, int line)
    {
        var c = Cond.Parse(text, out var err);
        if (c == null) Warn(line, "điều kiện không hiểu: " + err);
        return c;
    }

    Cond? BoldCond(Block b)
    {
        var t = b.Text;
        if (t.StartsWith("Nếu ")) return ParseCond(t, b.Line);
        if (t.StartsWith("Nhánh "))
        {
            var m = Regex.Match(t, @"\d-[ABC]");
            return m.Success ? ParseCond(m.Value, b.Line) : null;
        }
        if (t.StartsWith("Một câu của Rian"))
            return new Cond.Fn("một câu của Rian", s =>
                s.Flag("da_dung_don_cau_4") == "co" && s.Flag("lua_chon_4") != "B" &&
                s.Flag("vane") is "dong_minh" or "dung_ngoai");
        return null; // nhãn không điều kiện: "Nhập lại (cả ba nhánh)", "Cửa thang (chung...)", ...
    }

    // ──────────────────────────────────────────── tiêu đề

    void H1(Block b)
    {
        if (b.Text.StartsWith("Mảnh lưu trữ"))
        {
            CloseScope();
            if (!endEmitted) { code.Add(new ChapterEndI()); endEmitted = true; }
            defRegion = true;
            return;
        }
        ch.Title = b.Text;
        code.Add(new CmdI { Kind = CmdKind.Card, Value = b.Text, Line = b.Line });
    }

    void H2(Block b)
    {
        var t = b.Text;
        CloseScope();
        if (t.StartsWith("Cảnh"))
        {
            if (t.Contains("Đối chất với Helena")) { ch.BattleKind = "helena"; ch.StanceCounting = true; }
            else if (t.Contains("Đối chất với Vane")) { ch.BattleKind = "vane"; }
            code.Add(new CmdI { Kind = CmdKind.SceneStart, Line = b.Line });
            i++; return;
        }
        var mc = Regex.Match(t, @"^Lựa chọn (\d+)");
        if (mc.Success) { Choice(int.Parse(mc.Groups[1].Value), b); return; }
        var ms = Regex.Match(t, @"^Mảnh lưu trữ (\d+): ""(.+)""");
        if (ms.Success)
        {
            var def = new ShardDef { Number = int.Parse(ms.Groups[1].Value), Title = ms.Groups[2].Value };
            i++;
            while (i < B.Count && B[i].Type == BT.Plain && B[i].Text.StartsWith("(")) i++;   // "(Biểu tượng cổ tay nhấp nháy...)"
            if (i < B.Count && B[i].Type == BT.Quote) { def.Paras = B[i].Paras; i++; }
            shards[def.Number] = def;
            code.Add(new OpenShardI { Numbers = { def.Number }, Line = b.Line });
            return;
        }
        if (t.StartsWith("Bốn cách kết thúc")) { code.Add(new SpecialI { Name = "resolve_helena", Line = b.Line }); i++; return; }
        if (t.StartsWith("Ba cách kết thúc")) { code.Add(new SpecialI { Name = "resolve_vane", Line = b.Line }); i++; return; }
        Warn(b.Line, "tiêu đề ## không hiểu: " + t);
        i++;
    }

    void H3(Block b)
    {
        var t = b.Text;
        CloseScope();
        if (t.StartsWith("Câu "))
        {
            var spr = ch.BattleKind == "vane" ? "Vane_Neutral" : "Helena_Neutral";   // HUONG_DAN_DEV mục 7: về sprite thường khi bắt đầu câu mới
            code.Add(new CmdI { Kind = CmdKind.Sprite, Value = spr, Line = b.Line });
            return;
        }
        if (t.StartsWith("Đoạn")) return;
        if (t.StartsWith("Kết cục sớm"))
        {
            var c = ch.BattleKind == "vane" ? Cond.FlagIs("vane", "ket_cuc_som") : Cond.FlagIs("ket_doi_chat_helena", "ket_cuc_som");
            OpenScope(c);
            return;
        }
        Warn(b.Line, "tiêu đề ### không hiểu: " + t);
    }

    // ──────────────────────────────────────────── vùng định nghĩa mảnh cuối file

    void DefRegion()
    {
        var b = B[i];
        if (b.Type == BT.Plain)
        {
            var m = Regex.Match(b.Text, @"^Mảnh (\d+): ""(.+?)""");
            if (m.Success)
            {
                var def = new ShardDef { Number = int.Parse(m.Groups[1].Value), Title = m.Groups[2].Value };
                i++;
                if (i < B.Count && B[i].Type == BT.Quote) { def.Paras = B[i].Paras; i++; }
                shards[def.Number] = def;
                return;
            }
        }
        i++;   // mọi thứ khác trong vùng này là ghi chú soạn thảo
    }

    // ──────────────────────────────────────────── dòng thường

    static readonly string[] SkipPrefixes =
    {
        "(Ghi chú cho người làm game", "(Ba dòng", "(Một câu riêng", "(Không có ghi chú nào", "(Từ đây câu hỏi",
        "(Chỉ đòn", "(Chung cho cả", "(Hết Cảnh ", "(Hết Chương ", "(Viết dần", "(Lập trường của", "(Biểu tượng cổ tay nhấp nháy. Người chơi",
        "(Từ đây ô tên", "Bản 2,", "Bản 3", "Mỗi nhánh mở", "Năm câu hỏi,", "Cảnh nối liền", "Lời của ba lựa chọn", "Lượt hai:",
    };

    static readonly Regex OptRe = new(
        @"^\[(?<k>A|B|C|Lùi|Giữ lời|Đòn)\](?:,\s*bản (?<v>mạnh|yếu))?(?:,\s*cần ghi chú ""(?<n>[^""]+)"")?\s*""(?<t>.*)""\s*$");

    void Plain(Block b)
    {
        var t = b.Text;

        // tiêu đề dạng chữ thường: điều kiện hoặc cách kết
        if (t.StartsWith("Nếu đã chọn "))
        {
            CloseScope(); var c = ParseCond(t, b.Line); if (c != null) OpenScope(c); i++; return;
        }
        var oc = OutcomeHeading(t);
        if (oc != null) { CloseScope(); OpenScope(oc); i++; return; }

        if (OptRe.IsMatch(t)) { OptionGroupTop(); return; }

        if (t.StartsWith("(Hai thanh hiện lên"))
        {
            code.Add(new SpecialI { Name = "bars_on", Line = b.Line }); i++; return;
        }
        if (SkipPrefixes.Any(p => t.StartsWith(p, StringComparison.Ordinal))) { ch.Skipped.Add($"{b.Line}: {t}"); i++; return; }

        if (t.StartsWith("━━"))
        {
            var m = Regex.Match(t, @"━━\s*(.+?)\s*━━");
            code.Add(new EndScreenI { Title = m.Success ? m.Groups[1].Value : t, Line = b.Line });
            i++; return;
        }

        // dòng "→ ..." đứng riêng là kết quả của cả nhánh, không thuộc điều kiện con vừa đóng
        if (t.StartsWith("→")) CloseTo(2);

        Content(t, b.Line);
        i++;
    }

    Cond? OutcomeHeading(string t)
    {
        bool vane = ch.BattleKind == "vane";
        if (t.StartsWith("Thuyết phục được Helena")) return Cond.FlagIs("ket_doi_chat_helena", "thuyet_phuc");
        if (t.StartsWith("Thuyết phục được Vane")) return Cond.FlagIs("vane", "dong_minh");
        if (t.StartsWith("Bất phân (không bên nào đủ điểm)"))
            return vane ? Cond.FlagIs("vane", "dung_ngoai") : Cond.FlagIs("ket_doi_chat_helena", "bat_phan");
        if (t.StartsWith("Kael bị thuyết phục (Dao động"))
            return vane ? Cond.FlagIn("vane", "chu", "ket_cuc_som") : Cond.FlagIs("ket_doi_chat_helena", "bi_thuyet_phuc");
        if (t.StartsWith("Kết cục sớm: "))
            return vane ? Cond.FlagIs("vane", "ket_cuc_som") : Cond.FlagIs("ket_doi_chat_helena", "ket_cuc_som");
        return null;
    }

    void QuoteBlock(Block q)
    {
        var last = code.Count > 0 ? code[^1] : null;
        switch (last)
        {
            case AddItemI it when it.Desc.Length == 0:
                it.Desc = string.Join("\n\n", q.Paras); break;
            case EndScreenI es:
                es.Lesson = q.Paras; break;
            default:
                // khối trích dẫn đứng giữa truyện = chữ viết phấn trên tấm bảng suất (Chương 2, nhánh 3-C)
                code.Add(new SayI { Kind = SayKind.Board, Text = string.Join("\n", q.Paras), Line = q.Line });
                break;
        }
    }

    // ──────────────────────────────────────────── dòng nội dung

    static readonly Regex EditorTag = new(@"\s*\[(?:GIEO|TRẢ|GẶT)[^\]]*\]");
    static readonly Regex SpeakerRe = new(
        @"^(?<name>[^\s\[\]()""][^:\[\]""]*?)(?:\s\((?<mod>[^)]*)\))?:\s(?<text>.+)$");

    /// <summary>Biên dịch một dòng nội dung (có thể gồm vài thẻ + ngoặc tròn + lời thoại) và thêm lệnh vào chương trình.</summary>
    void Content(string text, int line)
    {
        text = EditorTag.Replace(text, "").Trim();
        while (text.Length > 0)
        {
            if (text[0] == '[')
            {
                var mt = Regex.Match(text, @"^\[(BG|BGM|SE|SPRITE):\s*([^\]]+)\]\s*");
                if (mt.Success)
                {
                    var kind = mt.Groups[1].Value switch { "BG" => CmdKind.Bg, "BGM" => CmdKind.Bgm, "SE" => CmdKind.Se, _ => CmdKind.Sprite };
                    var val = mt.Groups[2].Value.Trim();
                    if (kind == CmdKind.Bgm && val == "tắt") val = null!;
                    code.Add(new CmdI { Kind = kind, Value = val, Line = line });
                    text = text[mt.Length..]; continue;
                }
                var mi = Regex.Match(text, @"^\[(VẬT PHẨM MỚI|GHI CHÚ MỚI):\s*(.+)\]\s*(?:\(.*\))?\s*$");
                if (mi.Success)
                {
                    var name = mi.Groups[2].Value.Trim();
                    if (mi.Groups[1].Value == "VẬT PHẨM MỚI") code.Add(new AddItemI { Name = name, Line = line });
                    else code.Add(new AddNoteI { Name = name, Line = line });
                    return;
                }
                var ms = Regex.Match(text, @"^\[([A-Za-z]+(?:_[A-Za-z]+)+)\]\s*");   // [Helena_Smile] đứng đầu dòng = đổi sprite
                if (ms.Success)
                {
                    code.Add(new CmdI { Kind = CmdKind.Sprite, Value = ms.Groups[1].Value, Line = line });
                    text = text[ms.Length..]; continue;
                }
                Warn(line, "thẻ không hiểu: " + text);
                return;
            }
            if (text[0] == '(')
            {
                var end = MatchParen(text);
                if (end < 0) { Warn(line, "ngoặc không đóng: " + text); return; }
                var inner = text[..(end + 1)];
                text = text[(end + 1)..].Trim();
                if (SkipPrefixes.Any(p => inner.StartsWith(p, StringComparison.Ordinal))) { ch.Skipped.Add($"{line}: {inner}"); continue; }
                Stage(inner, line);
                continue;
            }
            if (text[0] == '→')
            {
                var eff = Effects(text[1..].Trim(), line);
                if (eff != null) code.Add(eff);
                return;
            }
            Speech(text, line);
            return;
        }
    }

    static int MatchParen(string s)
    {
        int d = 0;
        for (int k = 0; k < s.Length; k++)
        {
            if (s[k] == '(') d++;
            else if (s[k] == ')') { d--; if (d == 0) return k; }
        }
        return -1;
    }

    void Stage(string t, int line)
    {
        // chỗ dev phải tự đặt theo hướng dẫn (không có thẻ trong kịch bản)
        if (t.StartsWith("(Nhạc tắt. Hình chiếu"))                 // Chương 2, Cảnh 3: tắt BGM05_Sad_Piano_3
            code.Add(new CmdI { Kind = CmdKind.Bgm, Value = null, Line = line });
        if (t.StartsWith("(Màn hình tối."))                         // Chương 2, thang xuống: màn hình đen, ô sáng nhỏ dần
            code.Add(new CmdI { Kind = CmdKind.Elevator, Value = "on", Line = line });

        code.Add(new SayI { Kind = SayKind.Stage, Text = t, WristTeal = t.Contains("nháy xanh ngọc"), Line = line });

        if (t.StartsWith("(Im lặng một lúc. Người thợ van"))       // Chương 1, Cảnh 2: Mảnh 02 mở ngay sau khi đám đông tan
            code.Add(new OpenShardI { Numbers = { 2 }, Line = line });
    }

    void Speech(string text, int line)
    {
        // hai lượt thoại trên một dòng: Kael: "..." / Mẹ: "..."
        var parts = Regex.Split(text, @"\s/\s(?=[^\s/""][^:/\[\]""]{0,30}(?:\s\([^)]*\))?:\s)");
        foreach (var part in parts)
        {
            var m = SpeakerRe.Match(part.Trim());
            if (!m.Success) { Warn(line, "dòng không hiểu: " + part); continue; }
            var name = m.Groups["name"].Value.Trim();
            var mod = m.Groups["mod"].Value.Trim();
            var body = Regex.Replace(m.Groups["text"].Value.Trim(), @"\s*\(lời hát tạm[^)]*\)", "").Trim();

            if (name == "Dẫn chuyện") { code.Add(new SayI { Kind = SayKind.Narr, Text = Unquote(body), Line = line }); continue; }
            var say = new SayI { Kind = SayKind.Say, Name = name, Text = Unquote(body), Line = line };
            if (mod.Contains("nghĩ")) say.Kind = SayKind.Think;
            else if (mod.Contains("loa")) { say.Kind = SayKind.Loa; say.HideSprite = true; }
            if (mod.Contains("rất khẽ")) { say.Whisper = true; say.HideSprite = true; }   // Veritas chỉ có tiếng
            if (mod.Contains("thì thầm")) say.Whisper = true;
            if (mod.Contains("to tiếng")) say.Loud = true;
            code.Add(say);
        }
    }

    static string Unquote(string s) =>
        s.Length >= 2 && s[0] == '"' && s[^1] == '"' && s.Count(c => c == '"') == 2 ? s[1..^1] : s;

    // ──────────────────────────────────────────── hiệu ứng "→ ..."

    static readonly (string vi, string var)[] VarNames =
    {
        ("Làm đúng lệnh", "lam_dung_lenh"), ("Kiểm chứng", "kiem_chung"), ("Lòng tin", "long_tin"),
        ("Tầng Trung", "tang_trung"), ("Tầng Đáy", "tang_day"), ("Con số", "con_so"), ("Ngọn lửa", "ngon_lua"),
        ("Dao động", "dao_dong"), ("Lung lay", "lung_lay"),
    };

    static readonly (string vi, string var)[] Stances =
    {
        ("duy tâm", "duy_tam"), ("duy vật máy móc", "duy_vat_may_moc"), ("bất khả tri", "bat_kha_tri"),
        ("hoài nghi", "bat_kha_tri"),   // người viết chốt: "hoài nghi" tính vào bất khả tri
        ("siêu hình", "sieu_hinh"), ("duy vật biện chứng", "duy_vat_bien_chung"),
    };

    static readonly Regex VarRe = new(@"(Làm đúng lệnh|Kiểm chứng|Lòng tin|Tầng Trung|Tầng Đáy|Con số|Ngọn lửa|Dao động|Lung lay)\s*([+\-−])\s*(\d+)");

    /// <summary>Phân tích phần sau dấu →. Dòng chỉ là lời bình của người viết thì trả về null.</summary>
    EffectI? Effects(string t, int line)
    {
        var e = new EffectI { Line = line };

        // "Làm đúng lệnh +1 (nếu cầm Thẻ đặc phái: Lòng tin +1)": nếu cầm thẻ thì cộng cái trong ngoặc THAY cho cái đầu
        var mr = Regex.Match(t, @"\(nếu cầm (?<g>[^:]+): (?<v>[^)]+)\)");
        Cond? when = null;
        if (mr.Success)
        {
            when = ParseCond("cầm " + mr.Groups["g"].Value.Trim(), line);
            foreach (Match m in VarRe.Matches(mr.Groups["v"].Value)) e.Effs.Add(MakeEff(m, when, null));
            t = t.Remove(mr.Index, mr.Length);
        }
        foreach (Match m in VarRe.Matches(t)) e.Effs.Add(MakeEff(m, null, when));

        var mo = Regex.Match(t, @"Mở ((?:Mảnh \d+(?:,\s*)?)+)");
        if (mo.Success)
            foreach (Match n in Regex.Matches(mo.Groups[1].Value, @"\d+")) e.Shards.Add(int.Parse(n.Value));

        var mn = Regex.Match(t, @"^Ghi chú ""([^""]+)"" \(đổi rian_da_nghe");
        if (mn.Success) e.NoteToAdd = mn.Groups[1].Value;

        if (stanceOk)
        {
            var ms = Regex.Match(t, @"Lập trường(?: của \[C\])?:\s*([^(.]+)");
            if (ms.Success)
            {
                var name = ms.Groups[1].Value.Trim();
                foreach (var (vi, v) in Stances)
                    if (name.StartsWith(vi, StringComparison.Ordinal)) { e.Effs.Add(new Eff { Var = v, Delta = 1 }); break; }
            }
        }
        return e.Effs.Count + e.Shards.Count > 0 || e.NoteToAdd != null ? e : null;
    }

    static Eff MakeEff(Match m, Cond? ifc, Cond? unless)
    {
        var vi = m.Groups[1].Value;
        var sign = m.Groups[2].Value == "+" ? 1 : -1;
        int n = int.Parse(m.Groups[3].Value) * sign;
        var v = VarNames.First(x => x.vi == vi).var;
        return new Eff { Var = v, Delta = n, If = ifc, Unless = unless };
    }

    // ──────────────────────────────────────────── gạch đầu dòng và điều kiện lồng nhau

    sealed class Node { public string Text = ""; public int Line; public List<Node> Kids = new(); }

    List<Node> TakeBulletTree()
    {
        var roots = new List<Node>();
        var stack = new List<(int indent, Node n)>();
        while (i < B.Count && B[i].Type == BT.Bullet)
        {
            var b = B[i++];
            var n = new Node { Text = b.Text, Line = b.Line };
            while (stack.Count > 0 && stack[^1].indent >= b.Indent) stack.RemoveAt(stack.Count - 1);
            if (stack.Count == 0) roots.Add(n); else stack[^1].n.Kids.Add(n);
            stack.Add((b.Indent, n));
        }
        return roots;
    }

    void Bullets()
    {
        var tree = TakeBulletTree();
        Nodes(tree);
    }

    static readonly Regex CondBullet = new(@"^Nếu (?<c>.+?)(?:\s\((?<n>[^()]*)\))?(?<sep>[.:])(?:\s+(?<rest>.*))?$");

    void Nodes(List<Node> nodes)
    {
        Cond? last = null;
        foreach (var n in nodes)
        {
            var t = n.Text;
            if (t.StartsWith("Nếu "))
            {
                var m = CondBullet.Match(t);
                if (!m.Success) { Warn(n.Line, "điều kiện không đọc được: " + t); continue; }
                var ctext = m.Groups["c"].Value.Trim();
                Cond? c;
                if (ctext == "không")
                {
                    if (last == null) { Warn(n.Line, "\"Nếu không\" mà không có điều kiện đứng trước"); continue; }
                    c = Cond.Not(last);
                }
                else
                {
                    var full = m.Groups["n"].Success ? ctext + " (" + m.Groups["n"].Value + ")" : ctext;
                    c = ParseCond(full, n.Line);
                    if (c == null) continue;
                    last = c;
                }
                var jump = new JumpIfNotI { Cond = c, Target = -1, Line = n.Line };
                code.Add(jump);
                var rest = m.Groups["rest"].Success ? m.Groups["rest"].Value.Trim() : "";
                if (rest.Length > 0) Content(rest, n.Line);
                Nodes(n.Kids);
                jump.Target = code.Count;
            }
            else
            {
                Content(t, n.Line);
                Nodes(n.Kids);
            }
        }
    }

    // ──────────────────────────────────────────── lựa chọn của truyện (Lựa chọn 0..5)

    static readonly Regex ChoiceRe = new(@"^\[(\d)-([ABC])\]\s*(.+?)(?:\s+→\s+(.+))?$");

    void Choice(int id, Block head)
    {
        i++;
        var ask = new AskChoiceI { Id = id, Line = head.Line };
        while (i < B.Count && B[i].Type is BT.Bullet or BT.Plain)
        {
            if (B[i].Type == BT.Plain) { i++; continue; }   // ghi chú soạn thảo
            var tree = TakeBulletTree();
            CollectChoice(tree, new List<Cond>(), ask, id);
        }
        ch.ChoiceIds.Add(id);
        code.Add(ask);
    }

    void CollectChoice(List<Node> nodes, List<Cond> conds, AskChoiceI ask, int id)
    {
        foreach (var n in nodes)
        {
            var m = ChoiceRe.Match(n.Text);
            if (m.Success)
            {
                var o = new ChoiceOption { Key = m.Groups[2].Value[0], Text = Unquote(m.Groups[3].Value.Trim()) };
                if (conds.Count > 0) o.Cond = conds.Aggregate((a, b) => Cond.And(a, b));
                if (m.Groups[4].Success) { var e = Effects(m.Groups[4].Value, n.Line); if (e != null) o.Effs.AddRange(e.Effs); }
                // Chương 2, mục 4: chỉ 5-B là "ngay_bay_gio"
                if (id == 5) o.Effs.Add(new Eff { Var = "han_vane", SetFlagValue = o.Key == 'B' ? "ngay_bay_gio" : "sang_mai" });
                ask.Options.Add(o);
                continue;
            }
            var cm = CondBullet.Match(n.Text);
            if (n.Text.StartsWith("Nếu ") && cm.Success)
            {
                var c = ParseCond(cm.Groups["c"].Value.Trim(), n.Line);
                if (c != null) { conds.Add(c); CollectChoice(n.Kids, conds, ask, id); conds.RemoveAt(conds.Count - 1); }
                continue;
            }
            Warn(n.Line, "dòng trong khối lựa chọn không hiểu: " + n.Text);
        }
    }

    // ──────────────────────────────────────────── trận đối chất

    sealed class OptDef
    {
        public string Key = "", Text = ""; public string? Variant, Note; public List<Node> Body = new();
        public List<OptDef>? Second; public int Line;
    }

    List<OptDef> ParseOptGroup()
    {
        var list = new List<OptDef>();
        while (i < B.Count && B[i].Type == BT.Plain && OptRe.IsMatch(B[i].Text))
        {
            var m = OptRe.Match(B[i].Text);
            var d = new OptDef
            {
                Key = m.Groups["k"].Value, Text = m.Groups["t"].Value,
                Variant = m.Groups["v"].Success ? m.Groups["v"].Value : null,
                Note = m.Groups["n"].Success ? m.Groups["n"].Value : null, Line = B[i].Line,
            };
            i++;
            d.Body = TakeBulletTree();
            if (i < B.Count && B[i].Type == BT.Plain && B[i].Text.StartsWith("Lượt hai:")) { i++; d.Second = ParseOptGroup(); }
            list.Add(d);
        }
        return list;
    }

    void OptionGroupTop()
    {
        var g = ParseOptGroup();
        var ends = new List<int>();
        EmitGroup(g, ends);
        foreach (var j in ends) ((JumpI)code[j]).Target = code.Count;
    }

    void EmitGroup(List<OptDef> g, List<int> ends)
    {
        var ask = new AskBattleI { Line = g.Count > 0 ? g[0].Line : 0 };
        code.Add(ask);
        foreach (var d in g)
            ask.Options.Add(new BattleOption { Key = d.Key, Variant = d.Variant, RequiresNote = d.Note, Text = d.Text });
        for (int k = 0; k < g.Count; k++)
        {
            var d = g[k];
            ask.Options[k].Addr = code.Count;
            // Chọn xong: câu trên nút thành lượt thoại của Kael, rồi mới tới các dòng "- Kael:" bên dưới (HUONG_DAN_DEV mục 7)
            code.Add(new SayI { Kind = SayKind.Say, Name = "Kael", Text = d.Text, Line = d.Line });
            if (ch.BattleKind == "helena" && d.Key == "C")
                code.Add(new EffectI { Effs = { new Eff { Var = "duy_vat_bien_chung", Delta = 1 } }, Line = d.Line });
            if (ch.BattleKind == "vane" && d.Key == "Đòn" && d.Note != null && d.Note.StartsWith("Mười hai tệp chưa xóa"))
                code.Add(new EffectI { Effs = { new Eff { Var = "da_dung_don_cau_4", SetFlagValue = "co" } }, Line = d.Line });
            var saved = stanceOk;
            stanceOk = ch.StanceCounting && d.Key != "C";   // đáp án [C] của trận Helena đã được tính ở trên
            Nodes(d.Body);
            stanceOk = saved;
            if (d.Second != null) EmitGroup(d.Second, ends);
            else { code.Add(new JumpI { Target = -1 }); ends.Add(code.Count - 1); }
        }
    }
}
