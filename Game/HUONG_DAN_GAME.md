# Aethelgard: game web ASP.NET Core MVC (C#, Visual Studio 2022)

Game visual novel dựa trên Giáo trình Triết học Mác – Lênin (2021). Chạy được trọn truyện: Mở đầu và Chương 1 đến 5, bảy kết cục, 32 mảnh, theo kịch bản và các file hướng dẫn trong `ban_giao_dev/`.

## Chạy game

1. Mở `AethelgardGame.sln` bằng Visual Studio 2022 (cần workload "ASP.NET and web development", .NET 8).
2. Bấm F5 (hoặc Ctrl+F5). Trình duyệt mở trang menu.
3. Hoặc dùng dòng lệnh: `dotnet run` trong thư mục này, rồi mở địa chỉ in ra.

Không cần cài thêm gói nào. Không cần cơ sở dữ liệu.

## Cấu trúc theo MVC

| Phần | Nằm ở đâu | Làm gì |
|---|---|---|
| **Model** | `Models/Story/` | Đọc file kịch bản `.md` thành chương trình chạy được (`Blocks.cs`, `Compiler.cs`, `Cond.cs`, `Instr.cs`, `StoryLibrary.cs`) |
| **Model** | `Models/Game/` | Trạng thái ván chơi (`GameState.cs`) và bộ máy chạy truyện, tính điểm, rẽ nhánh, chọn cách kết (`GameEngine.cs`) |
| **Controller** | `Controllers/GameController.cs` | Các action `Start`, `Choose`, `Continue`, `Resume`, `Retry`, `Manifest`, `Play` |
| **Controller** | `Controllers/HomeController.cs` | Trang menu |
| **Controller** | `Controllers/CheckController.cs` | Trang kiểm tra cho người làm game (chỉ chạy khi F5/Development) |
| **View** | `Views/Home/Index.cshtml`, `Views/Game/Play.cshtml`, `Views/Check/Index.cshtml` | Giao diện Razor |
| Giao diện động | `wwwroot/js/game.js`, `audio.js`, `saves.js`, `menu.js`, `wwwroot/css/game.css` | Chữ hiện dần, nút lựa chọn, sổ tay cổ tay, nhạc, lưu/tải |

Máy chủ **không giữ trạng thái**. Mỗi lần người chơi chọn, trình duyệt gửi lên chuỗi JSON `GameState`, máy chủ chạy kịch bản tới điểm dừng kế tiếp và trả về cả loạt dòng cần hiện. Nhờ vậy lưu/tải, "quay lại điểm chọn" và chạy trên máy chủ nào cũng được. Bản lưu nằm trong `localStorage` của trình duyệt.

## Kịch bản là nguồn duy nhất

Các file `Story/*.md` là bản sao của `ban_giao_dev/*.md`. Game đọc thẳng các file này, không chép chữ vào code. Sửa chữ trong `Story/*.md` rồi tải lại trang là thấy ngay (không cần biên dịch lại). Quy ước đọc kịch bản lấy theo `HUONG_DAN_DEV.md` mục 2 và `HUONG_DAN_DEV_CHUONG_2.md` mục 3.

**Thêm Chương 3:** đặt file `03_chuong_3.md` vào thư mục `Story/`, viết theo cùng quy ước. Game tự nhận, tự nối sau Chương 2. Trang `/Check` sẽ báo ngay dòng nào không đọc được. Các điều kiện đã dùng được: `Nếu 1-C`, `Nếu 0-A hoặc 0-C`, `cầm Thẻ đặc phái / Giấy thông hành / Lệnh điều chuyển`, `thắng / bất phân / Kael bị thuyết phục` (kết quả trận Vane), `Rian đã nghe chuyện mười hai người`, và hai điều kiện mới cho Chương 3: `Kael ở lại xưởng`, `Kael đã bước vào thang` (đúng bảng ở `HUONG_DAN_DEV_CHUONG_2.md` mục 13).

Luật cứng nằm trong code, không trong kịch bản: ngưỡng và biên độ hai thanh, thứ tự xét cách kết, bộ đếm lập trường (`GameEngine.cs`, hàm `Special`). Các luật này khớp mục 7 đến 9 của hai hướng dẫn dev.

## Trang kiểm tra (chỉ khi F5)

