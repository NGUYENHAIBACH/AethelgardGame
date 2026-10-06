# Hướng dẫn cho dev: Chương 2

> **Cập nhật 05/10/2026:** cách hiện sprite và các chỗ tắt nhạc đã đổi (sân khấu tối đa ba người, thẻ `[SPRITE: … tắt]`, mọi chỗ tắt nhạc đều có thẻ). Đọc `HUONG_DAN_DEV_SAN_KHAU.md`; chỗ nào file này nói khác thì theo file đó (bảng đối chiếu ở mục 5 của nó).

Người viết kịch bản duyệt ngày 04/10/2026.

Tài liệu này nối tiếp `HUONG_DAN_DEV.md` (Mở đầu và Chương 1). Mọi quy ước ở tài liệu đó vẫn giữ nguyên: cách đọc kịch bản, sổ tay ba ngăn, ô tên, 11 quyết định về hiển thị. Ở đây chỉ ghi cái mới và cái khác. Khi tài liệu này và kịch bản lệch nhau thì kịch bản đúng.

## 1. Gói bàn giao

| File | Nội dung |
|---|---|
| `02_chuong_2.md` | Chương 2: "Tầng Đáy". 6 cảnh, Lựa chọn 3, 4, 5, một trận đối chất, 3 cách kết và 1 kết cục sớm, Mảnh 07 đến 16, 1 bài học kết cục |
| `Kich_ban_Chuong_2.docx` | File trên ở dạng đọc và in. Chữ giống hệt bản `.md` |
| `01_chuong_1.md` và `Kich_ban_Mo_dau_va_Chuong_1.docx` (xuất lại) | Chỉ đổi hai thẻ nhạc, xem mục 2 |

Lời thoại đã chốt. Dev không sửa chữ, không đổi thứ tự câu, không gộp hay tách lượt thoại. Thấy chỗ nào vô lý hoặc không làm được thì báo lại người viết.

## 2. Sửa ở phần đã giao (Chương 1)

Bản nhạc `BGM05_Sad_Piano` nay tách thành ba bản, mỗi bản một tâm trạng. Hai thẻ trong Chương 1 đổi tên theo:

| Chỗ | Thẻ cũ | Thẻ mới |
|---|---|---|
| Chương 1, đoạn nhập lại trước Lựa chọn 2 (mười hai tệp) | `BGM05_Sad_Piano` | `BGM05_Sad_Piano_1` |
| Chương 1, kết cục sớm "Ca làm hoàn hảo" | `BGM05_Sad_Piano` | `BGM05_Sad_Piano_2` |

**Trận đối chất với Helena đổi cách đặt chữ.** Trên nút nay chỉ còn phần nêu ý của mỗi đáp án (một hoặc hai câu); các câu dẫn chứng thành những dòng `- Kael: "…"` ngay bên dưới, Kael nói sau khi người chơi chọn. Chữ không đổi, điểm không đổi. Lý do: đáp án cho điểm dài hơn hẳn nên người chơi nhìn độ dài là đoán ra. Chi tiết ở mục 7 của `HUONG_DAN_DEV.md` (đã cập nhật). Trận Vane ở Chương 2 làm cùng cách, xem mục 7 bên dưới.

Không có thay đổi nào khác ở Mở đầu và Chương 1.

## 3. Ký hiệu mới trong kịch bản Chương 2

