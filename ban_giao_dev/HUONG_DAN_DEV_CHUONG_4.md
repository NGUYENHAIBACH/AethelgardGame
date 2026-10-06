# Hướng dẫn cho dev: Chương 4

**Bản thảo 07/10/2026, chờ người viết kịch bản duyệt.** Các điểm ở mục 13 là đề nghị, chưa chốt.

Tài liệu này nối tiếp `HUONG_DAN_DEV.md` (Mở đầu, Chương 1), `HUONG_DAN_DEV_CHUONG_2.md`, `HUONG_DAN_DEV_CHUONG_3.md` và `HUONG_DAN_DEV_SAN_KHAU.md` (luật sprite và nhạc). Mọi quy ước ở các tài liệu đó vẫn giữ nguyên. Ở đây chỉ ghi cái mới và cái khác. Khi tài liệu này và kịch bản lệch nhau thì kịch bản đúng.

## 1. Gói bàn giao

| File | Nội dung |
|---|---|
| `04_chuong_4.md` | Chương 4: "Kíp nổ". 4 cảnh, Lựa chọn 7 đến 9, 2 kết cục kèm bài học, Mảnh 22 đến 25 |
| `Kich_ban_Chuong_4.docx` | File trên ở dạng đọc và in. Chữ giống hệt bản `.md` |

Lời thoại đã chốt. Dev không sửa chữ, không đổi thứ tự câu, không gộp hay tách lượt thoại. Thấy chỗ nào vô lý hoặc không làm được thì báo lại người viết.

Chương này không sửa gì ở Mở đầu và Chương 1 đến 3.

**Chương 4 không có trận đối chất, không có thanh đo, không có màn chọn ghi chú.** Chỉ có ba lựa chọn thường. Cái khó của chương là số nhánh mang sang: gần như đoạn nào cũng rẽ theo hai ba biến cũ (mục 2, 3).

**Cả chương diễn ra ở Tầng Đáy**, trên một nền duy nhất (BG05), trừ hai màn kết cục.

## 2. Ký hiệu mới trong kịch bản Chương 4