- `/Check`: kịch bản có dòng nào không đọc được không, còn thiếu file tài nguyên nào, chạy thử 500 ván ngẫu nhiên.
- `/Check/Coverage`: so từng dòng thoại trong `.md` với chữ trong game.
- `/Check/Skipped`: các dòng bị coi là ghi chú soạn thảo và không hiện cho người chơi.
- `/Check/Transcript?keys=B,C,B,C,C,C,C`: chơi theo danh sách phím và in ra toàn bộ chữ người chơi sẽ thấy.

## Tài nguyên

| Loại | Thư mục | Ghi chú |
|---|---|---|
| Nhạc | `wwwroot/assets/bgm` | `BGM01_...mp3` ... đúng tên trong kịch bản |
| Sprite | `wwwroot/assets/sprites` | `Kael_Neutral.png` ... gộp cả `Sprites` và `Sprites_Phu` |
| Nền | `wwwroot/assets/bg` | **Đang trống.** Thả `BG01_ThanhPho_Aethelgard_Dem.jpg` ... đúng tên (jpg, png, webp). Chưa có thì game vẽ nền tạm |
| Hiệu ứng | `wwwroot/assets/se` | **Đang trống.** Thả `SE01_Coi_Bao_Dong_Tram.mp3` ... đúng tên (mp3, ogg, wav). Chưa có thì game tự tạo âm thay thế |

## Phím tắt

Cách / Enter: đọc tiếp. Ctrl (giữ): bỏ qua. A: tự động. L: nhật ký. N: sổ tay cổ tay. Esc: đóng bảng.

## Đưa lên máy chủ

`dotnet publish -c Release -o publish`, chép thư mục `publish` lên máy chủ chạy .NET 8. Thư mục `Story` đi kèm. Trang `/Check` tự tắt ngoài chế độ Development.

## Cập nhật 08/10/2026: sân khấu, Chương 3 đến 5, màn kết

Sáu file `Story/00..05_*.md` là bản sao mới nhất của `ban_giao_dev/`. Khi người viết xuất lại kịch bản thì chép đè vào `Story/`, không sửa tay ở đây.

**Sân khấu** (`HUONG_DAN_DEV_SAN_KHAU.md`). `GameState` giữ `Stage` (tối đa ba người, trái sang phải), `Holo` (hình chiếu Veritas), `Shadow` (bóng người phụ). Luật nằm ở `GameState.Enter / Spoke / Leave / HoloOn / ClearStage` và `GameEngine.Speak`. Máy chủ gửi cả sân khấu trong bước `stage`; `game.js` (`setStage`) chỉ vẽ. Thêm người có sprite hoặc có bóng: sửa hai bảng `Actors`, `Shadows` trong `GameEngine.cs`.

**Điều kiện** (`Models/Story/Cond.cs`). Bảng `Phrases` đổi từng cụm chữ của kịch bản thành điều kiện. Biến viết thẳng tên đọc được luôn: `vane_len = co`, `so_thung từ 2 tới 4`, `vung dưới 6`. Một dãy gạch đầu dòng `Nếu …` liền nhau kết bằng `Nếu không` là chuỗi xét từ trên xuống, chạy khối đầu tiên khớp; dòng trống ngắt chuỗi.

**Tiêu đề có điều kiện** (`Compiler.cs`, `HeadCond`, `BoldCond`). `### Nhánh 6-A`, `### Bản trên: …`, `#### Trước mặt Helena (T1, T2)`, `**Lối T2: …**`, `**Tấm nhãn (chỉ khi …)**` mở một phạm vi kéo dài tới tiêu đề cùng cấp kế tiếp. `**Chung**`, `**Nhập lại**` đóng các nhãn in đậm phía trên.

**Màn chơi mới.**

