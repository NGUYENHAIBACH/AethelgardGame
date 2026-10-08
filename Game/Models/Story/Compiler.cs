using System.Text.RegularExpressions;

namespace AethelgardGame.Models.Story;

/// <summary>Biên dịch một file kịch bản .md thành danh sách lệnh phẳng.
/// Quy ước đọc kịch bản lấy theo các file HUONG_DAN_DEV trong ban_giao_dev/ (Mở đầu và Chương 1: mục 2; Chương 2: mục 3;
/// Chương 3, 4, 5: mục 2 của từng file). Chữ trong kịch bản được giữ nguyên; thứ gì không hiểu thì ghi vào Warnings chứ không tự bịa.</summary>
public sealed class Compiler
{
    readonly List<Block> B;
    readonly Chapter ch;
    readonly List<Instr> code;
    readonly List<string> warn;
    readonly Dictionary<int, ShardDef> shards;
    int i;
    bool defRegion, endEmitted, stanceOk;

    int chapNo, sceneNo;                                  // "Chương N" (Mở đầu = 0), "Cảnh N"
    int slot, cau;                                        // Chương 3: mắt xích / điều đang xét. Chương 5: câu đang hỏi
    bool inGoi;                                           // đang ở trong khối **Gọi "…" (đúng)**
    readonly List<string> linkNames = new();              // tên các mắt xích của Chương 3 Cảnh 2
    List<(string who, string text)> ruleLines = new();    // lời nói trong khối "Luật chọn" / "Luật gắn" gần nhất
    readonly Dictionary<string, int> labels = new();      // tiêu đề ## / ### → địa chỉ lệnh
    JumpI? subSkip;                                       // đang biên dịch một đoạn chỉ chạy khi được gọi
    AskCallI? lastCall;