| Trong kịch bản | Xử lý |
|---|---|
| `- Nếu bản trên.`, `- Nếu bản dưới.` | **Không hiện.** Biến `ban` của Chương 3 |
| `- Nếu Vane là đồng minh`, `Vane đứng ngoài`, `Kael bị Vane thuyết phục` (hoặc `bị Vane thuyết phục`), `Vane không phải đồng minh` | `vane` = dong_minh / dung_ngoai / chu / khác dong_minh. **Lưu ý `vane` đổi được trong chương này** (mục 3) |
| `- Nếu lối D1.` | `loi_vao` = D1 (Chương 3) |
| `- Nếu cầm Lệnh điều chuyển và đã chọn 5-B` | `giay_to` = dieu_chuyen và `lua_chon_5` = B |
| `- Nếu Rian đã nghe chuyện mười hai người`, `Rian chưa nghe` | `rian_da_nghe` = co / khong (Chương 2) |
| `- Nếu 3-A.`, `3-B.`, `3-C.` | `lua_chon_3` |
| `- Nếu 7-A`, `7-B`, `7-C`, `không phải 7-A`, `không phải 7-B` | `lua_chon_7` |
| `- Nếu 8-A`, `8-B` | `lua_chon_8` |
| `- Nếu so_thung = 0`, `= 5`, `từ 2 tới 4`, `từ 2 trở lên` | Biến `so_thung` (mục 3). Người bốc hàng đọc đúng con số ấy |
| `- Nếu veritas = dong_hanh` / `do_du` | Biến `veritas` của Chương 3 |
| `- Nếu vane_nghe = co`, `rian_tin = co`, `biet_trung = co`, `rian_len = khong` | Biến mới (mục 3) |
| `- Nếu kết với bà Helena là Thuyết phục` / `Bất phân` / `Bị thuyết phục` / `khác Thuyết phục` | `ket_doi_chat_helena` của Chương 1. **Dùng giá trị gốc**, không dùng biến `helena` mới (mục 3) |
| `- Nếu thư ký đã đòi Kael lên`, `thư ký chưa đòi Kael lên` | Biến suy ra `doi_len` (mục 3) |
| `- Nếu ở Cảnh 3 thư ký đã đòi thiết bị cổ tay` | Biến suy ra `doi_thiet_bi` (mục 3) |
| `- Nếu có người bước vào lồng theo Kael` | Ít nhất một trong `rian_len`, `vane_len`, `tao_len` là co |
| `- Nếu có vật phẩm "…"`, `- Nếu có ghi chú "…"` | Điều kiện theo việc đang giữ thứ ấy |
| `- Nếu không.`, `- Nếu không:` | Nhánh "còn lại" của khối điều kiện ngay phía trên nó (cùng mức thụt lề) |
| `- Nếu không, và Rian đã nghe…`, `- Nếu không, và 8-B:`, `- Nếu không có điều nào ở trên:` (Cảnh 4, nhánh 9-C) | Một chuỗi xét **theo thứ tự từ trên xuống**, gặp điều kiện đúng đầu tiên thì dừng (mục 8) |
| `- Ở cả hai trường hợp trên:` (Cảnh 4, nhánh 9-C) | Chạy nếu một trong hai dòng điều kiện ngay phía trên nó đã chạy |
| `**Nếu …**`, `**Chung**`, `**Nhập lại (…)**`, `**Nhánh 7-A: …**` | **Không hiện.** Tiêu đề khối điều kiện; khối kéo dài tới tiêu đề in đậm kế tiếp hoặc tới `###`, `##` |
| `→ lo = co.`, `→ vane = dong_minh.`, `→ rian_len = co.`… | **Không hiện.** Đặt biến đúng lúc chạy tới dòng ấy |
| `→ Vật phẩm "…" rời khỏi túi đồ.` | **Không hiện.** Gỡ vật phẩm khỏi sổ tay |
| Dòng trong ngoặc tròn bắt đầu bằng ngoặc kép, ví dụ `("Tầng Trung, sáng nay: chín phần.")` | Hiện như lời dẫn. Đây là chữ viết phấn hoặc viết tay Kael đang đọc; nếu làm được thì cho một kiểu chữ khác (mục 13, điểm 4) |
| `(Ghi chú cho người làm game: …)` đầu và cuối mỗi cảnh | **Không hiện.** Đọc kỹ: tài liệu này tóm lại chứ không thay nó |

**Ô tên.** Có bóng: Chị thợ tảo (mới, `Bong_Chi_tho_tao`), Thợ bể tảo (`Bong_Tho_be_tao`). Chưa có bóng, chỉ có ô tên: Người bốc hàng, **Thư ký** (mới, từ Cảnh 3). `A.L.I.C.E (loa)` và `Dẫn chuyện` như cũ. Hai người lính gác, người xưởng 2 và xưởng 5, thợ già không nói câu nào.

**Veritas.** Từ chương này cô hiện hình và nói trước mặt người khác. Ngoại lệ duy nhất là đầu Cảnh 1: Kael úp tay che cô, hai dòng `Veritas (rất khẽ)` đầu tiên chỉ có tiếng; từ thẻ `[SPRITE: Veritas_Hologram]` đầu tiên trở đi, mọi dòng của cô (kể cả `Veritas (rất khẽ)`) đều có hình. Ở Cảnh 2, 3, 4 hình chiếu bật từ dòng đầu cảnh.

## 3. Biến

### Biến cũ mà chương này đổi giá trị