- Chương 3 Cảnh 2 (chọn ghi chú) và Cảnh 3 (gắn lời kể): lệnh `AskNoteI`, `AskLinkI`; luật ở `GameEngine.ChooseNote`, `ChooseLink`. Lời Veritas khi chọn sai lấy từ khối `**Luật chọn**`, `**Luật gắn**` của kịch bản.
- Chương 5 Cảnh 3 ("Gọi ai?"): lệnh `AskCallI`; các khối `**Gọi "…" (đúng|sai)**` là phạm vi theo nút vừa bấm. `→ Chạy "Khi một chốt vỡ"` là lệnh gọi rồi quay về (`CallI`, `ReturnI`); `sang "Thắng"` là lệnh nhảy (`GotoI`).
- Màn kết cục: khối "Lối bạn hay chọn" (`GameEngine.TrendBlock`, chữ chép từ `HUONG_DAN_DEV_CHUONG_5.md` mục 10), nút quay lại theo từng kết cục (`RetryIds`), hình cuối ghép lớp (`FinalLayers`, ảnh ở `wwwroot/assets/ending`), rồi màn kết game.

**Biến suy ra, không có dòng nào trong kịch bản đặt:** `ban`, `loi_vao`, `thang_vane` (lệnh `init_c3`), `helena` (`init_c4`), `lo_truoc_c3` (`c4_scene3`), `veritas` (`resolve_veritas`); `so_thung`, `doi_len`, `doi_thiet_bi`, `bi_giu`, `hen_thao`, `co_nguoi_di_cung` tính ngay trong `Cond.cs` và `GameState.Num`.

**Tài nguyên:** mọi nền, nhạc, hiệu ứng, sprite và bóng mà kịch bản gọi tới đều đã có file (soát 08/10/2026). Thêm hoặc thay file thì thả đúng tên vào thư mục, game tự dùng.

**Kiểm tra sau mỗi lần đổi kịch bản** (chạy ở chế độ Development):

- `/Check`: phải không còn cảnh báo nào.
- `/Check/Simulate?runs=500&strategy=mix` (và `random`, `c`, `a`): không lỗi, tới đủ bảy kết cục.
- `/Check/Transcript?keys=…`: in cả sân khấu sau mỗi lần đổi. Ở màn chọn ghi chú, màn gắn và "Gọi ai?", phím là vài chữ có trong tên nút. Thêm `&state=1` để lấy trạng thái ván tại điểm dừng.

## Cập nhật 08/10/2026 (đợt hai): trận A.L.I.C.E, hướng dẫn, Back, màn kết

- **Trận "Gọi ai?"** hiện thanh "Lung lay" và ba chốt (`#barC`), mỗi nút có gợi ý khi trỏ chuột (`GameEngine.CallHints`). Người viết yêu cầu, khác `HUONG_DAN_DEV_CHUONG_5.md` mục 15.
- **Bảng hướng dẫn cách chơi** (`game.js`, khối `GUIDES`): đối chất, chọn ghi chú, gắn lời kể, "Gọi ai?". Mỗi bảng tự hiện một lần cho mỗi trình duyệt (`Saves.guideSeen`), mở lại bằng nút "? Cách chơi". Máy chủ gửi `how` = `note` / `link` ở điểm dừng `pick` để phân biệt hai màn của Chương 3.
- **Nút Back** lùi về câu thoại trước trong cùng một đoạn, không lùi qua điểm chọn. Khi lùi hoặc tải bản lưu, `loadBatch` không diễn lại từng bước mà tính trạng thái sân khấu, nền, nhạc, thanh điểm tại câu ấy rồi dựng một lần.
- **Skip** giữ nguyên qua điểm chọn; chỉ tắt ở màn kết cục và khi bấm Back.
- **Màn kết game** (sau Kết cục 7/7) để sáng, chữ dồn lên phần trời (`.ending.sky`). Màn bài học trước đó vẫn nền tối.
- **Menu:** chỉ hiện những nhân vật đã gặp (`Saves.met`); A.L.I.C.E đứng giữa, phía sau. Mục "Kết cục" gộp các kết cục đã xem qua mọi lượt chơi.
- **Bóng người phụ:** bảng `GameEngine.Shadows` nối tên người nói với file `Bong_…`. Thêm người phụ mới thì thêm một dòng ở đó và thả file vào `wwwroot/assets/sprites/`.
- **File tĩnh** trả `Cache-Control: no-cache` (`Program.cs`) và đường dẫn sprite có dấu phiên bản (`Manifest.v`), để file thay mới cùng tên hiện ra ngay.
- **Trang `/Check`** chỉ mở ở chế độ Development; ở Production mọi địa chỉ `/Check/...` trả 404 và menu không in liên kết.