| Trong kịch bản | Xử lý |
|---|---|
| `[BGM: tắt]` | Tắt nhạc nền, im cho tới thẻ `[BGM]` kế tiếp |
| `[TRẢ: …]`, `[GẶT: …]` | **Không hiện.** Ghi chú của người viết, cùng loại với `[GIEO: …]` |
| `- Nếu cầm Giấy thông hành…`, `- Nếu cầm Lệnh điều chuyển…`, `- Nếu cầm Thẻ đặc phái…` | **Không hiện.** Điều kiện theo biến `giay_to` |
| `- Nếu 5-B:` rồi các dòng thụt vào bên dưới | Khối điều kiện nhiều dòng. Các dòng thụt vào chỉ chạy khi điều kiện đúng. Có chỗ lồng hai tầng (`Nếu 5-B` rồi bên trong `Nếu cầm Thẻ đặc phái`) |
| `- Nếu không.` | Nhánh còn lại của điều kiện ngay phía trên |
| `Kael: "…" / Mẹ: "…"` trên cùng một dòng điều kiện | Hai lượt thoại nối tiếp, tách ở dấu `/` |
| `[Lùi]`, `[Giữ lời]`, `[Đòn], cần ghi chú "X"` | Đáp án lượt hai của trận đối chất, xem mục 7 |
| Dòng `- Kael: "…"` ngay dưới một đáp án | Phần Kael **nói tiếp sau khi người chơi đã chọn**. Không nằm trên nút |
| `(Lập trường của [C]: …)` | **Không hiện.** Cùng loại với dòng `→ … Lập trường` |
| `Veritas (rất khẽ): "…"` | Lời thoại bình thường, ô tên "Veritas", **không hiện sprite** (cô chỉ có tiếng) |
| `Vane (to tiếng): "…"` | Lời thoại bình thường; có thể cho chữ to hơn hoặc rung khung thoại |
| Dòng "Năm câu hỏi, mỗi câu hai lượt…" dưới tiêu đề Cảnh 5, dòng "Cảnh nối liền sau trận…" dưới tiêu đề Cảnh 6, dòng "Lời của ba lựa chọn đổi theo giấy tờ…" dưới Lựa chọn 5 | **Không hiện.** Ghi chú soạn thảo |
| `**Một câu của Rian (…)**`, `**Câu cuối chương**`, `**Cái hòm (chung cho mọi nhánh)**` và các dòng in đậm tương tự | **Không hiện.** Tên đoạn và điều kiện |

**Ô tên.** Rian hiện "???" cho tới dòng `(Từ đây ô tên hiện "Rian".)`. Các vai chỉ có ô tên, không có sprite: Mẹ, Thợ già, Thợ trẻ, Chị thợ, Thợ học việc, Thợ bể tảo, Người gác thang, Người bốc hàng, Người phát suất. **(Câu này hết hiệu lực từ 06/10/2026: mẹ Kael và Doran có sprite, sáu người phụ có bóng. Xem `HUONG_DAN_DEV_SAN_KHAU.md` mục 2.1 và 2.11.)**

**Lời hát.** Hai câu hát trong Cảnh 2 và Cảnh 4 ("...lò đỏ bên thềm...", "...con ngủ đi...") là lời tạm, để dạng dễ thay.

## 4. Biến mới

| Biến | Giá trị | Đặt ở đâu |
|---|---|---|
| `giay_to` | thong_hanh / dieu_chuyen / dac_phai | Suy từ `ket_doi_chat_helena`: thuyet_phuc → thong_hanh; bat_phan → dieu_chuyen; bi_thuyet_phuc → dac_phai |
| `lua_chon_3` | A / B / C | Cảnh 1 |
| `lua_chon_4` | A / B / C | Cảnh 2 |
| `rian_da_nghe` | co / khong | Cảnh 2 (chỉ thành "co" khi 4-B **và** 1-B hoặc 1-C); có thể đổi thành "co" ở Cảnh 6 |
| `lua_chon_5` | A / B / C | Cảnh 4 |
| `han_vane` | sang_mai / ngay_bay_gio | Cảnh 4. Chỉ 5-B là ngay_bay_gio |
| `lung_lay`, `dao_dong` | Số nguyên | Đặt lại về 0 khi vào trận Vane. Nếu `lua_chon_5` = A thì `dao_dong` bắt đầu từ 1 |
| `da_dung_don_cau_4` | co / khong | Trận đối chất, Câu 4. Dùng ở Cảnh 6 |
| `vane` | dong_minh / dung_ngoai / chu / ket_cuc_som | Cuối trận đối chất |

Khuynh hướng `con_so` và `ngon_lua`, lực lượng `tang_day` bắt đầu được cộng từ chương này. Chương 2 **không có** bộ đếm lập trường (bộ đếm của Mảnh 06 giữ nguyên giá trị cuối Chương 1).

