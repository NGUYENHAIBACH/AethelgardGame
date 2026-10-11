# Bản tĩnh của game (thử nghiệm)

Bản này không cần máy chủ .NET. Bộ máy kịch bản (C#) được biên dịch sang WebAssembly và chạy ngay trong trình duyệt,
nên cả game chỉ còn là file tĩnh: đưa lên Vercel, GitHub Pages, Netlify, itch.io đều được, không có máy chủ nào để "ngủ"
hay quá tải.

Thư mục này **không chứa bản sao** của game. Nó trỏ thẳng vào `../Game`:

- bộ máy: `../Game/Models/**/*.cs` (trừ `StoryLibrary.cs`, thay bằng bản đọc kịch bản nhúng sẵn ở đây);
- kịch bản: `../Game/Story/*.md`, nhúng vào lúc biên dịch;
- trang, js, css, hình, nhạc: `../Game/Views` và `../Game/wwwroot`.

Sửa game thì sửa ở `../Game` rồi dựng lại; không sửa tay trong `dist/`.

## Dựng

```
python build.py
```

Cần .NET SDK và Python (có thư viện Pillow để nén hình). Kết quả nằm ở `dist/`, khoảng 90 MB: nhạc 57 MB, hình 9 MB,
tiếng động 6 MB, phần .NET 15 MB kể cả bản nén sẵn (trình duyệt thực tải khoảng 3 MB cho phần này).

Hình được nén sang WebP ngay trong lúc dựng (77 MB xuống 9 MB); file PNG gốc ở `../Game` không bị đụng tới. Nền nén ở
chất lượng 82, sprite và hình cuối ở 88, kênh trong suốt giữ nguyên vẹn. Muốn dựng với PNG gốc: `python build.py --png`.

`build.py` tự chạy tạm bản máy chủ để lấy HTML của hai trang, rồi đổi đường dẫn và
đổi chỗ `game.js` gọi máy chủ thành gọi WebAssembly. Game đổi tới mức script không nhận ra chỗ cần thay thì nó dừng và
báo, không dựng sai trong im lặng.

## Chạy thử trên máy

```
cd dist
python -m http.server 5320
```

rồi mở `http://127.0.0.1:5320/`. Mở thẳng `index.html` bằng cách bấm đúp thì **không chạy**: trình duyệt không cho
trang `file://` tải WebAssembly. Vì thế bản này là để đưa lên web, không phải bản "tải về bấm là chơi".

## Đưa lên web

Đưa nguyên thư mục `dist/` lên dịch vụ lưu trang tĩnh. Hai điều cần giữ:

- Không để công cụ nào đổi nội dung file trong `_framework/` (đổi kiểu xuống dòng, nén lại, thu gọn): .NET kiểm tra mã
  băm từng file khi tải. `dist/.gitattributes` đã chặn git làm việc này.
- Trang vào là `index.html`, màn chơi là `play.html`; mọi đường dẫn đều là tương đối nên đặt trong thư mục con cũng chạy.

## Khác gì bản máy chủ

- Lần đầu vào màn chơi phải tải phần .NET (vài MB) rồi biên dịch sáu file kịch bản, mất một hai giây; sau đó mỗi lần
  bấm lựa chọn tốn khoảng 16 ms trên máy thử.
- Không có trang `/Check`. Kiểm kịch bản vẫn làm ở bản máy chủ.
- Kịch bản được nhúng lúc dựng: sửa file `.md` thì phải dựng lại.
- Bản lưu nằm trong trình duyệt theo từng địa chỉ trang, nên bản lưu ở bản máy chủ không tự sang đây.

## Đã kiểm (08/10/2026)

Chơi 21 ván ngẫu nhiên trong trình duyệt bằng bản này (1.510 lần gọi bộ máy, không lỗi), rồi chơi lại 10 ván trong số đó
với đúng chuỗi lựa chọn ấy trên bản máy chủ: mọi dòng thoại và bước sân khấu trùng nhau từng ký tự.
