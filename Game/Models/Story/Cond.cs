using System.Text.RegularExpressions;
using AethelgardGame.Models.Game;

namespace AethelgardGame.Models.Story;

/// <summary>Điều kiện rẽ nhánh, đọc từ chữ trong kịch bản ("Nếu 1-C", "cầm Thẻ đặc phái", "Vane là đồng minh", "vung từ 6 trở lên"...).
/// Bảng cụm từ lấy theo các file HUONG_DAN_DEV (Chương 2 mục 13, Chương 3 mục 2-3, Chương 4 mục 2-3, Chương 5 mục 2-3).</summary>
public abstract class Cond
{
    public string Src = "";
    public abstract bool Eval(GameState s);

    public sealed class Fn : Cond
    {
        readonly Func<GameState, bool> f;
        public Fn(string src, Func<GameState, bool> f) { Src = src; this.f = f; }
        public override bool Eval(GameState s) => f(s);
    }
    sealed class NotC : Cond
    {
        readonly Cond a; public NotC(Cond a) { this.a = a; Src = "không " + a.Src; }
        public override bool Eval(GameState s) => !a.Eval(s);
    }
    sealed class AndC : Cond
    {
        readonly Cond a, b; public AndC(Cond a, Cond b) { this.a = a; this.b = b; Src = a.Src + " và " + b.Src; }
        public override bool Eval(GameState s) => a.Eval(s) && b.Eval(s);
    }
    sealed class OrC : Cond
    {
        readonly Cond a, b; public OrC(Cond a, Cond b) { this.a = a; this.b = b; Src = a.Src + " hoặc " + b.Src; }
        public override bool Eval(GameState s) => a.Eval(s) || b.Eval(s);
    }

    public static Cond Not(Cond c) => new NotC(c);
    public static Cond And(Cond a, Cond b) => new AndC(a, b);
    public static Cond Or(Cond a, Cond b) => new OrC(a, b);
    public static readonly Cond Never = new Fn("không bao giờ", _ => false);
    public static Cond FlagIs(string name, string value) =>
        new Fn($"{name}={value}", s => s.Flag(name) == value);
    public static Cond FlagIn(string name, params string[] values) =>
        new Fn($"{name} in [{string.Join(",", values)}]", s => values.Contains(s.Flag(name)));

    // ───────────── bộ phân tích ─────────────

