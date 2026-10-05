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
    public bool HideSprite;   // A.L.I.C.E (loa), (rất khẽ): có tiếng, không hiện sprite
    public bool Loud;         // (to tiếng)
    public bool Whisper;      // (rất khẽ), (thì thầm)
    public bool WristTeal;    // dòng tả biểu tượng cổ tay nháy xanh ngọc
}

public enum CmdKind { Bg, Bgm, Se, Sprite, Card, SceneStart, Elevator }

public sealed class CmdI : Instr
{
    public CmdKind Kind;
    public string? Value;
}

public sealed class AddItemI : Instr { public string Name = ""; public string Desc = ""; }
public sealed class AddNoteI : Instr { public string Name = ""; }
public sealed class OpenShardI : Instr { public List<int> Numbers = new(); }

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
