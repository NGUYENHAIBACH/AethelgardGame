# Bản tải về của game

Hai gói để người chơi tải về, giải nén và chơi, không cần cài .NET hay tự chạy máy chủ. Cả hai dùng chung bản tĩnh
do `../GameStatic/build.py` dựng (bộ máy game chạy bằng WebAssembly, hình đã nén WebP).

```
python build_desktop.py            # dựng lại bản tĩnh rồi dựng hai gói
python build_desktop.py --giu-dist # dùng lại ../GameStatic/dist đang có
```

Kết quả ở `release/`:

| Gói | Người chơi làm gì | Game hiện ở đâu |
|---|---|---|
| `Aethelgard-Windows.zip` (139 MB) | Giải nén, bấm đúp `Aethelgard.exe` | Một cửa sổ riêng (WebView2). F11: toàn màn hình |
| `Aethelgard-Mac.zip` (89 MB) | Giải nén, chuột phải vào `Choi Aethelgard.command` > Open | Trình duyệt mặc định |

## Cách chạy bên trong

- `Shared/StaticServer.cs`: máy chủ file tĩnh nhỏ, chỉ nghe ở `127.0.0.1` cổng 47350 (bận thì thử cổng kế). Cổng cố định
  để bản lưu trong trình duyệt không mất giữa các lần mở. Người chơi không phải làm gì với nó.
- `Windows/`: cửa sổ WinForms chứa WebView2 (có sẵn trên Windows 11 và hầu hết Windows 10). Máy không có WebView2 thì
  chương trình tự mở game bằng trình duyệt mặc định. Bản lưu nằm ở `%LOCALAPPDATA%\Aethelgard`.
- `Mac/`: chương trình dòng lệnh chạy máy chủ ấy rồi gọi `open` để mở trình duyệt. Gói có cả bản cho chip Apple
  (arm64) và chip Intel (x64); file `.command` tự chọn.

## Giới hạn đã biết

- Chưa file nào có chữ ký số. Windows hiện bảng SmartScreen ở lần chạy đầu ("More info" > "Run anyway"). Trên Mac phải
  mở bằng chuột phải > Open; file `.command` tự gỡ dấu cách ly và ký tạm cho chương trình.
- **Bản Mac được dựng chéo từ Windows và chưa chạy thử trên máy Mac thật.**
- Bản Windows đã thử trên máy dựng (08–09/10/2026): mở cửa sổ, vào màn chơi, đi qua ba lựa chọn, hình và nhạc tải được,
  tua nhạc được, bản lưu tự động ghi được.
