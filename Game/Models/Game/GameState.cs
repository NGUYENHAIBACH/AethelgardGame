namespace AethelgardGame.Models.Game;

/// <summary>Một người đang đứng trên sân khấu. Name là tiền tố file sprite (Kael, Helena, Vane, Rian, Me, Doran...).</summary>
public sealed class Actor
{
    public string Name { get; set; } = "";
    public string Expr { get; set; } = "Neutral";
    public int LastSpoke { get; set; }
}

public sealed class ItemRec
{
    public string Name { get; set; } = "";
    public string Desc { get; set; } = "";
}

/// <summary>Toàn bộ trạng thái một ván chơi. Máy chủ không giữ gì giữa hai request:
/// trình duyệt gửi lại chuỗi JSON của lớp này, nên lưu/tải và chơi lại từ điểm chọn đều rẻ.</summary>
public sealed class GameState
{
    public int Chapter { get; set; }
    public int Pc { get; set; }

    // Lựa chọn: lua_chon_0..5, ket_doi_chat_helena, giay_to, vane, han_vane, da_dung_don_cau_4, rian_da_nghe...
    public Dictionary<string, string> Flags { get; set; } = new();
    // Số: 5 khuynh hướng, 2 lực lượng, lung_lay, dao_dong, 5 bộ đếm lập trường. Người chơi không bao giờ thấy.
    public Dictionary<string, int> Vars { get; set; } = new();

    public List<ItemRec> Items { get; set; } = new();
    public List<string> Notes { get; set; } = new();
    public List<int> Shards { get; set; } = new();
    // Địa chỉ quay về của các đoạn được gọi như chương trình con (Chương 5: "Khi một chốt vỡ")
    public List<int> Ret { get; set; } = new();
    // Thứ tự cộng điểm năm khuynh hướng (cái cộng sau cùng đứng cuối), để phá hòa ở khối "Lối bạn hay chọn"
    public List<string> TrendOrder { get; set; } = new();
    public string? Amb { get; set; }       // hiệu ứng chạy lặp đang phát (SE07_Mua), tắt khi đổi nền

    // Cảnh đang hiển thị (để tải lại ván là dựng lại đúng màn hình)
    public string? Bg { get; set; }
    public string? Bgm { get; set; }
    // Sân khấu (HUONG_DAN_DEV_SAN_KHAU.md): danh sách người theo thứ tự trái sang phải, tối đa ba; hình chiếu Veritas và bóng người phụ đứng riêng
    public List<Actor> Stage { get; set; } = new();
    public string? Holo { get; set; }      // null = tắt; "Hologram" | "Glitch" | "Smile" | "Serious"
    public string? Shadow { get; set; }    // null hoặc tên file bóng ("Bong_Tho_gia")
    public int Tick { get; set; }          // đồng hồ lượt thoại, cho luật xoay vòng
    /// <summary>Trường của bản lưu cũ (một ô sprite). Chỉ đọc vào rồi đổi sang Stage; không ghi ra nữa.</summary>
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public string? Sprite { get; set; }
    public bool Bars { get; set; }
    public int LMax { get; set; }
    public int DMax { get; set; }
    public bool Elev { get; set; }

    // choiceId -> JSON trạng thái ngay trước khi chọn (cho nút "quay lại điểm chọn")
    public Dictionary<string, string> Checkpoints { get; set; } = new();

    public string Flag(string name) => Flags.TryGetValue(name, out var v) ? v : "";
    public int Var(string name) => Vars.TryGetValue(name, out var v) ? v : 0;
    public void Add(string name, int delta) => Vars[name] = Var(name) + delta;
    public bool HasNote(string prefix) => Notes.Any(n => n.StartsWith(prefix, StringComparison.Ordinal));
    public bool HasItem(string prefix) => Items.Any(n => n.Name.StartsWith(prefix, StringComparison.Ordinal));

    /// <summary>Biến số dùng trong điều kiện. so_thung suy ra theo bảng ở HUONG_DAN_DEV_CHUONG_4.md mục 3.</summary>
    public int Num(string name)
    {
        if (name != "so_thung") return Var(name);
        bool chu = Flag("vane") == "chu";
        return Flag("lua_chon_7") switch { "A" => 5, "B" => chu ? 2 : 0, "C" => chu ? 4 : 3, _ => 0 };
    }

    /// <summary>Bản trên của Chương 3: Giấy thông hành hoặc Thẻ đặc phái, và không bị Vane thuyết phục.</summary>
    public bool BanTren => GiayTo is "thong_hanh" or "dac_phai" && Flag("vane") is "dong_minh" or "dung_ngoai";

    // ───────────── sân khấu

    public Actor? OnStage(string name) => Stage.FirstOrDefault(a => a.Name == name);

    /// <summary>Đưa một người lên (hoặc chỉ đổi biểu cảm nếu đã có mặt). Đủ ba người thì người lâu nhất chưa nói, trừ Kael, lùi ra.</summary>
    public void Enter(string name, string expr)
    {
        var a = OnStage(name);
        if (a != null) { a.Expr = expr; return; }
        if (Stage.Count >= 3)
        {
            var oldest = Stage.Where(x => x.Name != "Kael").OrderBy(x => x.LastSpoke).First();
            Stage.Remove(oldest);
        }
        a = new Actor { Name = name, Expr = expr, LastSpoke = ++Tick };
        if (Stage.Count == 2) Stage.Insert(1, a); else Stage.Add(a);   // người thứ ba chen vào giữa
    }

    public void Spoke(string name)
    {
        if (OnStage(name) == null) Enter(name, "Neutral");
        OnStage(name)!.LastSpoke = ++Tick;
    }

    public void SetExpr(string name, string expr) { var a = OnStage(name); if (a != null) a.Expr = expr; }

    public void Leave(string name)
    {
        Stage.RemoveAll(a => a.Name == name);
        if (name == "Kael") Holo = null;   // hình chiếu ở trên cổ tay anh
    }

    public void HoloOn(string expr)
    {
        Holo = expr;
        if (OnStage("Kael") == null) Enter("Kael", "Neutral");
    }

    public void ClearStage() { Stage.Clear(); Holo = null; Shadow = null; }

    /// <summary>Bản lưu cũ chỉ có một chuỗi Sprite: đổi sang sân khấu.</summary>
    public void UpgradeLegacy()
    {
        if (Sprite == null) return;
        var sp = Sprite; Sprite = null;
        if (Stage.Count > 0 || Holo != null) return;
        int k = sp.IndexOf('_'); if (k <= 0) return;
        if (sp.StartsWith("Veritas_")) HoloOn(sp[(k + 1)..]); else Enter(sp[..k], sp[(k + 1)..]);
    }

    /// <summary>"giấy tờ" Kael cầm sau Chương 1: suy từ kết quả đối chất với Helena.</summary>
    public string GiayTo => Flag("ket_doi_chat_helena") switch
    {
        "thuyet_phuc" => "thong_hanh",
        "bat_phan" => "dieu_chuyen",
        "bi_thuyet_phuc" => "dac_phai",
        _ => ""
    };
}
