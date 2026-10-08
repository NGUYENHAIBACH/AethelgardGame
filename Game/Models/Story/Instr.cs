namespace AethelgardGame.Models.Story;

/// <summary>Một lệnh trong chương trình đã biên dịch từ file kịch bản .md.
/// Chương trình là một danh sách phẳng; "pc" (số thứ tự lệnh) là thứ duy nhất cần lưu để biết đang ở đâu.</summary>
public abstract class Instr
{
    public int Line;   // dòng trong file .md, để báo lỗi
}

public enum SayKind { Say, Think, Narr, Stage, Loa, Board }

/// <summary>Một dòng hiện trong khung thoại.</summary>
public sealed class SayI : Instr
{
    public SayKind Kind;
    public string Name = "";
    public string Text = "";
    public bool Loud;         // (to tiếng)
    public bool Whisper;      // (rất khẽ), (thì thầm)
    public bool WhisperTren;  // Chương 3 Cảnh 2, 3: ở bản trên (giữa xưởng lưu trữ, có camera) Kael và Veritas chỉ thì thầm
    public bool WristTeal;    // dòng tả biểu tượng cổ tay nháy xanh ngọc
}

/// <summary>Sprite: "Tên_BiểuCảm" (lên sân khấu hoặc đổi biểu cảm). SpriteOff: tên người rời đi, null = gỡ hết.
/// Expr: "Tên_BiểuCảm", chỉ đổi biểu cảm nếu người ấy đang có mặt.</summary>
public enum CmdKind { Bg, Bgm, Se, Sprite, SpriteOff, Expr, Card, SceneStart, Elevator }

public sealed class CmdI : Instr
{
    public CmdKind Kind;
    public string? Value;
}

public sealed class AddItemI : Instr { public string Name = ""; public string Desc = ""; }
public sealed class AddNoteI : Instr { public string Name = ""; }
public sealed class OpenShardI : Instr { public List<int> Numbers = new(); }
public sealed class RemoveItemI : Instr { public string Name = ""; }

/// <summary>Một thay đổi biến, có thể có điều kiện.</summary>
public sealed class Eff
{
    public string Var = "";
    public int Delta;
    public string? SetFlagValue;   // != null: đặt biến chuỗi thay vì cộng
    public Cond? If;
    public Cond? Unless;
}

public sealed class EffectI : Instr
{
    public List<Eff> Effs = new();
    public List<int> Shards = new();
    public string? NoteToAdd;
}

public sealed class JumpIfNotI : Instr { public Cond Cond = null!; public int Target; }
public sealed class JumpI : Instr { public int Target; }

/// <summary>Nhảy tới một nhãn (tiêu đề ## hoặc ###). ClearStack: bỏ mọi địa chỉ quay về đang chờ.</summary>
public sealed class GotoI : Instr { public string Label = ""; public int Target = -1; public bool ClearStack; }
/// <summary>Gọi một đoạn như chương trình con ("→ Chạy "Khi một chốt vỡ""), chạy xong thì quay về.</summary>
public sealed class CallI : Instr { public string Label = ""; public int Target = -1; }
public sealed class ReturnI : Instr { }
/// <summary>Điểm quay lại không gắn với một lựa chọn đánh số (đầu trận cuối).</summary>
public sealed class CheckpointI : Instr { public int Id; }

public sealed class ChoiceOption
{
    public char Key;
    public string Text = "";
    public Cond? Cond;
    public List<Eff> Effs = new();
}

public sealed class AskChoiceI : Instr
{
    public int Id;
    public List<ChoiceOption> Options = new();
}

/// <summary>Một thứ khớp với mắt xích (Chương 3 Cảnh 2): tên ghi chú hoặc vật phẩm bắt đầu bằng Key thì chạy từ Addr.
/// Pseudo: dòng không nằm trong sổ tay, tự thêm vào danh sách khi điều kiện đúng ("Tấm bảng ở quầy suất").</summary>
public sealed class PickAnswer { public string Key = ""; public int Addr = -1; public Cond? Pseudo; }

/// <summary>Chương 3 Cảnh 2: Veritas hỏi, người chơi chọn một ghi chú hoặc vật phẩm đang có.</summary>
public sealed class AskNoteI : Instr
{
    public int Slot;                              // mắt xích 1..4
    public List<PickAnswer> Answers = new();      // Addr = -1: chạy tiếp ngay sau lệnh này
    public int NoneAddr = -1;                     // đoạn "Nếu chọn Không có gì"; -1 = không bỏ trống được
    public List<string> Wrong = new();            // lời Veritas khi chọn sai, xoay vòng
    public string Remind = "";
}

/// <summary>Chương 3 Cảnh 3: Veritas kể một điều, người chơi chọn mắt xích để gắn vào.</summary>
public sealed class AskLinkI : Instr
{
    public int Slot;                              // điều 1..5
    public List<string> Links = new();            // tên sáu mắt xích của Cảnh 2
    public List<string> Wrong = new();
    public string Empty = "", HasPlace = "", NoMatchV = "", NoMatchK = "";
}

/// <summary>Chương 5 Cảnh 3: "Gọi ai?", năm nút cố định.</summary>
public sealed class AskCallI : Instr
{
    public int Cau;
    public string Correct = "";
    public List<string> Names = new();
}

public sealed class BattleOption
{
    public string Key = "";          // A, B, C, Lùi, Giữ lời, Đòn
    public string? Variant;          // mạnh | yếu | null
    public string? RequiresNote;
    public string Text = "";
    public int Addr;
}

public sealed class AskBattleI : Instr { public List<BattleOption> Options = new(); }

public sealed class SpecialI : Instr { public string Name = ""; }

public sealed class EndScreenI : Instr
{
    public string Title = "";
    public List<string> Lesson = new();
    public bool NoRetry;   // kịch bản không ghi "(có nút quay lại điểm chọn)": Kết cục 7/7
}

public sealed class ChapterEndI : Instr { }

public sealed class Chapter
{
    public string File = "";
    public string Title = "";
    public int Index;
    public List<Instr> Code = new();
    public List<int> ChoiceIds = new();
    public bool StanceCounting;
    public string? BattleKind;   // helena | vane
    public List<string> Skipped = new();   // dòng ghi chú soạn thảo đã bỏ qua (để người viết rà lại)
}

public sealed class ShardDef
{
    public int Number;
    public string Title = "";
    public List<string> Paras = new();
}