| Biến | Đổi ở đâu |
|---|---|
| `vane` | Cảnh 2, đoạn "Tờ của ông Vane": nếu `vane` = dung_ngoai **và** `vane_nghe` = co thì `vane` thành dong_minh. Từ dòng ấy trở đi mọi điều kiện "Vane là đồng minh" tính theo giá trị mới |
| `lo` | Cảnh 1: thành co nếu `ban` = tren và `vane` = dung_ngoai. Cảnh 2: nếu `vane` vừa đổi thành dong_minh như trên thì tờ báo cáo tối nay không đi lên, nên đặt lại `lo`: co nếu `loi_vao` = T2, còn lại khong (kể cả `loi_vao` = D2, vốn mang `lo` = co từ Chương 3). Cảnh 3: thành co ở mọi nhánh mà `ket_doi_chat_helena` khác thuyet_phuc |
| Túi đồ | Vật phẩm "Giấy của Ban Cố vấn cho chuyến thang chiều" rời túi ở nhánh 8-A (chỉ `loi_vao` = T1 mới có) |

**`vane` = chu luôn đi với `ban` = duoi.**

### Biến mới

| Biến | Giá trị | Đặt ở đâu |
|---|---|---|
| `lua_chon_7` | A / B / C | Cảnh 1 |
| `so_thung` | 0, 2, 3, 4, 5 | Cuối Cảnh 1, theo bảng dưới |
| `tao_nghe` | co / khong | co chỉ ở 7-A |
| `rian_tin` | co / khong | co chỉ ở 7-B |
| `vane_nghe` | co / khong | co chỉ ở 7-C **và** `vane` = dung_ngoai |
| `lua_chon_8` | A / B | Cảnh 2 |
| `lo_truoc_c3` | co / khong | Chép giá trị của `lo` ngay khi vào Cảnh 3, trước khi cảnh ấy đổi nó. Kịch bản không gọi tên biến này; cần nó để tính hai biến suy ra bên dưới |
| `biet_trung` | co / khong | Cảnh 3: co nếu 8-B, hoặc 8-A với `ket_doi_chat_helena` = thuyet_phuc hay bat_phan |
| `biet_hom` | co / khong | Cảnh 3: co nếu 8-B, hoặc 8-A với `ket_doi_chat_helena` = thuyet_phuc |
| `helena` | dong_minh / dung_ngoai / chu | Biến mới, để Chương 5 dùng. Khởi tạo từ `ket_doi_chat_helena`: thuyet_phuc → dong_minh; bat_phan → dung_ngoai; bi_thuyet_phuc → chu. Cảnh 3: nếu 8-A và bat_phan thì thành dong_minh (dòng `→ … Bà Helena từ đứng ngoài thành đồng minh`). Chương 4 không có điều kiện nào đọc biến này |
| `doi_len` | co / khong | Suy ra, cố định từ cuối Cảnh 3: co nếu `ket_doi_chat_helena` khác thuyet_phuc **và** (`lo_truoc_c3` = co **hoặc** `lua_chon_8` = B) |
| `doi_thiet_bi` | co / khong | Suy ra: co nếu `ket_doi_chat_helena` khác thuyet_phuc **và** `lo_truoc_c3` = co |
| `lua_chon_9` | A / B / C | Cảnh 4 |
| `rian_len`, `vane_len`, `tao_len` | co / khong | Cảnh 4, nhánh 9-C. Mặc định khong |

| `lua_chon_7` | `so_thung` nếu `vane` khác chu | `so_thung` nếu `vane` = chu |
|---|---|---|
| A | 5 | 5 |
| B | 0 | 2 |
| C | 3 | 4 |

Khuynh hướng và lực lượng cộng theo dòng `→` sau mỗi nút, như các chương trước: 7-A `con_so` +1, 7-B `ngon_lua` +1, 7-C `tang_day` +1; 8-A `long_tin` +1, 8-B `kiem_chung` +1; 9-A `con_so` +1, 9-B `ngon_lua` +1, 9-C `kiem_chung` +1.

## 4. Luồng Chương 4