Vị trí tấm thẻ viền vàng (nhánh Thẻ đặc phái): cài trên ngực; sau 4-C thì nằm trong túi áo. Đây chỉ là chi tiết trong lời tả, không cần biến.

## 5. Luồng Chương 2

| Bước | Nội dung | Tài nguyên | Sổ tay và biến |
|---|---|---|---|
| **Cảnh 1: Thang hàng** | Quảng trường tháp, cửa thang, đi xuống, chân thang Tầng Đáy. Rẽ theo `giay_to` và `lua_chon_1` | BG03, BGM01, Helena_Neutral (chỉ Thẻ đặc phái), SE05, SE06, Veritas_Hologram, BG05 | Mảnh 07 (mọi người) |
| **Lựa chọn 3** | 3-A đi thẳng / 3-B hỏi người bốc hàng / 3-C đọc bảng suất | | 3-A: `lam_dung_lenh` +1. 3-B: `tang_day` +1, ghi chú "Lời người bốc hàng…", Mảnh 09. 3-C: `kiem_chung` +1, ghi chú "Bảng suất Tầng Đáy…", Mảnh 08 |
| **Cảnh 2: Xưởng đúc số 4** | Vào xưởng (rẽ theo `lua_chon_3`), Rian, gặp mẹ (rẽ theo `giay_to`, `lua_chon_1`) | BG07, Rian_Neutral, Kael_*, BGM05_Sad_Piano_3 từ lúc gặp mẹ | 3-A: ghi chú "Lời nghe lỏm…". Lệnh điều chuyển: vật phẩm "Thẻ công nhân xưởng đúc số 4" |
| **Lựa chọn 4** | 4-A làm việc theo tờ giấy / 4-B kể chuyện ống vỡ / 4-C ngồi xuống cạnh nồi | | 4-A: `lam_dung_lenh` +1 (Thẻ đặc phái: `long_tin` +1 thay vào đó). 4-B: `kiem_chung` +1, Mảnh 11; nếu 1-B hoặc 1-C thì thêm ghi chú "Rian đã nghe…" và `rian_da_nghe` = co. 4-C: `tang_day` +1, ghi chú "Bữa sáu phần…" |
| Nhập lại Cảnh 2 | Bài hát, lời Veritas trên lối ra lò lớn | | Mảnh 10 (mọi người) |
| **Cảnh 3: Lò lớn** | Kael và Rian. Không có lựa chọn | BG06. **Không có nhạc** | Ghi chú "Lò lớn tắt từ năm 2310…" (mọi người), Mảnh 12 (mọi người). 1-C: ghi chú "Số đo đai vá…" |
| **Cảnh 4: Vane và Soma-X** | Cái hòm, người thợ bể tảo, bà tổ trưởng từ chối, Vane giao việc theo `giay_to` | BG07, Vane_Neutral, Vane_Surprised, Vane_Aggressive, Rian_Neutral. **Không có nhạc** | Mảnh 13 (mọi người) |
| **Lựa chọn 5** | Chữ trên ba nút đổi theo `giay_to` (ba bộ, mỗi bộ ba nút) | | 5-A: `con_so` +1. 5-B: `ngon_lua` +1, `han_vane` = ngay_bay_gio. 5-C: `kiem_chung` +1, ghi chú "Người đã tiêm…" |
| **Cảnh 5: Đối chất với Vane** | Năm câu, mỗi câu hai lượt | BG07, BGM03 từ Câu 1 | Xem mục 7 và 8 |
| **Cảnh 6: Sau cái hòm** | Ba đoạn, rẽ theo `vane` và `giay_to`; hoặc kết cục sớm | BG07, BG05, BG10, BGM07, BGM05_Sad_Piano_2, SE06, Veritas_Hologram | Thắng: vật phẩm "Nhãn hòm thuốc" |

**Nhạc.** Kịch bản chỉ có một thẻ `[BGM: tắt]` (đầu Cảnh 6). Hai chỗ nhạc phải tắt mà không có thẻ, dev tự đặt:

