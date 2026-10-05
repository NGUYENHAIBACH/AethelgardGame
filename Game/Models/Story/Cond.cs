using System.Text.RegularExpressions;
using AethelgardGame.Models.Game;

namespace AethelgardGame.Models.Story;

/// <summary>Điều kiện rẽ nhánh, đọc từ chữ trong kịch bản ("Nếu 1-C", "cầm Thẻ đặc phái", "thắng hoặc bất phân"...).</summary>
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
    public static Cond FlagIs(string name, string value) =>
        new Fn($"{name}={value}", s => s.Flag(name) == value);
    public static Cond FlagIn(string name, params string[] values) =>
        new Fn($"{name} in [{string.Join(",", values)}]", s => values.Contains(s.Flag(name)));

    // ───────────── bộ phân tích ─────────────

    static readonly (Regex re, string tok)[] Phrases =
    {
        (new Regex(@"(?:cầm\s+)?Thẻ đặc phái(?: của Ban Cố vấn)?"), " §DP "),
        (new Regex(@"(?:cầm\s+)?Giấy thông hành(?: hai chiều)?"), " §TH "),
        (new Regex(@"(?:cầm\s+)?Lệnh điều chuyển(?: một chiều)?"), " §DC "),
        (new Regex(@"(?:Kael\s+)?không bị thuyết phục"), " §NOTLOSE "),
        (new Regex(@"Kael bị thuyết phục"), " §LOSE "),
        (new Regex(@"thuyết phục được Vane"), " §WIN "),
        (new Regex(@"\bthắng\b"), " §WIN "),
        (new Regex(@"bất phân"), " §DRAW "),
        // Chương 3 mở theo nơi Kael đứng khi hết Chương 2 (HUONG_DAN_DEV_CHUONG_2.md mục 13)
        (new Regex(@"Kael (?:ở lại xưởng|còn ở xưởng)"), " §STAY "),
        (new Regex(@"Kael (?:đã bước vào thang|đã lên thang|đang đi lên)"), " §UP "),
        (new Regex(@"Rian đã nghe chuyện mười hai người"), " §RIAN "),
        (new Regex(@"Rian đã gạt đi"), " §NORIAN "),
        (new Regex(@"(\d)-([ABC])"), " §L$1$2 "),
    };

    /// <summary>Trả về null nếu không hiểu; ghi lý do vào <paramref name="error"/>.</summary>
    public static Cond? Parse(string text, out string? error)
    {
        error = null;
        var t = text.Trim();
        t = Regex.Replace(t, @"^Nếu\s+", "");
        t = Regex.Replace(t, @"^đã chọn\s+", "");
        // phần trong ngoặc có nghĩa: "(và không chọn 5-A)"
        t = Regex.Replace(t, @"\(\s*và không chọn (\d-[ABC])\s*\)", " và §NOT $1 ");
        // phần trong ngoặc còn lại chỉ là chú thích
        t = Regex.Replace(t, @"\([^)]*\)", " ");
        foreach (var (re, tok) in Phrases) t = re.Replace(t, tok);
        t = Regex.Replace(t, @"§NOT\s+(\d)-([ABC])", " §NOT §L$1$2 ");
        t = t.Replace(",", " ");
        var toks = Regex.Split(t, @"\s+").Where(x => x.Length > 0).ToList();
        int i = 0;
        try
        {
            var c = ParseAnd(toks, ref i);
            if (i != toks.Count) throw new FormatException("thừa chữ: " + string.Join(" ", toks.Skip(i)));
            c.Src = text.Trim();
            return c;
        }
        catch (FormatException e) { error = e.Message + " | " + text; return null; }
    }

    static Cond ParseAnd(List<string> t, ref int i)
    {
        var a = ParseOr(t, ref i);
        while (i < t.Count && t[i] == "và") { i++; a = new AndC(a, ParseOr(t, ref i)); }
        return a;
    }

    static Cond ParseOr(List<string> t, ref int i)
    {
        var a = ParseAtom(t, ref i);
        while (i < t.Count && t[i] == "hoặc") { i++; a = new OrC(a, ParseAtom(t, ref i)); }
        return a;
    }

    static Cond ParseAtom(List<string> t, ref int i)
    {
        if (i >= t.Count) throw new FormatException("thiếu vế");
        var w = t[i++];
        if (w == "§NOT") return new NotC(ParseAtom(t, ref i));
        // "không" đứng trước một vế khác: "Nếu không chọn..." hiếm gặp; "không" trần được xử lý ở nơi gọi
        if (w == "không") return new NotC(ParseAtom(t, ref i));
        if (w.StartsWith("§L") && w.Length == 4)
        {
            var name = "lua_chon_" + w[2]; var val = w[3].ToString();
            return new Fn(w, s => s.Flag(name) == val);
        }
        return w switch
        {
            "§DP" => new Fn(w, s => s.GiayTo == "dac_phai"),
            "§TH" => new Fn(w, s => s.GiayTo == "thong_hanh"),
            "§DC" => new Fn(w, s => s.GiayTo == "dieu_chuyen"),
            "§WIN" => new Fn(w, s => s.Flag("vane") == "dong_minh"),
            "§DRAW" => new Fn(w, s => s.Flag("vane") == "dung_ngoai"),
            "§LOSE" => new Fn(w, s => s.Flag("vane") is "chu" or "ket_cuc_som"),
            "§NOTLOSE" => new Fn(w, s => s.Flag("vane") is not ("chu" or "ket_cuc_som")),
            // bước vào thang: Giấy thông hành / Thẻ đặc phái và không bị Vane thuyết phục; mọi trường hợp khác ở lại xưởng
            "§UP" => new Fn(w, s => s.GiayTo != "dieu_chuyen" && s.Flag("vane") is "dong_minh" or "dung_ngoai"),
            "§STAY" => new Fn(w, s => !(s.GiayTo != "dieu_chuyen" && s.Flag("vane") is "dong_minh" or "dung_ngoai")),
            "§RIAN" => new Fn(w, s => s.HasNote("Rian đã nghe")),
            "§NORIAN" => new Fn(w, s => !s.HasNote("Rian đã nghe")),
            _ => throw new FormatException("không hiểu '" + w + "'")
        };
    }
}