| Bước | Nội dung | Tài nguyên | Sổ tay và biến |
|---|---|---|---|
| **Cảnh 1: Chỗ phát thuốc** | Chiều Ngày 4, dãy bể tảo. Mở theo `ban`; đoạn "Ông Vane" là ba khối theo `vane`; rồi "Chờ", "Thuốc tan" | BG05, không nhạc. Rian, Vane, mẹ, Veritas_Hologram; biểu cảm mới: Rian_Angry, Rian_Serious, Vane_Silent, Veritas_Serious, Kael_Trando, Me_LookAway | Mảnh 22 (sau câu "Tôi đợi cho đủ một ngày"). `lo` có thể thành co |
| **Lựa chọn 7** | Ba nút, ai cũng thấy đủ ba | BGM07 chỉ ở 7-B và 7-C, từ lúc người thợ bể tảo hát tới hết cảnh. 7-A im tới hết cảnh | `lua_chon_7`, `so_thung`, `tao_nghe`, `rian_tin`, `vane_nghe` |
| **Cảnh 2: Chuyến thang tối** | Tối Ngày 4, chân thang. Người bốc hàng đếm thùng theo `so_thung`; đoạn "Tờ của ông Vane" theo `vane` | BG05, không nhạc, SE06 lúc lồng xuống, SE05 lúc lồng lên | `vane` và `lo` có thể đổi (mục 3) |
| **Lựa chọn 8** | Hai nút | | `lua_chon_8`. T1 chọn 8-A: mất vật phẩm "Giấy của Ban Cố vấn…". Mảnh 23 (khi lồng lên) |
| **Cảnh 3: Ô hàng chở xuống** | Sáng Ngày 5, chân thang. Không có lựa chọn. "Thông báo" chung; "Bảy phần ấy" là bốn khối (mục 6); "Chuyến trưa" là ba khối rồi phần chung | BG05, không nhạc, SE06. Ô tên mới "Thư ký" | `lo_truoc_c3`, `biet_trung`, `biet_hom`, `helena`, `lo`, `doi_len`, `doi_thiet_bi`. Mảnh 24 (sau câu hẹn chuyến trưa) |
| **Cảnh 4: Cái tời** | Nối liền Cảnh 3. Ba đoạn rồi Lựa chọn 9 | BG05, không nhạc tới lựa chọn | |
| **Lựa chọn 9** | Ba nút, ai cũng thấy đủ ba (mục 7) | | `lua_chon_9` |
| Nhánh 9-A, 9-B | Kết cục (mục 9) | | |
| Nhánh 9-C | Kael vào lồng; từng người quyết định theo hay ở lại (mục 8) | BGM07 từ lúc mẹ tới cửa lồng, SE05 lúc lồng lên | `rian_len`, `vane_len`, `tao_len`. Mảnh 25 (khi lồng lên) |
| **Kết Chương 4** | Cái lồng đi lên. Không có câu dẫn chuyện nào sau đó | | Sang Chương 5 |

## 5. Lựa chọn 7 và Lựa chọn 8

- Cả hai xáo thứ tự nút, không hiện nhãn A, B, C, như các lựa chọn trước.
- **Lựa chọn 7:** ba nút luôn đủ. Sau mỗi nhánh là khối `**Nhập lại (cả ba nhánh)**`.
- **Lựa chọn 8:** hai nút luôn đủ. Không nút nào là kết cục.
- Trong nhánh 8-B, bốn dòng phấn Kael viết là bốn dòng trong ngoặc tròn có ngoặc kép. Ở `loi_vao` = D1 có thêm dòng chép chữ của ông Vane.
- Chỗ rẽ dễ sót ở Cảnh 2, đoạn "Tờ của ông Vane", khối "Vane đứng ngoài": bên trong còn rẽ theo `vane_nghe`. Nhánh `vane_nghe` = co là chỗ `vane` đổi thành dong_minh.

## 6. Cảnh 3: hai chỗ rẽ lớn