- Đầu Cảnh 3: tắt `BGM05_Sad_Piano_3`. Cảnh 3 và Cảnh 4 im hoàn toàn.
- `BGM01` chạy từ đầu Cảnh 1 tới khi `BGM05_Sad_Piano_3` thay nó ở Cảnh 2.

**Veritas chỉ có tiếng.** Ở Cảnh 3 cô nói nhưng không hiện sprite (các dòng `Veritas (rất khẽ)`). Ở Cảnh 4 và Cảnh 5 cô im hoàn toàn. Câu "(Thiết bị cổ tay lạnh đi trên da Kael.)" và "(…thiết bị cổ tay ấm lên…)" là lời tả, không phải thông báo mục mới; nếu muốn có hiệu ứng trên biểu tượng cổ tay thì dùng màu riêng, khác màu thông báo.

## 6. Lựa chọn 5: chữ trên nút

Ba nút, nhưng chữ khác nhau theo `giay_to`. Kịch bản liệt kê đủ chín dòng. Sau khi chọn, đoạn phản ứng cũng rẽ theo `giay_to` (trừ 5-C, chung cho cả ba).

## 7. Trận đối chất với Vane: luật

Khác trận Helena ở chỗ **mỗi câu có hai lượt**.

**Lượt một.** Vane nói vài lượt, rồi hiện ba đáp án [A], [B], [C].

- Chọn [A] hoặc [B]: chạy các dòng phản ứng bên dưới, cộng điểm, **sang câu sau** (không có lượt hai).
- Chọn [C]: chạy các dòng ngay dưới [C] (Vane vặn lại), **chưa cộng điểm**, rồi hiện đáp án lượt hai.

**Lượt hai** (chỉ sau [C]). Hiện [Lùi], [Giữ lời], và [Đòn] nếu người chơi đang giữ ghi chú mà đòn yêu cầu.

- Không có ghi chú thì chỉ hiện hai nút: [Lùi] và [Giữ lời].
- Câu 3 có hai bản [Đòn] (theo hai ghi chú khác nhau, không ai có cả hai). Người chơi chỉ thấy một bản.
- Ghi chú "Bảng suất Tầng Đáy" mở đòn ở cả Câu 3 và Câu 5.

**Hiển thị.** Như trận Helena: không hiện chữ A, B, C hay chữ "Lùi", "Giữ lời", "Đòn" trên nút; xáo ngẫu nhiên thứ tự các nút ở cả hai lượt.

**Trên nút chỉ có phần nêu ý.** Chữ trên nút chỉ là phần trong ngoặc kép ngay sau nhãn đáp án (ví dụ `[C] "Con số thì đâu có chọn ai chịu."`): một hoặc hai câu, đủ để người chơi hiểu lựa chọn ấy muốn nói gì. Dẫn chứng nằm ở các dòng `- Kael:` bên dưới. Làm thế để người chơi không nhìn độ dài mà đoán được đáp án nào cho điểm. Chọn xong: hiện câu trên nút thành một lượt thoại của Kael, rồi chạy tiếp các dòng `- Kael: "…"` bên dưới như những lượt thoại thường, rồi tới phản ứng của người khác. Không gộp các dòng ấy lên nút. Vane về `Vane_Neutral` khi bắt đầu câu hỏi kế tiếp hoặc đoạn kết.

**Hai thanh.** Hiện từ dòng "(Hai thanh hiện lên…)" tới hết Câu 5. Không hiện vạch ngưỡng.

- **Lung lay** của Vane: từ 0 đến 10, ngưỡng 6. Không xuống dưới 0 (Câu 4 đáp án [B] trừ 1).
- **Dao động** của Kael: từ 0 đến 11, ngưỡng 6.

**Lối vào trận** (trước Câu 1) rẽ theo `lua_chon_5` và `giay_to`; các dòng ấy chỉ để người và vật đứng đúng chỗ.

## 8. Trận đối chất với Vane: bảng điểm