    static readonly (Regex re, string tok)[] Phrases =
    {
        // ghi chú, vật phẩm, câu đã nói (phần trong ngoặc kép đã được thay bằng §Qn)
        (new Regex(@"ở Câu 5 đã không gọi\s+§Q\d+"), " §SAI5 "),
        (new Regex(@"ở Cảnh 1 Kael đã nói\s+§Q\d+"), " §HENTHAO "),
        (new Regex(@"(?:ở Chương \d+ Kael )?có ghi chú\s+§Q(\d+)"), " §NOTE$1 "),
        (new Regex(@"có vật phẩm\s+§Q(\d+)"), " §ITEM$1 "),
        // bà Helena
        (new Regex(@"kết với bà Helena(?: \(?ở Chương 1\)?)? là Thuyết phục"), " §KH_thuyet_phuc "),
        (new Regex(@"kết với bà Helena(?: \(?ở Chương 1\)?)? là Bất phân"), " §KH_bat_phan "),
        (new Regex(@"kết với bà Helena(?: \(?ở Chương 1\)?)? là Bị thuyết phục"), " §KH_bi_thuyet_phuc "),
        (new Regex(@"kết với bà Helena(?: \(?ở Chương 1\)?)? khác Thuyết phục"), " §NOT §KH_thuyet_phuc "),
        (new Regex(@"Kael là người của bà Helena"), " §KH_bi_thuyet_phuc "),
        (new Regex(@"có lính gác của bà Helena"), " §KH_thuyet_phuc "),
        (new Regex(@"bà Helena là đồng minh"), " §HE_dong_minh "),
        (new Regex(@"bà Helena đứng ngoài"), " §HE_dung_ngoai "),
        (new Regex(@"bà Helena chưa gặp Veritas"), " §CHUAGAP "),
        // Vane
        (new Regex(@"ở Chương 2 Kael thuyết phục được Vane"), " §TV "),
        (new Regex(@"Kael không dùng đòn câu 4 ở Chương 2"), " §NOT §DON4 "),
        (new Regex(@"Vane không phải đồng minh"), " §NOT §WIN "),
        (new Regex(@"Vane là đồng minh"), " §WIN "),
        (new Regex(@"Vane đứng ngoài"), " §DRAW "),
        (new Regex(@"(?:Kael\s+)?không bị (?:Vane )?thuyết phục"), " §NOTLOSE "),
        (new Regex(@"(?:Kael\s+)?bị Vane thuyết phục"), " §LOSE "),
        (new Regex(@"Kael bị thuyết phục"), " §LOSE "),
        (new Regex(@"thuyết phục được Vane"), " §WIN "),
        (new Regex(@"\bthắng(?: Vane)?"), " §WIN "),
        (new Regex(@"bất phân(?: với Vane)?"), " §DRAW "),
        // thư ký, cái lồng, Tầng Đỉnh
        (new Regex(@"thư ký chưa đòi Kael lên"), " §NOT §DOILEN "),
        (new Regex(@"thư ký đã đòi Kael lên"), " §DOILEN "),
        (new Regex(@"(?:ở Cảnh 3 )?thư ký đã đòi thiết bị(?: cổ tay)?"), " §DOITB "),
        (new Regex(@"không ai bước vào lồng theo Kael"), " §NOT §THEO "),
        (new Regex(@"có người bước vào lồng theo Kael"), " §THEO "),
        (new Regex(@"có người đi cùng Kael"), " §DICUNG "),
        (new Regex(@"Kael không bị giữ"), " §NOT §GIU "),
        (new Regex(@"Kael bị giữ"), " §GIU "),
        (new Regex(@"Tầng Đáy đông"), " §DAYDONG "),
        (new Regex(@"Tầng Đáy thưa"), " §NOT §DAYDONG "),
        (new Regex(@"Tầng Trung đông"), " §TRUNGDONG "),
        (new Regex(@"Tầng Trung thưa"), " §NOT §TRUNGDONG "),
        (new Regex(@"có người Tầng Trung lên"), " §TRUNGLEN "),
        (new Regex(@"vào một mình"), " §MOTMINH "),
        // Chương 3
        (new Regex(@"đã nối mắt xích (\d)"), " §MX$1 "),
        (new Regex(@"đã gắn điều 3(?: ở Cảnh 3)?"), " §DIEU3 "),
        (new Regex(@"sổ đủ"), " §SODU "),
        (new Regex(@"sổ thiếu"), " §NOT §SODU "),
        (new Regex(@"có cờ bac_dung"), " §BAC "),
        (new Regex(@"(?:ở )?[Bb]ản trên"), " §TREN "),
        (new Regex(@"(?:ở )?[Bb]ản dưới"), " §NOT §TREN "),
        (new Regex(@"(?:lối )?\b([TD][123])\b"), " §LV$1 "),
        // giấy tờ, Rian
        (new Regex(@"(?:ở Chương 2 Kael )?(?:đang )?(?:cầm\s+)?Thẻ đặc phái(?: của Ban Cố vấn)?"), " §DP "),
        (new Regex(@"(?:đang )?(?:cầm\s+)?Giấy thông hành(?: hai chiều)?"), " §TH "),
        (new Regex(@"(?:đang )?(?:cầm\s+)?Lệnh điều chuyển(?: một chiều)?"), " §DC "),
        // Chương 3 mở theo nơi Kael đứng khi hết Chương 2 (HUONG_DAN_DEV_CHUONG_2.md mục 13)
        (new Regex(@"Kael (?:ở lại xưởng|còn ở xưởng)"), " §NOT §TREN "),
        (new Regex(@"Kael (?:đã bước vào thang|đã lên thang|đang đi lên)"), " §TREN "),
        (new Regex(@"Rian đã nghe chuyện mười hai người"), " §RIAN "),
        (new Regex(@"Rian (?:đã gạt đi|chưa nghe)"), " §NOT §RIAN "),
        // biến viết thẳng tên
        (new Regex(@"\b([a-z][a-z_0-9]*) = ([A-Za-z_0-9]+)"), " §EQ:$1:$2 "),
        (new Regex(@"\b([a-z][a-z_0-9]*) từ (\d+) tới (\d+)"), " §RG:$1:$2:$3 "),
        (new Regex(@"\b([a-z][a-z_0-9]*) từ (\d+) trở lên"), " §GE:$1:$2 "),
        (new Regex(@"\b([a-z][a-z_0-9]*) dưới (\d+)"), " §LT:$1:$2 "),
        // lựa chọn
        (new Regex(@"(?<!\d)(\d{1,2})-([ABC])\b"), " §L:$1:$2 "),
        (new Regex(@"không (?:phải|chọn)\b"), " §NOT "),
        (new Regex(@"đã chọn\b"), " "),
    };

    [ThreadStatic] static string? lastItem;