**Đoạn "Bảy phần ấy"** chạy đúng một trong bốn khối:

| Khối | Điều kiện | Kael được gì |
|---|---|---|
| `**Nếu 8-B**` | `lua_chon_8` = B | Đọc hai dòng phấn Tầng Trung viết trả. `biet_trung`, `biet_hom` = co |
| `**Nếu 8-A và kết … Thuyết phục**` | 8-A, thuyet_phuc | Tờ giấy của bà Helena (hai dòng trong ngoặc kép). `biet_trung`, `biet_hom` = co |
| `**Nếu 8-A và kết … Bất phân**` | 8-A, bat_phan | Tờ giấy một dòng. `biet_trung` = co; `helena` = dong_minh |
| `**Nếu 8-A và kết … Bị thuyết phục**` | 8-A, bi_thuyet_phuc | Không có tờ nào |

Ở đầu cảnh cũng có một chỗ rẽ 8-A / 8-B (thứ nằm trong cái ô sơn trắng khi lồng chạm đất).

**Đoạn "Chuyến trưa"** chạy đúng một trong ba khối, rồi tới `**Chung**`:

| Khối | Điều kiện | Việc |
|---|---|---|
| `**Nếu kết … Thuyết phục**` | thuyet_phuc | Thư ký không đòi gì. `lo` giữ nguyên |
| `**Nếu kết … khác Thuyết phục và lo = co**` | khác thuyet_phuc, `lo_truoc_c3` = co | Thư ký đòi Kael và thiết bị. Bên trong rẽ theo `vane`: đồng minh thì đội đồ đen chắn; không thì mẹ bước ra (có thẻ `[SPRITE: Vane tắt]` rồi `[SPRITE: Me_Neutral]`) |
| `**Nếu kết … khác Thuyết phục và lo = khong**` | khác thuyet_phuc, `lo_truoc_c3` = khong | Thư ký ghi một dòng; 8-B thì gọi người ký 2231 lên. `lo` = co |

Hai khối sau xét theo `lo_truoc_c3`, không theo `lo` đã bị đổi trong cảnh.

## 7. Lựa chọn 9

- Ba nút có ở **mọi** nhánh. Xáo thứ tự, không hiện nhãn.
- 9-A và 9-B là kết cục. 9-C sang Chương 5.
- Không có nút nào bị ẩn, không có nút nào cần điều kiện.
- Chữ trên ba nút giống nhau ở mọi nhánh.

Ngay trước lựa chọn (đoạn "Sắp tới giờ") có hai chỗ rẽ theo `doi_len`.

## 8. Nhánh 9-C: ai vào lồng

Các khối chạy theo đúng thứ tự trong kịch bản. Tóm tắt:

| Thứ tự | Khối | Điều kiện | Biến |
|---|---|---|---|
| 1 | Thư ký và Kael | `doi_len` | |
| 2 | Câu thêm của Kael | 8-A và bi_thuyet_phuc | |
| 3 | Mẹ trả lời thư ký | `vane` = chu thì ba câu (mẹ, Vane, mẹ); không thì một câu | |
| 4 | Người đang đói | 7-B: chị thợ tảo hỏi, mẹ đáp. Khác 7-B: một dòng tả | |
| 5 | Rian và mấy cái ống | `rian_da_nghe` | |
| 6 | **Rian lên hay ở lại** | Xét lần lượt, gặp điều đúng đầu tiên thì dừng: (a) `rian_tin` = co; (b) `rian_da_nghe` = co; (c) `lua_chon_8` = B; (d) không điều nào | (a), (b), (c): `rian_len` = co. (d): khong |
| 7 | Vane | `vane` = dong_minh: lên. dung_ngoai, chu: ở lại | dong_minh: `vane_len` = co |
| 8 | Dãy tảo | 7-B hoặc 7-C: người thợ bể tảo lên. 7-A: ở lại | 7-B, 7-C: `tao_len` = co |
| 9 | Chuyện giấy | `doi_len` = khong: luôn chạy. `doi_len` = co: chỉ chạy khi có người vào lồng theo Kael. Chạy thì có thêm hai dòng "Ở cả hai trường hợp trên" | |
| 10 | Mẹ ở cửa lồng | Mọi nhánh. `[SPRITE: Vane tắt]` trước, `[BGM: BGM07…]` cùng dòng mẹ tới | |
| 11 | Veritas và Kael | Hai câu chung, rồi rẽ theo `doi_thiet_bi` | |
| 12 | Người bốc hàng | Có người theo Kael hay không | |
| 13 | Gạt cần | `rian_len` = khong thì dòng tả khác | Mảnh 25 |