| Câu | Đáp án | Điều kiện hiện | Điểm | Sprite |
|---|---|---|---|---|
| 1 | A | | Dao động +2 | |
| 1 | B | | Dao động +1 | |
| 1 | C → Lùi | | Dao động +1 | |
| 1 | C → Giữ lời | | Không cộng gì | |
| 1 | C → Đòn | Ghi chú "Bữa sáu phần" | Lung lay +2, `tang_day` +1 | |
| 2 | A | | Dao động +2 | |
| 2 | B | | Không cộng gì | |
| 2 | C → Lùi | | Dao động +1 | |
| 2 | C → Giữ lời | | Không cộng gì | |
| 2 | C → Đòn | Ghi chú "Người đã tiêm" | Lung lay +2, `tang_day` +1 | |
| 3 | A | | Dao động +2 | |
| 3 | B | | Dao động +1 | |
| 3 | C → Lùi | | Dao động +1 | |
| 3 | C → Giữ lời | | Không cộng gì | |
| 3 | C → Đòn (bảng suất) | Ghi chú "Bảng suất Tầng Đáy" | Lung lay +2, `tang_day` +1 | Vane_Surprised |
| 3 | C → Đòn (người bốc hàng) | Ghi chú "Lời người bốc hàng" | Lung lay +2, `tang_day` +1 | |
| 4 | A | | Dao động +2 | |
| 4 | B | | Lung lay −1 | Vane_Aggressive |
| 4 | C → Lùi | | Dao động +1 | |
| 4 | C → Giữ lời | | Không cộng gì | |
| 4 | C → Đòn | Ghi chú "Mười hai tệp chưa xóa" (từ Chương 1, 2-B) | Lung lay +2; `da_dung_don_cau_4` = co | |
| 5 | A | | Dao động +2 | |
| 5 | B | | Dao động +1 | |
| 5 | C → Lùi | | Dao động +1 | |
| 5 | C → Giữ lời | | Không cộng gì | |
| 5 | C → Đòn | Ghi chú "Bảng suất Tầng Đáy" | Lung lay +2 | |

Trong đòn Câu 4 có một dòng `- Nếu 1-C.` (lời tả, chỉ chạy khi `lua_chon_1` = C).

Chỉ đòn mới cho điểm Lung lay, mỗi đòn +2. **Muốn thắng phải trúng ít nhất ba đòn trên năm câu.** Bảng kiểm thử:

| Đòn ở câu | Cần đã chọn |
|---|---|
| 1 | 4-C |
| 2 | 5-C |
| 3 | 3-B hoặc 3-C |
| 4 | 2-B (Chương 1) |
| 5 | 3-C |

- Nhiều đòn nhất: năm (2-B, 3-C, 4-C, 5-C), Lung lay 10.
- Ít lựa chọn nhất để thắng: 3-C (hai đòn) cộng một trong ba: 2-B, 4-C, 5-C.
- Người chọn 2-A, 3-A, 4-A hoặc 4-B, 5-A hoặc 5-B không có đòn nào: không bao giờ thắng.
- Hai ngưỡng không thể cùng đạt: Lung lay 6 cần ba câu đi tới đòn, hai câu còn lại cho Dao động nhiều nhất 4, cộng 1 của 5-A là 5.

## 9. Kết trận và Cảnh 6

Ngay sau Câu 5, **mở Mảnh 14, 15, 16 trước** (ở mọi cách kết, kể cả kết cục sớm), rồi xét theo đúng thứ tự này:

1. Nếu `dao_dong` ≥ 6 **và** `lua_chon_5` = A: `vane` = ket_cuc_som. Chạy đoạn "Kael bị thuyết phục" ở cuối Cảnh 5, rồi nhảy thẳng tới "Kết cục sớm: Không còn đói" ở Cảnh 6 (BG10, BGM05_Sad_Piano_2). Màn hình "KẾT CỤC 3/7", khối Bài học kết cục, nút quay lại. Game dừng ở đây. `con_so` +2.
2. Nếu `lung_lay` ≥ 6: **Thuyết phục được**. `vane` = dong_minh. `kiem_chung` +2.
3. Nếu `dao_dong` ≥ 6: **Bị thuyết phục**. `vane` = chu. `con_so` +2. Sprite Kael_Trando.
4. Còn lại: **Bất phân**. `vane` = dung_ngoai.

**Cảnh 6** chạy ba đoạn theo thứ tự:

