using System.Text.RegularExpressions;

namespace AethelgardGame.Models.Story;

public enum BT { H1, H2, H3, H4, Bold, Bullet, Quote, Plain }

/// <summary>Một "khối" dòng trong file .md: tiêu đề, dòng in đậm, gạch đầu dòng, khối trích dẫn (>), hoặc dòng thường.</summary>
public sealed class Block
{
    public BT Type;
    public int Indent;                       // chỉ dùng cho Bullet: số cấp thụt (2 dấu cách = 1 cấp)
    public string Text = "";
    public List<string> Paras = new();       // chỉ dùng cho Quote: các đoạn, tách bởi dòng ">" trống
    public int Line;
    public bool Blank;                       // có dòng trống ngay phía trên (ngắt một chuỗi "Nếu ... / Nếu không")
}

public static class BlockReader
{
    public static List<Block> Read(string[] lines)
    {
        var blocks = new List<Block>();
        Block? quote = null;
        bool newPara = true, blank = true;
        void Add(Block b) { b.Blank = blank; blank = false; blocks.Add(b); }
        for (int n = 0; n < lines.Length; n++)
        {
            var raw = lines[n].TrimEnd('\r');
            var trimmed = raw.Trim();
            if (trimmed.Length == 0) { quote = null; blank = true; continue; }

            if (trimmed.StartsWith('>'))
            {
                var body = trimmed.Length > 1 ? trimmed[1..].TrimStart() : "";
                if (quote == null)
                {
                    quote = new Block { Type = BT.Quote, Line = n + 1 };
                    Add(quote);
                    newPara = true;
                }
                if (body.Length == 0) newPara = true;          // dòng ">" trống: ngắt đoạn
                else if (newPara || quote.Paras.Count == 0) { quote.Paras.Add(body); newPara = false; }
                else quote.Paras[^1] += " " + body;            // dòng liền kề: cùng đoạn
                continue;
            }
            quote = null;

            if (trimmed.StartsWith("#### ")) { Add(new Block { Type = BT.H4, Text = trimmed[5..].Trim(), Line = n + 1 }); continue; }
            if (trimmed.StartsWith("### ")) { Add(new Block { Type = BT.H3, Text = trimmed[4..].Trim(), Line = n + 1 }); continue; }
            if (trimmed.StartsWith("## ")) { Add(new Block { Type = BT.H2, Text = trimmed[3..].Trim(), Line = n + 1 }); continue; }
            if (trimmed.StartsWith("# ")) { Add(new Block { Type = BT.H1, Text = trimmed[2..].Trim(), Line = n + 1 }); continue; }

            // nhãn in đậm đứng riêng một dòng, có thể kèm chú thích trong ngoặc: **Veritas đồng hành** (có cờ bac_dung, ...)
            var mb = Regex.Match(trimmed, @"^\*\*([^*]+)\*\*(\s*\(.*\))?$");
            if (mb.Success) { Add(new Block { Type = BT.Bold, Text = (mb.Groups[1].Value.Trim() + " " + mb.Groups[2].Value.Trim()).Trim(), Line = n + 1 }); continue; }

            var mu = Regex.Match(raw, @"^(\s*)- (.*)$");
            if (mu.Success)
            {
                Add(new Block { Type = BT.Bullet, Indent = mu.Groups[1].Value.Length / 2, Text = mu.Groups[2].Value.Trim(), Line = n + 1 });
                continue;
            }
            Add(new Block { Type = BT.Plain, Text = trimmed, Line = n + 1 });
        }
        return blocks;
    }
}