    /// <summary>Trả về null nếu không hiểu; ghi lý do vào <paramref name="error"/>.</summary>
    public static Cond? Parse(string text, out string? error)
    {
        error = null;
        var t = text.Trim();
        t = Regex.Replace(t, @"^Nếu\s+", "");
        var quoted = new List<string>();
        t = Regex.Replace(t, "\"([^\"]*)\"", m => { quoted.Add(m.Groups[1].Value); return " §Q" + (quoted.Count - 1) + " "; });
        // "không có vật phẩm ấy": vật phẩm vừa được nhắc ở điều kiện ngay phía trên
        var item = Regex.Match(t, @"có vật phẩm\s+§Q(\d+)");
        if (item.Success) lastItem = quoted[int.Parse(item.Groups[1].Value)];
        else if (t.Contains("có vật phẩm ấy") && lastItem != null)
        {
            quoted.Add(lastItem);
            t = t.Replace("có vật phẩm ấy", "có vật phẩm §Q" + (quoted.Count - 1));
        }
        // phần trong ngoặc có nghĩa: "(và không chọn 5-A)"
        t = Regex.Replace(t, @"\(\s*và không chọn (\d{1,2}-[ABC])\s*\)", " và không chọn $1 ");
        // phần trong ngoặc còn lại chỉ là chú thích
        t = Regex.Replace(t, @"\([^)]*\)", " ");
        foreach (var (re, tok) in Phrases) t = re.Replace(t, tok);
        // dấu phẩy tách các vế lớn: "cầm Giấy thông hành hoặc Thẻ đặc phái, và thắng" = (A hoặc B) và C; "7-C, hoặc 7-B và X" = A hoặc (B và X)
        t = Regex.Replace(t, @",\s*hoặc\b", " §COR ");
        t = Regex.Replace(t, @",\s*và\b", " §CAND ");
        t = t.Replace(",", " §CAND ");
        var toks = Regex.Split(t, @"\s+").Where(x => x.Length > 0).ToList();
        int i = 0;
        try
        {
            var c = ParseOr(toks, ref i, quoted);
            while (i < toks.Count && toks[i] is "§CAND" or "§COR")
            {
                bool and = toks[i++] == "§CAND";
                var r = ParseOr(toks, ref i, quoted);
                c = and ? new AndC(c, r) : new OrC(c, r);
            }
            if (i != toks.Count) throw new FormatException("thừa chữ: " + string.Join(" ", toks.Skip(i)));
            c.Src = text.Trim();
            return c;
        }
        catch (FormatException e) { error = e.Message + " | " + text; return null; }
    }

    // "và" buộc chặt hơn "hoặc": "7-C, hoặc 7-B và Kael bị Vane thuyết phục" là 7-C hoặc (7-B và bị thuyết phục)
    static Cond ParseOr(List<string> t, ref int i, List<string> q)
    {
        var a = ParseAnd(t, ref i, q);
        while (i < t.Count && t[i] == "hoặc") { i++; a = new OrC(a, ParseAnd(t, ref i, q)); }
        return a;
    }

    static Cond ParseAnd(List<string> t, ref int i, List<string> q)
    {
        var a = ParseAtom(t, ref i, q);
        while (i < t.Count && t[i] == "và") { i++; a = new AndC(a, ParseAtom(t, ref i, q)); }
        return a;
    }

    static bool IsNo(GameState s, string flag) => s.Flag(flag) is "" or "khong";
    static bool AnyFollow(GameState s) => s.Flag("rian_len") == "co" || s.Flag("vane_len") == "co" || s.Flag("tao_len") == "co";
    static string Ket(GameState s) => s.Flag("ket_doi_chat_helena");