| Đoạn | Rẽ theo | Ghi chú |
|---|---|---|
| Đoạn 1: trong xưởng | `vane`; bên trong còn rẽ theo `lua_chon_5` (5-B) và `giay_to` | Thắng: vật phẩm "Nhãn hòm thuốc" |
| "Một câu của Rian" | Chỉ chạy khi `vane` là dong_minh hoặc dung_ngoai, **và** `da_dung_don_cau_4` = co, **và** `lua_chon_4` khác B | Nếu thêm `lua_chon_1` là B hoặc C: ghi chú "Rian đã nghe…", `rian_da_nghe` = co |
| Đoạn 2: mẹ và Kael | `vane` (thắng và bất phân dùng chung một bộ, rẽ tiếp theo `giay_to`; bị thuyết phục có bộ riêng) | Dòng "Nếu bất phân" trong nhánh Giấy thông hành là hai lượt thoại |
| Đoạn 3: Kael và Veritas | Phần chung, rồi một khối riêng nếu bị thuyết phục, một khối riêng nếu thắng | BG05, BGM07, Veritas_Hologram |
| Câu cuối chương | Bốn bộ: (thong_hanh hoặc dac_phai) và thắng / (thong_hanh hoặc dac_phai) và bất phân / dieu_chuyen và không bị thuyết phục / bị thuyết phục | |

**Vật phẩm "Nhãn hòm thuốc":** hiện mô tả "Một dãy số và một cái ngày. Không có tên." Con số cụ thể sẽ có ở Chương 3; chừa chỗ để thêm.

**Nút quay lại ở kết cục 3/7:** cho chọn một trong ba điểm: Lựa chọn 3, Lựa chọn 4, Lựa chọn 5.

## 10. Vật phẩm và ghi chú của Chương 2

| Mục | Loại | Ai có | Dùng trong chương này |
|---|---|---|---|
| Thẻ công nhân xưởng đúc số 4 | Vật phẩm | Chỉ Lệnh điều chuyển | |
| Nhãn hòm thuốc | Vật phẩm | Chỉ khi thắng Vane | |
| Lời người bốc hàng: chất lên thì đầy, lĩnh về thì vơi | Ghi chú | 3-B | Đòn Câu 3 |
| Bảng suất Tầng Đáy: chín phần xuống sáu phần trong bốn tháng | Ghi chú | 3-C | Đòn Câu 3 và Câu 5 |
| Lời nghe lỏm: mai Rian định kéo sang dãy bể tảo | Ghi chú | 3-A | Không. Để dành Chương 4 |
| Rian đã nghe chuyện mười hai người | Ghi chú | 4-B cộng 1-B hoặc 1-C; hoặc ở Cảnh 6 | Không. Để dành Chương 4 |
| Bữa sáu phần: mẹ sẻ nửa bát của mình cho người khác | Ghi chú | 4-C | Đòn Câu 1 |
| Lò lớn tắt từ năm 2310; đai vá nấu lại từ đồ hỏng, mỗi mẻ một ít đi | Ghi chú | Mọi người | Không |
| Số đo đai vá: đai đời cũ 5 mm, đai mẻ mới 2 mm | Ghi chú | 1-C | Không. Để dành Chương 3 |
| Người đã tiêm: không đói, không để ý con ăn chưa, nghe hát không nối | Ghi chú | 5-C | Đòn Câu 2 |

Vật phẩm "Thẻ cũ của mẹ" (từ Mở đầu) xuất hiện trong lời tả ở Cảnh 2; không cần xử lý gì thêm, vẫn nằm trong ngăn Vật phẩm.

## 11. Mảnh lưu trữ