Bảng kiểm thử nhanh:

| Người chơi | Trong lồng ngoài Kael, thư ký, hai lính |
|---|---|
| 7-B (nên `rian_tin`), `vane` = dong_minh | Rian và người hai xưởng, Vane và hai người đồ đen, người thợ bể tảo |
| 7-A, 8-A, `rian_da_nghe` = khong, `vane` khác dong_minh | Không ai. Rian ở lại với cây búa cạnh cái cần |
| 7-C, 8-A, `rian_da_nghe` = khong, `vane` = dung_ngoai ban đầu (nên `vane_nghe` = co và `vane` đã thành dong_minh) | Vane và hai người đồ đen, người thợ bể tảo. Rian ở lại |
| 7-A, 8-B, `rian_da_nghe` = khong | Rian (lý do 8-B) |

## 9. Hai kết cục

| Kết cục | Đến từ | Nền, nhạc | Đoạn rẽ |
|---|---|---|---|
| 5/7 "Lồng kính" | 9-A | BG10, BGM05_Sad_Piano_2 | Dòng dẫn chuyện đầu theo `vane` = chu hay không. Đoạn thiết bị theo `ket_doi_chat_helena`: thuyet_phuc một dòng; khác thì hai hoặc ba dòng (thêm một dòng nếu `vane` = dong_minh) |
| 4/7 "Tro tàn" | 9-B | BG09, BGM06_Apocalyptic_Drums; `[BGM: tắt]` trước dòng tả Rian nhìn lên hàng đèn | Trước khi đổi nền: một dòng theo `rian_tin`; một khối theo `vane` = chu hay không |

- Mỗi kết cục: màn hình "KẾT CỤC n/7", khối Bài học kết cục (chép nguyên văn khối `>`), nút quay lại.
- **Nút quay lại của cả hai kết cục đưa về Lựa chọn 9.** Khi quay lại, bỏ điểm khuynh hướng nút vừa bấm đã cộng.
- **Kết cục 5/7 sẽ có thêm một lối vào ở Chương 5** (như Kết cục 3/7 có hai lối). Đánh dấu "đã xem" theo số kết cục, không theo lối.
- "Tro tàn": câu "Nó không lên lại." là dòng cuối, im lặng, rồi mới hiện màn hình kết cục. BG09 lần đầu dùng.
- Sau thẻ `[BG]` của kết cục, hình chiếu Veritas tắt và mọi sprite gỡ hết; trong "Tro tàn" có một thẻ `[SPRITE: Rian_Serious]` bật lại Rian.

## 10. Vật phẩm và ghi chú của Chương 4

**Chương 4 không thêm vật phẩm hay ghi chú nào vào sổ tay.** Thứ Kael biết thêm nằm trong biến (`biet_trung`, `biet_hom`).

