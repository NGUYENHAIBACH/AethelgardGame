namespace AethelgardGame.Models.Game;

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

    // Cảnh đang hiển thị (để tải lại ván là dựng lại đúng màn hình)
    public string? Bg { get; set; }
    public string? Bgm { get; set; }
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

    /// <summary>"giấy tờ" Kael cầm sau Chương 1: suy từ kết quả đối chất với Helena.</summary>
    public string GiayTo => Flag("ket_doi_chat_helena") switch
    {
        "thuyet_phuc" => "thong_hanh",
        "bat_phan" => "dieu_chuyen",
        "bi_thuyet_phuc" => "dac_phai",
        _ => ""
    };
}
