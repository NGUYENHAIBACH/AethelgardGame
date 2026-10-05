# Aethelgard: game web ASP.NET Core MVC (C#, Visual Studio 2022)

Game visual novel dựa trên Giáo trình Triết học Mác – Lênin (2021). Chạy được hết Mở đầu, Chương 1 và Chương 2 theo kịch bản trong `ban_giao_dev/`.

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