| Mục | Việc |
|---|---|
| Vật phẩm "Giấy của Ban Cố vấn cho chuyến thang chiều" (T1) | Rời túi ở nhánh 8-A. Ở 8-B vẫn còn; Cảnh 3 có một dòng của Veritas chỉ hiện khi còn giữ nó |
| Ghi chú "Người đã tiêm" (5-C) | Mở một trao đổi ở Cảnh 1, đoạn "Thuốc tan" |
| Ghi chú "Bà Helena đã chép lời Veritas kể vào sổ riêng của bà" (T1, dong_hanh) | Mở một dòng ở Cảnh 2, nhánh 8-A |
| Vật phẩm "Thẻ đặc phái", "Lệnh điều chuyển", "Giấy thông hành" | Vẫn nằm trong sổ tay, không còn tác dụng |

## 11. Mảnh lưu trữ

| Mảnh | Tên | Ai nhận | Mở lúc nào |
|---|---|---|---|
| 22 | "Que diêm trong hộp" | Mọi người | Cảnh 1, sau câu "Tôi đợi cho đủ một ngày" |
| 23 | "Thêm một độ nữa" | Mọi người | Cảnh 2, khi cái lồng đi lên thiếu thùng |
| 24 | "Cần nhau mà giằng nhau" | Mọi người | Cảnh 3, sau câu hẹn chuyến trưa của thư ký |
| 25 | "Giữ cái móng" | Ai đi nhánh 9-C | Cảnh 4, khi cái lồng đi lên |

Người chơi dừng ở kết cục 9-A hoặc 9-B không có Mảnh 25 (quay lại chọn 9-C thì có). Không mảnh nào có số liệu động. Nội dung chép nguyên văn từ các khối `>` ở cuối file. Mảnh 25 nhắc tới "phủ định sạch trơn" nhưng **không** hiện bộ đếm lập trường của Chương 1 (mục 13, điểm 3).

## 12. Tài nguyên dùng trong Chương 4

| Loại | Tên file | Ghi chú |
|---|---|---|
| Nền | BG05_Khu_O_Chuot_Tang_Day, BG09_Thanh_Pho_Tro_Tan, BG10_Tang_Day_No_Du_Nhung_U_Toi | BG05 dùng cho cả dãy bể tảo lẫn chân thang. BG09 lần đầu dùng |
| Nhạc | BGM05_Sad_Piano_2, BGM06_Apocalyptic_Drums, BGM07_Bittersweet_Ambient | BGM06 lần đầu dùng. Phần lớn chương không có nhạc: đừng tự thêm nhạc nền |
| Hiệu ứng | SE05_Xa_Van_Khi_Kim_Loai (tời chạy, búa đập tời), SE06_Cua_Thep_Mo_Ra (lồng chạm đất) | |
| Sprite | Kael_Neutral, Kael_Trando, Rian_Neutral, **Rian_Serious, Rian_Angry**, Vane_Neutral, **Vane_Silent**, **Me_Neutral, Me_LookAway**, Veritas_Hologram, **Veritas_Serious** | Các file in đậm nằm trong `Sprites_moi/` (đã giao 06/10/2026) |
| Bóng | Bong_Chi_tho_tao, Bong_Tho_be_tao | Trong `Sprites_moi/` |

- Cảnh 1, đầu đoạn "Thuốc tan": đèn dãy bể hạ xuống mức đêm. Có thể chỉnh tối nền; không bắt buộc.
- Mỗi cảnh mở bằng một thẻ `[BG]` (dù vẫn là BG05), nên sân khấu được gỡ sạch ở đầu cảnh và hình chiếu Veritas phải bật lại bằng thẻ: kịch bản đã ghi đủ.
- Sân khấu của từng cảnh, kể cả các chỗ game phải tự xoay vòng, ghi trong `(Ghi chú cho người làm game: …)` cuối mỗi cảnh. Hai chỗ đã biết và để nguyên ở Cảnh 1 cũng ghi ở đó.

## 13. Các điểm kịch bản không nói rõ

Dưới đây là **đề nghị**, chờ người viết chốt. Các mục phía trên viết theo đúng các đề nghị này.