| Mảnh | Tên | Ai nhận | Mở lúc nào |
|---|---|---|---|
| 07 | "Ô trống trên sàn thang" | Mọi người | Cảnh 1, dòng `→ Mở Mảnh 07.` |
| 08 | "Số hôm qua" | Chỉ 3-C | Dòng `→ Mở Mảnh 08.` |
| 09 | "Ai làm, ai chia" | Chỉ 3-B | Dòng `→ Mở Mảnh 09.` |
| 10 | "Bài hát đổi lời" | Mọi người | Cảnh 2, dòng `→ Mở Mảnh 10.` |
| 11 | "Từ cái nồi tới cái ống" | Chỉ 4-B | Dòng `→ Mở Mảnh 11…` |
| 12 | "Thợ còn, khuôn còn" | Mọi người | Cảnh 3, dòng `→ Mở Mảnh 12.` |
| 13 | "Biết mà không thấy gì" | Mọi người | Cảnh 4, dòng `→ Mở Mảnh 13.` |
| 14 | "Cái bụng và cái đầu" | Mọi người | Ngay sau Câu 5, trước đoạn kết |
| 15 | "Lính của ai" | Mọi người | Như trên |
| 16 | "Lời cũ, lò nguội" | Mọi người | Như trên |

Hết Chương 2 mỗi người chơi có thêm 7 đến 9 mảnh: bảy mảnh chung (07, 10, 12, 13, 14, 15, 16), cộng 08 hoặc 09 nếu chọn 3-C hoặc 3-B, cộng 11 nếu chọn 4-B. Ba mảnh 14, 15, 16 mở cùng lúc: chỉ một thông báo "Mảnh lưu trữ mới", biểu tượng nháy một lần.

Chương 2 không có mảnh nào có số liệu động. Nội dung mảnh và bài học kết cục chép nguyên văn từ các khối `>`.

## 12. Tài nguyên dùng trong Chương 2

| Loại | Tên file | Tình trạng |
|---|---|---|
| Nền | BG03_Thap_Nang_Luong_Sup_Do, BG05_Khu_O_Chuot_Tang_Day, BG06_Duong_Ong_Ngam_Tang_Day, BG07_Dai_Ban_Doanh_Khang_Chien, BG10_Tang_Day_No_Du_Nhung_U_Toi | **Chưa có file.** Thư mục `Backgrounds` đang trống |
| Nhạc | BGM01_Dark_Ambient_Cyberpunk, BGM03_Heavy_Industrial, BGM05_Sad_Piano_2, BGM05_Sad_Piano_3, BGM07_Bittersweet_Ambient | Có, trong thư mục `BGM` |
| Hiệu ứng | SE05_Xa_Van_Khi_Kim_Loai, SE06_Cua_Thep_Mo_Ra | Có, trong thư mục `SE` |
| Sprite | Kael_Neutral, Kael_Surprised, Kael_Trando, Helena_Neutral, Rian_Neutral, Vane_Aggressive, Veritas_Hologram | Có, trong thư mục `Sprites` |
| Sprite | Vane_Neutral, Vane_Surprised | Có, nhưng nằm ở thư mục `Sprites_Phu` |

- **BG07 dùng làm xưởng đúc số 4.** Xưởng không có lửa (lò nguội, một bóng đèn vàng). Nếu ảnh có ánh lửa thì chỉnh tối đi. Riêng nhánh bị thuyết phục ở Cảnh 6 thì lò vừa được nhóm lại.
- BG06 dùng cho cả lối đi lẫn khoảng lò lớn (Cảnh 3). BG05 dùng cho chân thang Tầng Đáy (Cảnh 1) và quãng ngoài cửa xưởng (Cảnh 6). BG10 chỉ dùng cho kết cục sớm.
- Đoạn đi thang ở Cảnh 1 là màn hình đen với một ô sáng nhỏ dần.
- Chương này không dùng Rian_Angry, Vane_Smile, Vane_Smug.
- `Vane_Aggressive` chỉ dùng hai lần: 5-B của nhánh Thẻ đặc phái, và Câu 4 đáp án [B]. `Vane_Surprised` chỉ dùng hai lần: cuối Cảnh 4, và đòn "Bảng suất" ở Câu 3.

## 13. Dữ liệu phải mang sang Chương 3

- Mọi biến đã có từ Chương 1, cộng: `giay_to`, `lua_chon_3`, `lua_chon_4`, `lua_chon_5`, `rian_da_nghe`, `han_vane`, `vane`.
- Năm khuynh hướng và hai lực lượng.
- Toàn bộ vật phẩm, ghi chú, mảnh.
- **Kael đang ở tầng nào khi Chương 2 kết thúc:**