    public static readonly string[] CallNames = { "Tầng Đáy", "Tầng Trung", "Người ký và người gác", "Veritas", "Cái lô chiều nay" };
    public const int BattleCheckpoint = 100;              // điểm quay lại "đầu Câu 1" của trận cuối

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
                case BT.H4: H4(b); i++; break;
                case BT.Bold: Bold(b); break;
                case BT.Bullet: Bullets(); break;
                case BT.Quote: QuoteBlock(b); i++; break;
                case BT.Plain: Plain(b); break;
            }
        }
        CloseAll(); EndSub();
        if (!endEmitted) { code.Add(new ChapterEndI()); endEmitted = true; }
        FillItemFallbacks();
        foreach (var ins in code)
        {
            if (ins is GotoI g) { if (labels.TryGetValue(g.Label, out var a)) g.Target = a; else Warn(g.Line, "không tìm thấy đoạn để nhảy tới: " + g.Label); }
            if (ins is CallI c) { if (labels.TryGetValue(c.Label, out var a)) c.Target = a; else Warn(c.Line, "không tìm thấy đoạn để gọi: " + c.Label); }
        }
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

    // Mỗi tiêu đề có điều kiện mở một phạm vi; phạm vi kéo dài tới tiêu đề cùng cấp hoặc cấp cao hơn kế tiếp.
    // ### → cấp 10. #### → cấp 20. **Nhánh X**, tiêu đề cách kết, "Nếu đã chọn …" → cấp 30. Nhãn in đậm có điều kiện khác → cấp 40.
    // Nhãn in đậm không điều kiện ("Chung", "Nhập lại", "Nhịp 1"...) đóng các phạm vi cấp 30 và 40.
    const int LH3 = 10, LH4 = 20, LBR = 30, LBOLD = 40;
    readonly List<(int jump, int level)> scopes = new();

    void OpenScope(Cond c, int level)
    {
        CloseTo(level);
        code.Add(new JumpIfNotI { Cond = c, Target = -1 });
        scopes.Add((code.Count - 1, level));
    }

    void CloseTo(int level)
    {
        while (scopes.Count > 0 && scopes[^1].level >= level)
        {
            ((JumpIfNotI)code[scopes[^1].jump]).Target = code.Count;
            scopes.RemoveAt(scopes.Count - 1);
        }
    }

    void CloseAll() => CloseTo(0);

    Cond? ParseCond(string text, int line)
    {
        var c = Cond.Parse(text, out var err);
        if (c == null) Warn(line, "điều kiện không hiểu: " + err);
        return c;
    }

    void Label(string title)
    {
        labels[title] = code.Count;
        var cut = title.IndexOfAny(new[] { ':', '(' });
        if (cut > 0) labels[title[..cut].Trim()] = code.Count;
    }

    /// <summary>Điều kiện ngầm trong một tiêu đề: "Nhánh 6-A: …", "Bản trên: …", "Mở, bản dưới (D1, D2, D3)", "Lối T2: …",
    /// "Trước mặt Helena (T1, T2)", "Tấm nhãn (chỉ khi thắng Vane và ở bản trên; …)". Không có thì trả null.</summary>
    Cond? HeadCond(string t, int line)
    {
        if (t.StartsWith("Nhánh "))
        {
            var m = Regex.Match(t, @"\d{1,2}-[ABC]");
            return m.Success ? ParseCond(m.Value, line) : null;
        }
        var only = Regex.Match(t, @"\(chỉ khi ([^;)]+)");
        if (only.Success) return ParseCond(only.Groups[1].Value, line);
        var ban = Regex.Match(t, @"^(?:Mở, )?([Bb]ản (?:trên|dưới))");
        if (ban.Success) return ParseCond(ban.Groups[1].Value, line);
        var loi = Regex.Match(t, @"\b[Ll]ối ([TD]\d)\b");
        if (loi.Success) return Cond.FlagIs("loi_vao", loi.Groups[1].Value);
        var list = Regex.Match(t, @"\(([TD]\d(?:,\s*[TD]\d)*)\)");
        if (list.Success) return Cond.FlagIn("loi_vao", Regex.Split(list.Groups[1].Value, @",\s*"));
        return null;
    }

    Cond? BoldCond(string t, int line)
    {
        // Chương 3 Cảnh 3: kết quả của màn gắn lời kể
        if (t == "Nếu bác ngay") return Cond.FlagIs("bac_dung", "co");
        if (t == "Nếu không bác") return Cond.Not(Cond.FlagIs("bac_dung", "co"));
        if (t == "Nếu gắn được") return Cond.FlagIs("dieu_3", "gan");
        if (t == "Nếu treo") return Cond.FlagIs("dieu_3", "treo");
        var gan = Regex.Match(t, @"^Nếu gắn vào ""(.+)""$");
        if (gan.Success) return Cond.FlagIs("dieu_" + slot, LinkNo(gan.Groups[1].Value, line));
        if (t.StartsWith("Veritas đồng hành")) return Cond.FlagIs("veritas", "dong_hanh");
        if (t.StartsWith("Veritas do dự")) return Cond.FlagIs("veritas", "do_du");

        if (t.StartsWith("Nếu ")) return ParseCond(t, line);
        if (t.StartsWith("Một câu của Rian"))
            return new Cond.Fn("một câu của Rian", s =>
                s.Flag("da_dung_don_cau_4") == "co" && s.Flag("lua_chon_4") != "B" &&
                s.Flag("vane") is "dong_minh" or "dung_ngoai");
        return HeadCond(t, line);
    }

    string LinkNo(string name, int line)
    {
        int k = linkNames.FindIndex(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
        if (k < 0) Warn(line, "không có mắt xích tên: " + name);
        return (k + 1).ToString();
    }

    // ──────────────────────────────────────────── tiêu đề

    void H1(Block b)
    {
        CloseAll(); EndSub();
        if (b.Text.StartsWith("Mảnh lưu trữ"))
        {
            if (!endEmitted) { code.Add(new ChapterEndI()); endEmitted = true; }
            defRegion = true;
            return;
        }
        ch.Title = b.Text;
        var m = Regex.Match(b.Text, @"^Chương (\d+)");
        chapNo = m.Success ? int.Parse(m.Groups[1].Value) : 0;
        code.Add(new CmdI { Kind = CmdKind.Card, Value = b.Text, Line = b.Line });
        if (chapNo is 3 or 4) code.Add(new SpecialI { Name = "init_c" + chapNo, Line = b.Line });
    }

    void H2(Block b)
    {
        var t = b.Text;
        CloseAll(); EndSub();
        Label(t);
        if (t.StartsWith("Cảnh"))
        {
            if (t.Contains("Đối chất với Helena")) { ch.BattleKind = "helena"; ch.StanceCounting = true; }
            else if (t.Contains("Đối chất với Vane")) { ch.BattleKind = "vane"; }
            var mn = Regex.Match(t, @"^Cảnh (\d+)");
            sceneNo = mn.Success ? int.Parse(mn.Groups[1].Value) : sceneNo + 1;
            slot = 0; cau = 0;
            code.Add(new CmdI { Kind = CmdKind.SceneStart, Line = b.Line });
            if (chapNo == 4 && sceneNo == 3) code.Add(new SpecialI { Name = "c4_scene3", Line = b.Line });
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
        if (t.StartsWith("Hai cách kết thúc"))
        {
            // Chương 3 Cảnh 3: chốt biến veritas trước khi rẽ hai cách kết. Chương 5 Cảnh 3: chỉ là mốc để nhảy tới
            if (chapNo == 3) code.Add(new SpecialI { Name = "resolve_veritas", Line = b.Line });
            i++; return;
        }
        if (t.StartsWith("Kết Chương")) { i++; return; }
        Warn(b.Line, "tiêu đề ## không hiểu: " + t);
        i++;
    }

    void H3(Block b)
    {
        var t = b.Text;
        CloseAll(); EndSub();
        inGoi = false;
        if (t.StartsWith("Câu "))
        {
            var mq = Regex.Match(t, @"^Câu (\d+)");
            cau = mq.Success ? int.Parse(mq.Groups[1].Value) : cau + 1;
            if (ch.BattleKind != null)
            {
                var spr = ch.BattleKind == "vane" ? "Vane_Neutral" : "Helena_Neutral";   // HUONG_DAN_DEV mục 7: về sprite thường khi bắt đầu câu mới
                code.Add(new CmdI { Kind = CmdKind.Expr, Value = spr, Line = b.Line });   // chỉ đổi biểu cảm, không đưa ai lên (sân khấu 2.9)
            }
            else if (cau == 1) code.Add(new CheckpointI { Id = BattleCheckpoint, Line = b.Line });   // Chương 5: nút quay lại về đầu Câu 1
            Label(t);
            return;
        }
        if (t.StartsWith("Khi một chốt vỡ"))
        {
            // đoạn chỉ chạy khi được gọi: đường chạy thẳng nhảy qua nó
            subSkip = new JumpI { Target = -1, Line = b.Line };
            code.Add(subSkip);
            Label(t);
            return;
        }
        Label(t);
        if (t.StartsWith("Đoạn")) return;
        if (t.StartsWith("Kết cục sớm"))
        {
            var c = ch.BattleKind == "vane" ? Cond.FlagIs("vane", "ket_cuc_som") : Cond.FlagIs("ket_doi_chat_helena", "ket_cuc_som");
            OpenScope(c, LBR);
            return;
        }
        if (chapNo == 5 && t is "Thắng" or "Thua") { OpenScope(Cond.FlagIs("tran", t == "Thắng" ? "thang" : "thua"), LH3); return; }

        var mx = Regex.Match(t, @"^Mắt xích (\d): ([^(]+)");
        if (mx.Success)
        {
            slot = int.Parse(mx.Groups[1].Value);
            var name = mx.Groups[2].Value.Trim();
            linkNames.Add(char.ToUpper(name[0]) + name[1..]);
            var need = Regex.Match(t, @"chỉ khi có ""([^""]+)""");
            if (need.Success)
            {
                // mắt xích 5 và 6: không có màn chọn; có ghi chú thì tự nối
                var key = need.Groups[1].Value;
                OpenScope(new Cond.Fn("có " + key, s => s.HasNote(key)), LH3);
                code.Add(new EffectI
                {
                    Line = b.Line,
                    Effs = { new Eff { Var = "mat_xich_" + slot, SetFlagValue = "co" }, new Eff { Var = "so_cai", Delta = 1 } },
                });
            }
            return;
        }
        var md = Regex.Match(t, @"^Điều (\d)");
        if (md.Success) { slot = int.Parse(md.Groups[1].Value); return; }

        var hc = HeadCond(t, b.Line);
        if (hc != null) OpenScope(hc, LH3);
    }

    void H4(Block b)
    {
        CloseTo(LH4);
        var hc = HeadCond(b.Text, b.Line);
        if (hc != null) OpenScope(hc, LH4);
    }

    void Bold(Block b)
    {
        var t = b.Text.TrimEnd(':').Trim();
        i++;
        if (t.StartsWith("Luật chọn") || t.StartsWith("Luật gắn"))
        {
            // luật của màn chơi: không hiện; các dòng thoại bên trong là lời nói khi chọn sai
            ruleLines = new();
            void Walk(List<Node> ns)
            {
                foreach (var n in ns)
                {
                    foreach (Match m in Regex.Matches(n.Text, @"(Veritas|Kael): ""([^""]+)""")) ruleLines.Add((m.Groups[1].Value, m.Groups[2].Value));
                    Walk(n.Kids);
                }
            }
            if (i < B.Count && B[i].Type == BT.Bullet) Walk(TakeBulletTree());
            return;
        }
        var mg = Regex.Match(t, @"^Gọi ""(.+)"" \((đúng|sai)\)$");
        if (mg.Success)
        {
            OpenScope(Cond.FlagIs("goi", mg.Groups[1].Value), LBOLD);
            inGoi = mg.Groups[2].Value == "đúng";
            if (inGoi && lastCall != null) lastCall.Correct = mg.Groups[1].Value;
            return;
        }
        inGoi = false;
        if (t.StartsWith("Nhánh "))
        {
            CloseTo(LBR);
            var c = HeadCond(t, b.Line);
            if (c != null) OpenScope(c, LBR);
            return;
        }
        var bc = BoldCond(t, b.Line);
        if (bc != null) OpenScope(bc, LBOLD);
        else CloseTo(LBR);                      // nhãn không điều kiện: các nhánh chạy chung từ đây
    }

    /// <summary>Kết thúc đoạn chương trình con "Khi một chốt vỡ": mất cả ba chốt thì sang thẳng "Thua", không quay về.</summary>
    void EndSub()
    {
        if (subSkip == null) return;
        var j = new JumpIfNotI { Cond = new Cond.Fn("mat_chot từ 3", s => s.Var("mat_chot") >= 3), Target = -1 };
        code.Add(j);
        code.Add(new EffectI { Effs = { new Eff { Var = "tran", SetFlagValue = "thua" } } });
        code.Add(new GotoI { Label = "Thua", ClearStack = true });
        j.Target = code.Count;
        code.Add(new ReturnI());
        subSkip.Target = code.Count;
        subSkip = null;
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
            CloseAll(); var c = ParseCond(t, b.Line); if (c != null) OpenScope(c, LBR); i++; return;
        }
        var oc = OutcomeHeading(t);
        if (oc != null) { CloseAll(); OpenScope(oc, LBR); i++; return; }

        if (OptRe.IsMatch(t)) { OptionGroupTop(); return; }

        if (t.StartsWith("(Hai thanh hiện lên"))
        {
            code.Add(new SpecialI { Name = "bars_on", Line = b.Line }); i++; return;
        }
        if (SkipPrefixes.Any(p => t.StartsWith(p, StringComparison.Ordinal))) { ch.Skipped.Add($"{b.Line}: {t}"); i++; return; }

        if (t.StartsWith("━━"))
        {
            var m = Regex.Match(t, @"━━\s*(.+?)\s*━━");
            var title = m.Success ? m.Groups[1].Value : t;
            code.Add(new EndScreenI { Title = title, NoRetry = title.StartsWith("KẾT CỤC 7/7"), Line = b.Line });
            i++; return;
        }

        if (t.StartsWith("→"))
        {
            // Chương 1, 2: dòng "→ ..." đứng riêng là kết quả của cả nhánh, không thuộc điều kiện con vừa đóng
            if (chapNo <= 2 && scopes.Any(s => s.level == LBR)) CloseTo(LBOLD);
            // Chương 3 Cảnh 2: danh sách các thứ khớp nằm ở các gạch đầu dòng ngay bên dưới
            if (t.Contains("Người chơi chọn một trong các thứ đang có")) { i++; NotePickList(b.Line); return; }
        }

        // bảng và đoạn văn xuôi không có người nói: ghi chú của người viết, không hiện
        if (t.StartsWith("|") || IsProse(t)) { ch.Skipped.Add($"{b.Line}: {t}"); i++; return; }

        Content(t, b.Line);
        i++;
    }

    static bool IsProse(string t)
    {
        if (t[0] is '[' or '(' or '→' or '━' or '"') return false;
        var m = SpeakerRe.Match(Regex.Split(t, @"\s/\s")[0]);
        if (!m.Success) return !Regex.IsMatch(t, @"^sang """);
        var name = m.Groups["name"].Value;
        return name.Length > 32 || name.Contains(". ") || name.Contains(", ");
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

    static readonly Regex EditorTag = new(@"\s*\[(?:GIEO|TRẢ|GẶT|GẮN)[^\]]*\]");
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
                    if (kind == CmdKind.Sprite && val == "tắt") { kind = CmdKind.SpriteOff; val = null!; }               // gỡ hết
                    else if (kind == CmdKind.Sprite && val.EndsWith(" tắt")) { kind = CmdKind.SpriteOff; val = val[..^4].Trim(); }
                    code.Add(new CmdI { Kind = kind, Value = val, Line = line });
                    text = text[mt.Length..]; continue;
                }
                var mi = Regex.Match(text, @"^\[(VẬT PHẨM MỚI|GHI CHÚ MỚI):\s*(.+?)\]\s*(?<tail>\(.*\))?\s*$");
                if (mi.Success)
                {
                    var name = mi.Groups[2].Value.Trim();
                    if (mi.Groups[1].Value == "VẬT PHẨM MỚI")
                    {
                        // Chương 3: xấp nhãn gộp luôn tấm nhãn ông Vane đưa ở Chương 2
                        if (name == "Xấp nhãn hòm thuốc") code.Add(new RemoveItemI { Name = "Nhãn hòm thuốc", Line = line });
                        code.Add(new AddItemI { Name = name, Line = line });
                        return;
                    }
                    // Chương 3 Cảnh 3: tên ghi chú liệt kê đúng các điều đã gắn: (thêm "…" nếu gắn được điều 3)
                    var extra = Regex.Match(mi.Groups["tail"].Value, @"thêm ""([^""]+)"" nếu gắn được điều 3");
                    if (extra.Success)
                    {
                        int cut = name.LastIndexOf("; ", StringComparison.Ordinal);
                        var full = cut > 0 ? name[..cut] + "; " + extra.Groups[1].Value + name[cut..] : name + "; " + extra.Groups[1].Value;
                        var j = new JumpIfNotI { Cond = Cond.FlagIs("dieu_3", "gan"), Target = -1, Line = line };
                        code.Add(j);
                        code.Add(new AddNoteI { Name = full, Line = line });
                        j.Target = code.Count;
                        var j2 = new JumpIfNotI { Cond = Cond.Not(Cond.FlagIs("dieu_3", "gan")), Target = -1, Line = line };
                        code.Add(j2);
                        code.Add(new AddNoteI { Name = name, Line = line });
                        j2.Target = code.Count;
                        return;
                    }
                    code.Add(new AddNoteI { Name = name, Line = line });
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
                Arrow(text[1..].Trim(), line);
                return;
            }
            var go = Regex.Match(text, @"^sang ""(.+?)""\.?$");       // "Nếu vung từ 6 trở lên: sang "Thắng"."
            if (go.Success)
            {
                var to = go.Groups[1].Value;
                code.Add(new EffectI { Effs = { new Eff { Var = "tran", SetFlagValue = to == "Thắng" ? "thang" : "thua" } }, Line = line });
                code.Add(new GotoI { Label = to, Line = line });
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
        if (t.StartsWith("(Màn hình tối."))                         // Chương 2, thang xuống: màn hình đen, ô sáng nhỏ dần
            code.Add(new CmdI { Kind = CmdKind.Elevator, Value = "on", Line = line });

        // Chương 1: dòng "Về việc" trên giấy gọi đổi theo Lựa chọn 0 (HUONG_DAN_DEV.md mục 6; ba câu lấy từ ghi chú dev ngay dưới dòng ấy)
        const string veViec = "Về việc: tệp 4412-07.";
        if (t.StartsWith("(Màn hình hiện: \"Cư dân KAEL") && t.Contains(veViec))
        {
            foreach (var (key, phrase) in new[] { ("A", "xác nhận lệnh xóa cưỡng chế tệp 4412-07"), ("B", "lô 4412 thiếu bản ghi xóa tệp 07"), ("C", "tiếp nhận báo cáo lỗi tệp 4412-07") })
            {
                var j = new JumpIfNotI { Cond = Cond.FlagIs("lua_chon_0", key), Target = -1, Line = line };
                code.Add(j);
                code.Add(new SayI { Kind = SayKind.Stage, Text = t.Replace(veViec, "Về việc: " + phrase + "."), Line = line });
                j.Target = code.Count;
            }
            return;
        }

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
            else if (mod.Contains("loa")) say.Kind = SayKind.Loa;
            if (mod.Contains("rất khẽ") || mod.Contains("thì thầm")) say.Whisper = true;   // chỉ là kiểu chữ (sân khấu 2.8)
            if (mod.Contains("to tiếng")) say.Loud = true;
            code.Add(say);
        }
    }

    static string Unquote(string s) =>
        s.Length >= 2 && s[0] == '"' && s[^1] == '"' && s.Count(c => c == '"') == 2 ? s[1..^1] : s;

    // ──────────────────────────────────────────── dòng "→ ..."

    void Arrow(string t, int line)
    {
        if (t.StartsWith("Gọi ai?"))
        {
            lastCall = new AskCallI { Cau = cau, Names = CallNames.ToList(), Line = line };
            code.Add(lastCall);
            return;
        }
        var run = Regex.Match(t, @"^Chạy ""(.+?)""");
        if (run.Success) { code.Add(new CallI { Label = run.Groups[1].Value, Line = line }); return; }
        var drop = Regex.Match(t, @"^Vật phẩm ""(.+?)"" rời khỏi túi đồ");
        if (drop.Success) { code.Add(new RemoveItemI { Name = drop.Groups[1].Value, Line = line }); return; }
        if (t.StartsWith("Người chơi chọn") || Regex.IsMatch(t, @"^Nếu đã nối mắt xích \d ở Cảnh 2: người chơi chọn"))
        {
            if (sceneNo == 3) LinkPick(line);
            else
            {
                var key = Regex.Match(t, @"ghi chú ""(.+?)""");
                if (!key.Success) { Warn(line, "màn chọn ghi chú không ghi thứ cần chọn: " + t); return; }
                code.Add(new AskNoteI
                {
                    Slot = slot, Line = line, Answers = { new PickAnswer { Key = key.Groups[1].Value } },
                    Wrong = RuleTexts(0, 3), Remind = RuleText(3),
                });
            }
            return;
        }
        var eff = Effects(t, line);
        if (eff != null) code.Add(eff);
        // Chương 3, lối T2: Kael để tờ lời lại trên bàn bà Helena (HUONG_DAN_DEV_CHUONG_3.md mục 13, điểm 6)
        if (t.Contains("báo cho A.L.I.C.E sau chuyến thang chiều")) code.Add(new RemoveItemI { Name = "Tờ lời bà Helena soạn", Line = line });
    }

    string RuleText(int k) => k < ruleLines.Count ? ruleLines[k].text : "";
    List<string> RuleTexts(int from, int count) => ruleLines.Skip(from).Take(count).Select(x => x.text).ToList();

    void LinkPick(int line)
    {
        var v = ruleLines.Where(x => x.who == "Veritas").Select(x => x.text).ToList();
        string V(int k) => k < v.Count ? v[k] : "";
        code.Add(new AskLinkI
        {
            Slot = slot, Line = line, Links = linkNames.ToList(),
            Wrong = v.Take(2).ToList(), Empty = V(2), HasPlace = V(3), NoMatchV = V(4),
            NoMatchK = ruleLines.FirstOrDefault(x => x.who == "Kael").text ?? "",
        });
    }

    /// <summary>Chương 3 Cảnh 2, mắt xích 3 và 4: mỗi gạch đầu dòng là một thứ khớp, kèm đoạn thoại của nó.</summary>
    void NotePickList(int line)
    {
        var ask = new AskNoteI { Slot = slot, Line = line, Wrong = RuleTexts(0, 3), Remind = RuleText(3) };
        code.Add(ask);
        var ends = new List<JumpI>();
        void CloseEnds() { foreach (var e in ends) e.Target = code.Count; ends.Clear(); }
        if (i >= B.Count || B[i].Type != BT.Bullet) { Warn(line, "màn chọn ghi chú không có danh sách bên dưới"); return; }
        foreach (var n in TakeBulletTree())
        {
            var ma = Regex.Match(n.Text, @"^""([^""]+)""");
            if (ma.Success)
            {
                ask.Answers.Add(new PickAnswer { Key = ma.Groups[1].Value, Addr = code.Count });
                // người cầm Lệnh điều chuyển và không bị thuyết phục sáng nay đã thấy tấm bảng ở quầy: thêm một dòng riêng vào danh sách
                if (n.Text.Contains("đang cầm Lệnh điều chuyển và không bị thuyết phục"))
                    ask.Answers.Add(new PickAnswer
                    {
                        Key = "Tấm bảng ở quầy suất", Addr = code.Count,
                        Pseudo = new Cond.Fn("thấy tấm bảng ở quầy", s => s.GiayTo == "dieu_chuyen" && s.Flag("vane") is "dong_minh" or "dung_ngoai"),
                    });
                Nodes(n.Kids);
                var j = new JumpI { Target = -1 }; code.Add(j); ends.Add(j);
            }
            else if (n.Text.StartsWith("Nếu chọn \"Không có gì\""))
            {
                ask.NoneAddr = code.Count;
                Nodes(n.Kids);
                var j = new JumpI { Target = -1 }; code.Add(j); ends.Add(j);
            }
            else if (n.Text.StartsWith("Nếu nối được"))
            {
                CloseEnds();
                var j = new JumpIfNotI { Cond = Cond.FlagIs("mat_xich_" + slot, "co"), Target = -1, Line = n.Line };
                code.Add(j);
                Nodes(n.Kids);
                j.Target = code.Count;
            }
            else Warn(n.Line, "dòng trong màn chọn ghi chú không hiểu: " + n.Text);
        }
        CloseEnds();
    }

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

        // Chương 3 trở đi: biến viết thẳng tên. "→ lo = co.", "→ vane = dong_minh.", "(veritas = dong_hanh)", "→ vung +2."
        if (chapNo >= 3)
        {
            // Chương 4 Cảnh 2: Vane thành đồng minh thì tờ báo cáo tối nay không đi lên (HUONG_DAN_DEV_CHUONG_4.md mục 3)
            t = VarRe.Replace(t, "");   // "Lòng tin +1" đã tính ở trên; đừng đọc chữ "tin" thành tên biến
            const string onlyT2 = "lo = co chỉ còn ở lối T2";
            if (t.Contains(onlyT2))
            {
                var isT2 = Cond.FlagIs("loi_vao", "T2");
                t = t.Replace(onlyT2, "");
                e.Effs.Add(new Eff { Var = "lo", SetFlagValue = "co", If = isT2 });
                e.Effs.Add(new Eff { Var = "lo", SetFlagValue = "khong", Unless = isT2 });
            }
            foreach (Match m in Regex.Matches(t, @"\b([a-z][a-z_0-9]*) = ([a-z_]+)\b"))
            {
                var name = m.Groups[1].Value;
                if (name.StartsWith("lua_chon_")) continue;      // đã đặt lúc người chơi bấm nút
                e.Effs.Add(new Eff { Var = name, SetFlagValue = m.Groups[2].Value });
            }
            foreach (Match m in Regex.Matches(t, @"\b([a-z][a-z_0-9]*) \+(\d+)"))
                e.Effs.Add(new Eff { Var = m.Groups[1].Value, Delta = int.Parse(m.Groups[2].Value) });
            if (t.Contains("Bà Helena từ đứng ngoài thành đồng minh")) e.Effs.Add(new Eff { Var = "helena", SetFlagValue = "dong_minh" });
        }

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

    sealed class Node { public string Text = ""; public int Line; public bool Blank; public List<Node> Kids = new(); }

    List<Node> TakeBulletTree()
    {
        var roots = new List<Node>();
        var stack = new List<(int indent, Node n)>();
        while (i < B.Count && B[i].Type == BT.Bullet)
        {
            var b = B[i++];
            var n = new Node { Text = b.Text, Line = b.Line, Blank = b.Blank };
            while (stack.Count > 0 && stack[^1].indent >= b.Indent) stack.RemoveAt(stack.Count - 1);
            if (stack.Count == 0) roots.Add(n); else stack[^1].n.Kids.Add(n);
            stack.Add((b.Indent, n));
        }
        return roots;
    }

    void Bullets()
    {
        var tree = TakeBulletTree();
        Nodes(tree, inGoi);
    }

    // "Nếu 1-C. …", "Nếu cầm X:", "Bản trên. …", "Bản dưới, bị Vane thuyết phục. …", "Ở cả hai trường hợp trên:"
    static readonly Regex CondBullet = new(
        @"^(?:Nếu (?<c>(?:""[^""]*""|[^"".:])+?)|(?<c>Bản (?:trên|dưới)(?:""[^""]*""|[^"".:])*?)|(?<c>Ở cả hai trường hợp trên))(?:\s\((?<n>[^()]*)\))?(?<sep>[.:])(?:\s+(?<rest>.*))?$");

    static readonly string[] ElseForms = { "không", "không có điều nào ở trên", "không có cả ba", "không có vật phẩm ấy" };

    enum NK { If, Else, ElseIf, Both }

    sealed class CondNode { public Node N = null!; public NK Kind; public Cond? C; public string Rest = ""; public bool MarkBoth; }

    CondNode? AsCond(Node n)
    {
        var t = n.Text;
        if (!(t.StartsWith("Nếu ") || t.StartsWith("Bản trên") || t.StartsWith("Bản dưới") || t.StartsWith("Ở cả hai trường hợp trên"))) return null;
        var m = CondBullet.Match(t);
        if (!m.Success) return null;
        var ctext = m.Groups["c"].Value.Trim();
        var cn = new CondNode { N = n, Rest = m.Groups["rest"].Success ? m.Groups["rest"].Value.Trim() : "" };
        if (ctext.StartsWith("Ở cả hai")) { cn.Kind = NK.Both; cn.C = Cond.FlagIs("_ca_hai", "co"); return cn; }
        if (ElseForms.Contains(ctext)) { cn.Kind = NK.Else; return cn; }
        var mei = Regex.Match(ctext, @"^không,? và (.+)$");
        if (mei.Success) { cn.Kind = NK.ElseIf; ctext = mei.Groups[1].Value; }
        // Chương 3 Cảnh 3, điều 5: "Nếu chọn "Cái ống và cái lò"."
        var pick = Regex.Match(ctext, @"^chọn ""(.+)""$");
        if (pick.Success) { cn.C = Cond.FlagIs("dieu_" + slot, LinkNo(pick.Groups[1].Value, n.Line)); return cn; }
        var full = m.Groups["n"].Success ? ctext + " (" + m.Groups["n"].Value + ")" : ctext;
        cn.C = ParseCond(full, n.Line) ?? Cond.Never;
        return cn;
    }

    /// <summary>Các gạch đầu dòng cùng cấp. Một dãy "Nếu …" liền nhau (không cách dòng trống) kết bằng "Nếu không" là một chuỗi
    /// xét từ trên xuống, chạy đúng khối đầu tiên khớp. Dãy không có "Nếu không" thì mỗi khối tự xét riêng.</summary>
    void Nodes(List<Node> nodes, bool firstMatch = false)
    {
        int k = 0;
        while (k < nodes.Count)
        {
            var n = nodes[k];
            var cn = AsCond(n);
            if (cn == null)
            {
                if (n.Text.StartsWith("**")) ch.Skipped.Add($"{n.Line}: {n.Text}");   // "- **Bản trên:** …": lời giải thích của người viết
                else if (n.Text.StartsWith("Nếu ")) Warn(n.Line, "điều kiện không đọc được: " + n.Text);
                else Content(n.Text, n.Line);
                Nodes(n.Kids);
                k++; continue;
            }
            var run = new List<CondNode> { cn };
            int e = k + 1;
            while (e < nodes.Count && !nodes[e].Blank && AsCond(nodes[e]) is { } more) { run.Add(more); e++; }
            EmitRun(run, firstMatch);
            k = e;
        }
    }

    void Body(CondNode cn)
    {
        if (cn.MarkBoth) code.Add(new EffectI { Effs = { new Eff { Var = "_ca_hai", SetFlagValue = "co" } }, Line = cn.N.Line });
        if (cn.Rest.Length > 0) Content(cn.Rest, cn.N.Line);
        Nodes(cn.N.Kids);
    }

    void EmitRun(List<CondNode> run, bool firstMatch)
    {
        // "Ở cả hai trường hợp trên": chạy nếu một trong hai khối ngay phía trên đã chạy
        for (int k = 0; k < run.Count; k++)
            if (run[k].Kind == NK.Both)
            {
                if (k >= 1) run[k - 1].MarkBoth = true;
                if (k >= 2) run[k - 2].MarkBoth = true;
            }
        if (run.Any(x => x.Kind == NK.Both))
            code.Add(new EffectI { Effs = { new Eff { Var = "_ca_hai", SetFlagValue = "khong" } }, Line = run[0].N.Line });

        bool chain = firstMatch || run.Any(x => x.Kind is NK.Else or NK.ElseIf);
        var ends = new List<JumpI>();
        void CloseChain() { foreach (var j in ends) j.Target = code.Count; ends.Clear(); }
        bool seenIf = false;
        foreach (var cn in run)
        {
            if (cn.Kind == NK.Both)
            {
                CloseChain();
                var jb = new JumpIfNotI { Cond = cn.C!, Target = -1, Line = cn.N.Line };
                code.Add(jb); Body(cn); jb.Target = code.Count;
                continue;
            }
            if (cn.Kind == NK.Else)
            {
                if (!seenIf) { Warn(cn.N.Line, "\"Nếu không\" mà không có điều kiện đứng trước"); continue; }
                Body(cn);
                continue;
            }
            if (cn.Kind == NK.ElseIf && !seenIf) Warn(cn.N.Line, "\"Nếu không, và …\" mà không có điều kiện đứng trước");
            seenIf = true;
            var jn = new JumpIfNotI { Cond = cn.C!, Target = -1, Line = cn.N.Line };
            code.Add(jn);
            Body(cn);
            if (chain) { var j = new JumpI { Target = -1 }; code.Add(j); ends.Add(j); }
            jn.Target = code.Count;
        }
        CloseChain();
    }

    // ──────────────────────────────────────────── lựa chọn của truyện (Lựa chọn 0..10)

    static readonly Regex ChoiceRe = new(@"^\[(\d{1,2})-([ABC])\]\s*(?:\((?<ann>[^)]*)\)\s*)?(?<t>.+?)(?:\s+→\s+(?<e>.+))?$");

    void Choice(int id, Block head)
    {
        i++;
        var ask = new AskChoiceI { Id = id, Line = head.Line };
        Cond? group = null, skip = null;
        while (i < B.Count && B[i].Type is BT.Bullet or BT.Plain or BT.Bold)
        {
            var b = B[i];
            if (b.Type == BT.Plain)
            {
                // "(Lối D1 không có lựa chọn: chạy thẳng vào "Nhánh 6-C".)"
                var ms = Regex.Match(b.Text, @"Lối ([TD]\d) không có lựa chọn");
                if (ms.Success) skip = Cond.FlagIs("loi_vao", ms.Groups[1].Value);
                i++; continue;   // còn lại là ghi chú soạn thảo
            }
            if (b.Type == BT.Bold)
            {
                // "**Bản trên (T1, T2)**": nhóm nút theo nơi Kael đứng
                if (!Regex.IsMatch(b.Text, @"^Bản (trên|dưới)")) break;
                group = HeadCond(b.Text, b.Line);
                i++; continue;
            }
            var tree = TakeBulletTree();
            CollectChoice(tree, group == null ? new List<Cond>() : new List<Cond> { group }, ask, id);
        }
        ch.ChoiceIds.Add(id);
        if (skip == null) { code.Add(ask); return; }
        // lối không có lựa chọn: coi như đã chọn C (HUONG_DAN_DEV_CHUONG_3.md mục 13, điểm 2)
        var toAsk = new JumpIfNotI { Cond = skip, Target = -1, Line = head.Line };
        code.Add(toAsk);
        code.Add(new EffectI
        {
            Line = head.Line,
            Effs = { new Eff { Var = "lua_chon_" + id, SetFlagValue = "C" }, new Eff { Var = "kiem_chung", Delta = 1 } },
        });
        var over = new JumpI { Target = -1 };
        code.Add(over);
        toAsk.Target = code.Count;
        code.Add(ask);
        over.Target = code.Count;
    }

    void CollectChoice(List<Node> nodes, List<Cond> conds, AskChoiceI ask, int id)
    {
        foreach (var n in nodes)
        {
            var m = ChoiceRe.Match(n.Text);
            if (m.Success)
            {
                var o = new ChoiceOption { Key = m.Groups[2].Value[0], Text = Unquote(m.Groups["t"].Value.Trim()) };
                var all = conds.ToList();
                if (m.Groups["ann"].Success)
                {
                    var c = ChoiceNote(m.Groups["ann"].Value.Trim(), n.Line);
                    if (c != null) all.Add(c);
                }
                if (all.Count > 0) o.Cond = all.Aggregate((a, b) => Cond.And(a, b));
                if (m.Groups["e"].Success) { var e = Effects(m.Groups["e"].Value, n.Line); if (e != null) o.Effs.AddRange(e.Effs); }
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

    /// <summary>Chú thích sau nhãn nút: "(chỉ T2)", "(T2: chỉ khi sổ đủ)".</summary>
    Cond? ChoiceNote(string ann, int line)
    {
        var only = Regex.Match(ann, @"^chỉ ([TD]\d)$");
        if (only.Success) return Cond.FlagIs("loi_vao", only.Groups[1].Value);
        var when = Regex.Match(ann, @"^([TD]\d): chỉ khi (.+)$");
        if (when.Success)
        {
            var c = ParseCond(when.Groups[2].Value, line);
            return c == null ? null : Cond.Or(Cond.Not(Cond.FlagIs("loi_vao", when.Groups[1].Value)), c);
        }
        Warn(line, "chú thích của nút không hiểu: (" + ann + ")");
        return null;
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