| # | Điểm | Đề nghị |
|---|---|---|
| 1 | Nút quay lại ở hai kết cục quay về đâu | Về Lựa chọn 9, như kịch bản ghi |
| 2 | Kết cục 5/7 có hai lối vào (ở đây và ở Chương 5): đếm là mấy | Một, như Kết cục 3/7 |
| 3 | Ghi chú ở Chương 1 nói đáp án "phủ định sạch trơn" của trận Helena "để dành cho mảnh của Chương 4" | Mảnh 25 không có bộ đếm; các bộ đếm của Mảnh 06 giữ như cũ |
| 4 | Chữ phấn trên sàn lồng và chữ viết tay của bà Helena hiển thị ra sao | Như lời dẫn; nếu làm được thì dùng kiểu chữ viết tay hoặc đặt trong một khung riêng, tùy dev |
| 5 | Biến `helena` | Thêm mới như mục 3, để Chương 5 dùng; không sửa `ket_doi_chat_helena` |
| 6 | `doi_len`, `doi_thiet_bi` tính theo `lo` lúc nào | Theo `lo_truoc_c3` (giá trị khi vào Cảnh 3) |
| 7 | Ô tên "Thư ký" và "Người bốc hàng" chưa có bóng | Chỉ hiện ô tên, sân khấu giữ nguyên |
| 8 | Quay lại từ kết cục rồi chọn nút khác: điểm khuynh hướng | Bỏ điểm của nút cũ, cộng điểm của nút mới |

Gặp điểm nào khác mà kịch bản và tài liệu này đều không nói: hỏi người viết, đừng tự quyết.

## 14. Dữ liệu phải mang sang Chương 5

- Mọi biến đã có, cộng: `lua_chon_7`, `so_thung`, `tao_nghe`, `rian_tin`, `vane_nghe`, `lua_chon_8`, `biet_trung`, `biet_hom`, `helena`, `doi_len`, `doi_thiet_bi`, `lua_chon_9`, `rian_len`, `vane_len`, `tao_len`; và giá trị mới của `vane`, `lo`.
- Năm khuynh hướng, hai lực lượng, toàn bộ vật phẩm, ghi chú, mảnh.
- **Mọi người chơi vào Chương 5 đều đã chọn 9-C và đang ở trong cái lồng thang đi lên.**
- Các ghi chú kiếm được ở Tầng Trung từ Chương 1 ("Lời bà cụ", "Mười hai tệp chưa xóa", "Khóa ngắt dự phòng"…) sẽ được dùng lại ở Chương 5: đừng dọn sổ tay.

## 15. Không tự ý đổi

Như các tài liệu trước, thêm:

- Không gợi ý nút nào của Lựa chọn 9 là nút "đi tiếp". Ba nút cùng cỡ, cùng kiểu, xáo thứ tự.
- Không thêm nhạc vào các đoạn kịch bản để im. Cảnh 2 và Cảnh 3 không có nhạc từ đầu tới cuối; ở Cảnh 1 nhánh 7-A im tới hết cảnh là chủ ý.
- Người đã tiêm thuốc (chị thợ tảo ở 7-A và 7-C, người dãy tảo) nói đều giọng: không thêm hiệu ứng rung, phóng chữ hay biểu cảm cho các dòng ấy.
- Không hiện con số `so_thung` ở đâu ngoài lời người bốc hàng.
- Không báo cho người chơi biết ai sẽ vào lồng trước khi họ chọn 9-C, và không hiện danh sách người trong lồng sau đó.
- Chương kết ở dòng cái lồng đi lên. Không thêm câu dẫn chuyện, không thêm chữ "Hết Chương 4" kiểu khẩu hiệu ngoài màn chuyển chương thường dùng.
- Các con số: sáu thùng một chuyến; suất chín, tám, bảy, sáu phần; Tầng Trung sáng Ngày 5 còn chín phần; mã cư dân 2231; năm 2296, chín ngày, ba trăm người; hai mươi bốn bể tảo.