| `giay_to` | `vane` = dong_minh hoặc dung_ngoai | `vane` = chu |
|---|---|---|
| thong_hanh | Đã bước vào thang đi lên | Ở lại xưởng đêm nay |
| dac_phai | Đã bước vào thang đi lên | Ở lại xưởng đêm nay |
| dieu_chuyen | Ở lại xưởng | Ở lại xưởng |

Chương 3 dự định mở theo nơi Kael đứng.

## 14. Các điểm người viết đã chốt thêm (04/10/2026)

Kịch bản không nói rõ 13 điểm dưới đây. Người viết đã chốt, và các mục phía trên đã viết theo đúng các quyết định này. Bảng để dev tra nhanh.

| # | Điểm | Quyết định | Nằm ở mục |
|---|---|---|---|
| 1 | Mảnh 14, 15, 16 mở lúc nào | Ngay sau Câu 5, trước đoạn kết, ở mọi cách kết kể cả kết cục sớm (giống Mảnh 06) | 9, 11 |
| 2 | Ba mảnh mở cùng lúc thì báo mấy lần | Một thông báo, nháy một lần | 11 |
| 3 | Nhạc tắt ở đâu khi kịch bản không có thẻ | Tắt ở đầu Cảnh 3; Cảnh 3 và 4 im | 5 |
| 4 | Đáp án lượt hai có xáo thứ tự và giấu nhãn không | Có, như lượt một | 7 |
| 5 | Không có ghi chú thì nút [Đòn] hiện mờ hay ẩn hẳn | Ẩn hẳn | 7 |
| 6 | Biên độ hai thanh | Lung lay 0 đến 10, Dao động 0 đến 11 | 7 |
| 7 | Nút quay lại ở kết cục 3/7 | Cho chọn Lựa chọn 3, 4 hoặc 5 | 9 |
| 8 | Mô tả vật phẩm "Nhãn hòm thuốc" | "Một dãy số và một cái ngày. Không có tên." | 9 |
| 9 | Vật phẩm "Thẻ công nhân xưởng đúc số 4" có mô tả không | Không có mô tả riêng; chỉ hiện tên | 10 |
| 10 | Lời `Vane (to tiếng)` hiển thị ra sao | Chữ to hơn hoặc rung khung thoại, tùy dev | 3 |
| 11 | Thiết bị cổ tay "ấm lên", "lạnh đi" có hiệu ứng không | Không bắt buộc; nếu có thì dùng màu riêng | 5 |
| 12 | 4-A ở nhánh Thẻ đặc phái: `long_tin` +1 thay cho `lam_dung_lenh` +1, hay cộng cả hai | Thay: chỉ cộng `long_tin` +1 | 5 |
| 13 | Đáp án dài trong trận đối chất | Nút chỉ hiện phần nêu ý; các câu dẫn chứng Kael nói sau khi chọn. Áp dụng cả cho trận Helena ở Chương 1 | 2, 3, 7 |

Gặp điểm nào khác mà kịch bản và tài liệu này đều không nói: hỏi người viết, đừng tự quyết.

## 15. Không tự ý đổi

Như mục 13 của `HUONG_DAN_DEV.md`, thêm:

- Trong lời thoại không ai gọi tên thuốc. "Soma-X" chỉ có ở tiêu đề cảnh, **không hiện** cho người chơi ở bất cứ đâu, kể cả tên cảnh trên màn hình.
- Không hiện chữ "Đòn", "Giữ lời", "Lùi", cũng không báo cho người chơi biết còn một đáp án bị ẩn vì thiếu ghi chú.
- Veritas im ở Cảnh 4 và Cảnh 5; không thêm lời hay sprite của cô vào hai cảnh này.
- Cảnh 3 và Cảnh 4 không có nhạc. Không thêm nhạc nền cho đỡ trống.
- Con số trên tấm bảng suất (chín, tám, bảy, sáu phần) và nhịp gạch (chín tuần, năm tuần, ba tuần) không được đổi.