    static Cond ParseAtom(List<string> t, ref int i, List<string> q)
    {
        if (i >= t.Count) throw new FormatException("thiếu vế");
        var w = t[i++];
        if (w is "§NOT" or "không") return new NotC(ParseAtom(t, ref i, q));
        var p = w.Split(':');
        switch (p[0])
        {
            case "§L":
            {
                var name = "lua_chon_" + p[1]; var val = p[2];
                return new Fn(w, s => s.Flag(name) == val);
            }
            case "§EQ":
            {
                var name = p[1]; var val = p[2];
                if (int.TryParse(val, out var n)) return new Fn(w, s => s.Num(name) == n);
                if (val == "khong") return new Fn(w, s => IsNo(s, name));
                return new Fn(w, s => s.Flag(name) == val);
            }
            case "§GE": { var name = p[1]; int n = int.Parse(p[2]); return new Fn(w, s => s.Num(name) >= n); }
            case "§LT": { var name = p[1]; int n = int.Parse(p[2]); return new Fn(w, s => s.Num(name) < n); }
            case "§RG": { var name = p[1]; int a = int.Parse(p[2]), b = int.Parse(p[3]); return new Fn(w, s => s.Num(name) >= a && s.Num(name) <= b); }
        }
        var m = Regex.Match(w, @"^§(NOTE|ITEM)(\d+)$");
        if (m.Success)
        {
            var name = q[int.Parse(m.Groups[2].Value)].TrimEnd('…', '.', ' ');
            return m.Groups[1].Value == "NOTE" ? new Fn(w, s => s.HasNote(name)) : new Fn(w, s => s.HasItem(name));
        }
        m = Regex.Match(w, @"^§KH_(\w+)$");
        if (m.Success) { var v = m.Groups[1].Value; return new Fn(w, s => Ket(s) == v); }
        m = Regex.Match(w, @"^§HE_(\w+)$");
        if (m.Success) { var v = m.Groups[1].Value; return new Fn(w, s => s.Flag("helena") == v); }
        m = Regex.Match(w, @"^§LV([TD]\d)$");
        if (m.Success) { var v = m.Groups[1].Value; return new Fn(w, s => s.Flag("loi_vao") == v); }
        m = Regex.Match(w, @"^§MX(\d)$");
        if (m.Success) { var v = "mat_xich_" + m.Groups[1].Value; return new Fn(w, s => s.Flag(v) == "co"); }
        return w switch
        {
            "§DP" => new Fn(w, s => s.GiayTo == "dac_phai"),
            "§TH" => new Fn(w, s => s.GiayTo == "thong_hanh"),
            "§DC" => new Fn(w, s => s.GiayTo == "dieu_chuyen"),
            "§WIN" => new Fn(w, s => s.Flag("vane") == "dong_minh"),
            "§DRAW" => new Fn(w, s => s.Flag("vane") == "dung_ngoai"),
            "§LOSE" => new Fn(w, s => s.Flag("vane") is "chu" or "ket_cuc_som"),
            "§NOTLOSE" => new Fn(w, s => s.Flag("vane") is not ("chu" or "ket_cuc_som")),
            "§TV" => new Fn(w, s => s.Flag("thang_vane") == "co"),
            "§DON4" => new Fn(w, s => s.Flag("da_dung_don_cau_4") == "co"),
            // bản trên: Giấy thông hành / Thẻ đặc phái và không bị Vane thuyết phục; mọi trường hợp khác là bản dưới
            "§TREN" => new Fn(w, s => s.BanTren),
            "§RIAN" => new Fn(w, s => s.HasNote("Rian đã nghe")),
            "§SODU" => new Fn(w, s => s.Var("so_cai") >= 4),
            "§DIEU3" => new Fn(w, s => s.Flag("dieu_3") == "gan"),
            "§BAC" => new Fn(w, s => s.Flag("bac_dung") == "co"),
            "§CHUAGAP" => new Fn(w, s => s.Flag("loi_vao") is not ("T1" or "T2")),
            // Chương 4 mục 3: tính theo lo lúc vào Cảnh 3
            "§DOILEN" => new Fn(w, s => Ket(s) != "thuyet_phuc" && (s.Flag("lo_truoc_c3") == "co" || s.Flag("lua_chon_8") == "B")),
            "§DOITB" => new Fn(w, s => Ket(s) != "thuyet_phuc" && s.Flag("lo_truoc_c3") == "co"),
            "§THEO" => new Fn(w, AnyFollow),
            // Chương 5 mục 3
            "§DICUNG" => new Fn(w, s => AnyFollow(s) || Ket(s) == "thuyet_phuc" || s.Flag("nha_len") == "kip"),
            "§GIU" => new Fn(w, s => s.Flag("vane_len") != "co" && Ket(s) != "thuyet_phuc" && s.Flag("nha_len") != "kip"),
            "§HENTHAO" => new Fn(w, s => Ket(s) != "thuyet_phuc" && s.Flag("lo_truoc_c3") == "co" && s.Flag("vane_len") != "co"),
            "§DAYDONG" => new Fn(w, s => s.Var("tang_day") >= 3),
            "§TRUNGDONG" => new Fn(w, s => s.Var("tang_trung") >= 1),
            "§TRUNGLEN" => new Fn(w, s => s.Var("tang_trung") >= 1 || s.Flag("nha_len") == "kip"),
            "§MOTMINH" => new Fn(w, s => IsNo(s, "vao_day") && IsNo(s, "vao_trung")),
            "§SAI5" => new Fn(w, s => s.Flag("sai_cau_5") == "co"),
            _ => throw new FormatException("không hiểu '" + w + "'")
        };
    }
}
