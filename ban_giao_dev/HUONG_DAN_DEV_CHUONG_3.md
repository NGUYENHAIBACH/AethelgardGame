# Hướng dẫn cho dev: Chương 3

> **Cập nhật 05/10/2026:** cách hiện sprite và các chỗ tắt nhạc đã đổi (sân khấu tối đa ba người, thẻ `[SPRITE: … tắt]`, mọi chỗ tắt nhạc đều có thẻ). Đọc `HUONG_DAN_DEV_SAN_KHAU.md`; chỗ nào file này nói khác thì theo file đó (bảng đối chiếu ở mục 5 của nó).

Người viết kịch bản duyệt ngày 05/10/2026.

Tài liệu này nối tiếp `HUONG_DAN_DEV.md` (Mở đầu, Chương 1) và `HUONG_DAN_DEV_CHUONG_2.md`. Mọi quy ước ở hai tài liệu đó vẫn giữ nguyên. Ở đây chỉ ghi cái mới và cái khác. Khi tài liệu này và kịch bản lệch nhau thì kịch bản đúng.

## 1. Gói bàn giao

| File | Nội dung |
|---|---|
| `03_chuong_3.md` | Chương 3: "Sổ cái". 4 cảnh, hai màn chơi mới (xếp sổ, gắn lời kể), Lựa chọn 6, 3 kết cục kèm bài học, Mảnh 17 đến 21 |
| `Kich_ban_Chuong_3.docx` | File trên ở dạng đọc và in. Chữ giống hệt bản `.md` |

Lời thoại đã chốt. Dev không sửa chữ, không đổi thứ tự câu, không gộp hay tách lượt thoại. Thấy chỗ nào vô lý hoặc không làm được thì báo lại người viết.

Chương này không sửa gì ở Mở đầu, Chương 1, Chương 2.

**Chương 3 không có trận đối chất hai thanh đo.** Thay vào đó là hai màn chọn ở Cảnh 2 và Cảnh 3 (mục 5, 6).

## 2. Ký hiệu mới trong kịch bản Chương 3

| Trong kịch bản | Xử lý |
|---|---|
| `**Bản trên**`, `**Bản dưới**`, `- Bản trên.`, `- Bản dưới.` | **Không hiện.** Điều kiện theo biến `ban` (mục 3) |
| `- Nếu T1.`, `- Nếu T2.`, `- Nếu D1.`, `- Nếu D2 hoặc D3.` | **Không hiện.** Điều kiện theo biến `loi_vao` (mục 3), chỉ dùng ở Cảnh 4 và kết chương |
| `- Nếu thắng Vane`, `bất phân`, `bị Vane thuyết phục` | `vane` = dong_minh / dung_ngoai / chu |
| `- Nếu đã nối mắt xích 3.`, `- Nếu đã gắn điều 3 ở Cảnh 3.`, `- Nếu sổ thiếu.`, `- Nếu sổ đủ` | Điều kiện theo kết quả Cảnh 2 và Cảnh 3 (mục 5, 6). "Sổ đủ" là `so_cai` ≥ 4 |
| `- Nếu veritas = dong_hanh:` / `do_du:` | Điều kiện theo biến `veritas` |
| `- Nếu có ghi chú "Tệp 4406-16…":` | Điều kiện theo việc đang giữ ghi chú ấy |
| `→ Người chơi chọn ghi chú "X".`, `→ Người chơi chọn mắt xích "X".` | **Không hiện.** Chỉ ra đáp án đúng của màn chọn |
| `**Luật chọn…**`, `**Luật gắn…**` và các gạch đầu dòng bên dưới | **Không hiện** như lời thoại. Là luật của màn chơi; các dòng `Veritas: "…"` trong đó là lời cô nói khi chọn sai |
| `(chỉ T2)`, `(T2: chỉ khi sổ đủ)` sau nhãn `[6-B]`, `[6-C]` | **Không hiện.** Điều kiện để nút ấy xuất hiện |
| `(Ghi chú cho người làm game: …)` cuối mỗi cảnh | **Không hiện.** Đọc kỹ: mỗi cảnh có một đoạn, tài liệu này tóm lại chứ không thay nó |

**Ô tên** không có sprite: Mẹ, Doran, Thợ học việc, Người gác thang, Dẫn chuyện, A.L.I.C.E (loa).

**Veritas.** `Veritas (rất khẽ)` vẫn như Chương 2: có tiếng, không có sprite. Ở bản trên, suốt Cảnh 2 và Cảnh 3 cô chỉ có tiếng và mọi lời của Kael là thì thầm (có thể cho chữ nhỏ hoặc nghiêng). Từ nhịp 3 của nhánh 6-C cô hiện hình trước mặt người khác: đây là lần đầu trong game, đừng làm mất bằng cách cho cô hiện sớm hơn.

## 3. Biến mới

| Biến | Giá trị | Đặt ở đâu |
|---|---|---|
| `ban` | tren / duoi | Đầu Cảnh 1. tren: `giay_to` là thong_hanh hoặc dac_phai **và** `vane` khác chu. duoi: còn lại |
| `so_cai` | 2 đến 5 | Cảnh 2: số mắt xích nối được |
| `mat_xich_3`, `mat_xich_4` | co / khong | Cảnh 2. Dùng lại ở Cảnh 3 và Cảnh 4 |
| `bac_dung` | co / khong | Cảnh 3, điều 1: "co" chỉ khi **lần chọn đầu tiên** là "Không khớp với thứ mình đang giữ" |
| `dieu_3` | gan / treo | Cảnh 3, điều 3 |
| `veritas` | dong_hanh / do_du | Cuối Cảnh 3: dong_hanh khi `bac_dung` = co **và** `dieu_3` = gan |
| `loi_vao` | T1 / T2 / D1 / D2 / D3 | Đầu Cảnh 4, suy ra theo bảng dưới |
| `lua_chon_6` | A / B / C | Cảnh 4 |
| `lo` | co / khong | Cuối nhánh 6-C: co ở T2, D2, D3; khong ở T1, D1 |

| `loi_vao` | Điều kiện |
|---|---|
| T1 | `giay_to` = thong_hanh và `vane` khác chu |
| T2 | `giay_to` = dac_phai và `vane` khác chu |
| D1 | `giay_to` = dieu_chuyen và `vane` = dong_minh |
| D2 | `giay_to` = dieu_chuyen và `vane` = dung_ngoai |
| D3 | `vane` = chu, mọi loại giấy |

Chương 3 không có thanh đo và không có bộ đếm lập trường.

## 4. Luồng Chương 3

| Bước | Nội dung | Tài nguyên | Sổ tay và biến |
|---|---|---|---|
| **Cảnh 1: Sáng Ngày 4** | Hai bản theo `ban`, rồi đoạn "Chỗ vắng". Không có lựa chọn, không mở mảnh | Trên: BG03, BG04, BGM01, Helena_Neutral. Dưới: BG05, BG07, BG06, không nhạc, Vane_Neutral, Rian_Neutral. Veritas_Hologram ở "Chỗ vắng" | Trên: ghi chú "Sổ ký của bà Helena…"; T2 thêm vật phẩm "Tờ lời bà Helena soạn". D1: vật phẩm "Xấp nhãn hòm thuốc", ghi chú "Ba tệp trùng ngày suất bị bớt…" |
| **Cảnh 2: Sổ cái** | Màn xếp sổ, tối đa sáu mắt xích (mục 5) | Trên: BG02, SE02. Dưới: BG06, Veritas_Hologram. BGM09 từ mắt xích 1 | `so_cai`. Mảnh 17 (sau mắt xích 1), Mảnh 18 (cuối cảnh). Có thể thêm ghi chú "Tệp 4406-16…" hoặc "Tệp ống số 7 và mười hai phiếu…" |
| **Cảnh 3: Cột ngày xưa** | Màn gắn lời kể, năm điều (mục 6) | Nền như Cảnh 2. BGM09 chạy tiếp, tắt từ điều 4 | `bac_dung`, `dieu_3`, `veritas`. Mảnh 19 (sau điều 1), Mảnh 20 (sau điều 4). Ghi chú "Veritas kể về ngày xưa: …". dong_hanh: `kiem_chung` +1 |
| **Cảnh 4: Đặt sổ** | Mở theo `loi_vao`, rồi Lựa chọn 6 | Trên: BG02, BG04, Helena_Neutral. Dưới: BG07, Vane_Neutral. Không nhạc | |
| **Lựa chọn 6** | Xem mục 7 | | 6-A: `lam_dung_lenh` +1. 6-B: `long_tin` +1 (T2) hoặc `con_so` +1 (D3). 6-C: `kiem_chung` +1 |
| Nhánh 6-A, 6-B | Kết cục (mục 8) | | |
| Nhánh 6-C | Bốn nhịp trước mặt Helena hoặc Vane | BGM02 hoặc BGM03 từ câu hỏi nguồn ở nhịp 2; BGM07 và Veritas_Hologram từ nhịp 3; Helena_Angry, Helena_Surprised, Vane_Surprised | Mảnh 21 (cuối nhịp 1). `lo`. T1: vật phẩm "Giấy của Ban Cố vấn cho chuyến thang chiều"; T1 và dong_hanh: ghi chú "Bà Helena đã chép lời Veritas kể vào sổ riêng của bà" |
| **Kết Chương 3** | Bản trên xuống thang; cả hai bản nghe tin Rian | BG03, SE06, BG05, không nhạc | |

**Đoạn quầy suất ở Cảnh 1** chỉ có ở D1 và D2 (Lệnh điều chuyển, không bị thuyết phục). D2 sau đoạn ấy chuyển nền sang BG07. D3 mở thẳng trong xưởng.

## 5. Cảnh 2: màn xếp sổ

Veritas hỏi; người chơi chọn một ghi chú hoặc vật phẩm đang có để trả lời.

**Giao diện.** Danh sách gồm **mọi** ghi chú và vật phẩm người chơi đang có, cộng nút "Không có gì" ở cuối. Không đánh dấu thứ nào là đúng.

**Luật** (nguyên văn ở khối "Luật chọn" đầu cảnh):

- Chọn sai: Veritas nói một trong ba câu (xoay vòng), danh sách mở lại. Không trừ gì.
- "Không có gì" khi thật sự không có thứ khớp: mắt xích bỏ trống, chạy đoạn "Nếu chọn Không có gì".
- "Không có gì" khi đang giữ thứ khớp: lần đầu Veritas nhắc, danh sách mở lại; lần hai thì mắt xích bỏ trống.
- Mắt xích 1 và 2 không bỏ trống được: Veritas nhắc rồi mở lại danh sách, bao nhiêu lần cũng thế.

| Mắt xích | Thứ khớp | Ai có |
|---|---|---|
| 1 | Ghi chú "Bảng số ống 7" | Mọi người |
| 2 | Ghi chú "Lò lớn tắt từ năm 2310…" | Mọi người |
| 3 | Một trong: "Bảng suất Tầng Đáy", "Lời người bốc hàng", "Bữa sáu phần", "Xấp nhãn hòm thuốc", hoặc dòng "Tấm bảng ở quầy suất" | Dòng "Tấm bảng ở quầy suất" tự thêm vào danh sách cho D1 và D2 |
| 4 | Một trong: "Mười hai tệp chưa xóa", "Sổ ký của bà Helena" | 2-B, hoặc bản trên |
| 5 | "Lời bà cụ" | Chỉ 1-B. Không có thì cả mắt xích không hiện |
| 6 | "Số đo đai vá" | Chỉ 1-C. Không có thì cả mắt xích không hiện |

- Mỗi mắt xích nối được: `so_cai` +1. Có nhiều thứ khớp thì người chơi chọn một, không cộng thêm.
- 1-B và 1-C loại trừ nhau, nên `so_cai` cao nhất là 5.
- Đoạn "Tấm nhãn": chỉ khi `vane` = dong_minh và `ban` = tren; chạy ngay sau mắt xích 3 dù người chơi chọn gì. Cho ghi chú "Tệp 4406-16…".
- Đoạn "Năm trên năm": chỉ khi `vane` = dong_minh và `ban` = duoi (tức D1). Cho ghi chú "Tệp ống số 7 và mười hai phiếu y tế cũng về máy 2231".
- Cuối cảnh rẽ "sổ đủ" (`so_cai` ≥ 4) hoặc "sổ thiếu".

Bảng kiểm thử `so_cai`:

| Người chơi | `so_cai` |
|---|---|
| Bản trên, có một ghi chú cho mắt xích 3, và 1-B hoặc 1-C | 5 |
| Bản trên, có ghi chú cho mắt xích 3, chọn 1-A | 4 |
| Bản trên, không có gì cho mắt xích 3, chọn 1-A | 3 |
| D1 hoặc D2, chọn 2-A và 1-A | 3 |
| D3 từ Lệnh điều chuyển, chọn 2-A, 1-A, 3-A, 4-A hoặc 4-B | 2 |

## 6. Cảnh 3: màn gắn lời kể

Veritas kể một điều cô nhớ; người chơi chọn chỗ gắn nó vào.

**Giao diện.** Danh sách gồm các mắt xích của Cảnh 2 theo tên (ví dụ "Cái ống và cái lò"); mắt xích Cảnh 2 bỏ trống thì hiện mờ, kèm chữ "còn trống"; mắt xích 5, 6 không có thì không hiện. Cuối danh sách có hai nút: "Chưa gắn được vào đâu" và "Không khớp với thứ mình đang giữ".

| Điều | Đáp án đúng | Ghi chú |
|---|---|---|
| 1 | "Không khớp với thứ mình đang giữ" | **Chỉ chọn một lần.** Đúng ngay lần đầu: `bac_dung` = co, chạy "Nếu bác ngay". Chọn bất cứ thứ gì khác: chạy "Nếu không bác". Không cho chọn lại |
| 2 | Mắt xích 2 | Sai thì chọn lại |
| 3 | Mắt xích 3 nếu `mat_xich_3` = co (`dieu_3` = gan). Nếu khong: "Chưa gắn được vào đâu" (`dieu_3` = treo) | Sai thì chọn lại |
| 4 | Mắt xích 1, hoặc mắt xích 4 nếu đã nối. Hai đáp án, hai đoạn thoại khác nhau | Sai thì chọn lại |
| 5 | "Chưa gắn được vào đâu", hoặc mắt xích 2 | Sai thì chọn lại |

Lời Veritas khi chọn sai, khi chọn mắt xích còn trống, khi chọn "Chưa gắn được" mà có chỗ gắn, và khi chọn "Không khớp" ở điều 2 đến 5: nguyên văn ở khối "Luật gắn".

**Tên ghi chú "Veritas kể về ngày xưa: …" là động:** liệt kê đúng các điều đã gắn. Điều 1 và điều 5 không bao giờ vào. Hai dạng: có hoặc không có vế "bản tin từng đọc sau khi đếm" (theo `dieu_3`).

Trong cảnh có vài dòng đổi theo `lua_chon_1` (1-A), `lua_chon_3` (3-B, 3-C), `lua_chon_4` (4-B) và `giay_to` (Thẻ đặc phái).

## 7. Lựa chọn 6

| `loi_vao` | Nút hiện |
|---|---|
| T1 | 6-A, 6-C |
| T2 | 6-A, 6-B, và 6-C nếu `so_cai` ≥ 4 |
| D1 | **Không hiện lựa chọn.** Chạy thẳng nhánh 6-C; đặt `lua_chon_6` = C, `kiem_chung` +1 |
| D2 | 6-A, 6-C |
| D3 | 6-A, 6-B, và 6-C nếu `so_cai` ≥ 4 |

- Chữ trên nút khác nhau giữa bản trên và bản dưới, và 6-B khác nhau giữa T2 và D3. Kịch bản liệt kê đủ.
- Xáo thứ tự nút, không hiện nhãn A, B, C, như các lựa chọn trước.
- Nút 6-C thiếu thì **ẩn hẳn**, không hiện mờ, không báo.
- T2 hoặc D3 mà `so_cai` < 4: chỉ còn hai nút và cả hai đều là kết cục. Đây là chủ ý của người viết.

**Nhánh 6-C** chạy bốn nhịp. Bản trước Helena (T1, T2) và bản trước Vane (D1, D2, D3) là hai bộ thoại riêng; nhịp 1 đến 3 dùng chung trong mỗi bộ, nhịp 4 tách theo `loi_vao`. Các dòng điều kiện bên trong: `mat_xich_3`, `mat_xich_4`, `lua_chon_2` (2-A, 2-B), ghi chú "Tệp 4406-16", `so_cai` < 4 ("sổ thiếu"), `dieu_3`, `lua_chon_0` (0-B), `da_dung_don_cau_4`, `veritas`, `lua_chon_3` (3-A, lối D3).

## 8. Ba kết cục

| Kết cục | Đến từ | Nền, nhạc | Đoạn rẽ |
|---|---|---|---|
| 6/7 "Thăng chức" | 6-A, mọi lối | BG08, BGM05_Sad_Piano_2 | Dòng dẫn chuyện đầu tiên khác nhau giữa bản trên và bản dưới |
| 2/7 "Người phát ngôn" | 6-B ở T2 | BG03, BGM05_Sad_Piano_2, kết bằng SE04 | |
| 3/7 "Không còn đói" | 6-B ở D3 | BG10, BGM05_Sad_Piano_2 | Hai dòng chỉ chạy khi `giay_to` = dieu_chuyen |

- Mỗi kết cục: màn hình "KẾT CỤC n/7", khối Bài học kết cục (chép nguyên văn khối `>`), nút quay lại.
- **Kết cục 3/7 giờ có hai lối vào:** cuối Chương 2 (đã giao) và lối D3 ở đây. Phần dẫn chuyện của hai lối khác nhau. Bài học chỉ khác ở đoạn đầu; lối nào hiện bài của lối ấy. Trong màn tổng kết kết cục vẫn tính là một kết cục.
- Kết cục 2/7: tiếng nổ SE04 là âm thanh cuối cùng, rồi mới hiện màn hình kết cục.

## 9. Vật phẩm và ghi chú của Chương 3

| Mục | Loại | Ai có | Dùng trong chương này |
|---|---|---|---|
| Sổ ký của bà Helena: mỗi sáng một danh sách nhãn chỉ có dãy số và ngày, dòng nào cũng có chữ ký của bà | Ghi chú | Bản trên | Mắt xích 4 |
| Tờ lời bà Helena soạn | Vật phẩm | T2 | Nhánh 6-B |
| Xấp nhãn hòm thuốc | Vật phẩm | D1. **Thay** vật phẩm "Nhãn hòm thuốc" của Chương 2 | Mắt xích 3 |
| Nhãn hòm thuốc (từ Chương 2) | Vật phẩm | Thắng Vane, bản trên | Thêm dãy số 4406-16 vào mô tả sau đoạn "Tấm nhãn" |
| Ba tệp trùng ngày suất bị bớt đều về máy 2231, trong một xưởng hai chục máy | Ghi chú | D1 | |
| Tệp 4406-16: bị xóa đúng hôm suất xuống sáu phần, người xóa là máy 2231 | Ghi chú | Thắng Vane, bản trên | Nhánh 6-C, nhịp 1 |
| Tệp ống số 7 và mười hai phiếu y tế cũng về máy 2231 | Ghi chú | D1 | |
| Veritas kể về ngày xưa: … | Ghi chú, tên động | Mọi người | Nhánh 6-C |
| Giấy của Ban Cố vấn cho chuyến thang chiều | Vật phẩm | T1, nhánh 6-C | Kết chương |
| Bà Helena đã chép lời Veritas kể vào sổ riêng của bà | Ghi chú | T1, nhánh 6-C, dong_hanh | Để dành Chương 5 |

## 10. Mảnh lưu trữ

| Mảnh | Tên | Ai nhận | Mở lúc nào |
|---|---|---|---|
| 17 | "Xui, hay là phải thế" | Mọi người | Cảnh 2, sau mắt xích 1 |
| 18 | "Ba chuyện, một cái lệ" | Mọi người | Cuối Cảnh 2 |
| 19 | "Nhớ như in" | Mọi người | Cảnh 3, sau điều 1 (cả hai cách xử) |
| 20 | "Vẫn cái lô ấy" | Mọi người | Cảnh 3, sau điều 4 |
| 21 | "Mỗi người một việc" | Ai đi nhánh 6-C | Cảnh 4, cuối nhịp 1 |

Người chơi rơi vào kết cục ở 6-A hoặc 6-B không có Mảnh 21. Không mảnh nào có số liệu động. Nội dung chép nguyên văn từ các khối `>`.

## 11. Tài nguyên dùng trong Chương 3

| Loại | Tên file | Ghi chú |
|---|---|---|
| Nền | BG02_Phong_Luu_Tru_Du_Lieu, BG03_Thap_Nang_Luong_Sup_Do, BG04_Hanh_Lang_Toi_Tang_Trung, BG05_Khu_O_Chuot_Tang_Day, BG06_Duong_Ong_Ngam_Tang_Day, BG07_Dai_Ban_Doanh_Khang_Chien, BG08_Aethelgard_Dong_Bang, BG10_Tang_Day_No_Du_Nhung_U_Toi | BG08 lần đầu dùng (Kết cục 6) |
| Nhạc | BGM01_Dark_Ambient_Cyberpunk, BGM02_Tension_Debate, BGM03_Heavy_Industrial, BGM05_Sad_Piano_2, BGM07_Bittersweet_Ambient, **BGM09_Investigation** | **BGM09 là bản mới, chưa có file.** Kiểu lần manh mối, phá án; phải chạy lặp được lâu vì phủ gần hết Cảnh 2 và nửa Cảnh 3 |
| Hiệu ứng | SE02_Tieng_Go_Ban_Phim, SE04_Tieng_No_Thap_Nang_Luong, SE06_Cua_Thep_Mo_Ra | |
| Sprite | Helena_Neutral, Helena_Angry, Helena_Surprised, Vane_Neutral, Vane_Surprised, Rian_Neutral, Veritas_Hologram | Helena_Surprised lần đầu dùng |

- BG04 dùng cho Trạm Ban Cố vấn và hành lang ngoài cửa. BG07 là xưởng đúc số 4: nguội ở D1, D2; lò đỏ ở D3. BG06 là khoảng lò lớn.
- BGM02 và BGM03 chỉ bật trong nhịp 2 của nhánh 6-C, tới khi BGM07 thay.
- Chương này không có sprite Kael riêng theo cảm xúc ngoài các thẻ đã ghi trong kịch bản.

## 12. Dữ liệu phải mang sang Chương 4

- Mọi biến đã có, cộng: `so_cai`, `mat_xich_3`, `mat_xich_4`, `bac_dung`, `dieu_3`, `veritas`, `loi_vao`, `lua_chon_6`, `lo`.
- Năm khuynh hướng, hai lực lượng, toàn bộ vật phẩm, ghi chú, mảnh.
- **Mọi người chơi vào Chương 4 đều đã chọn 6-C và đều đang ở Tầng Đáy.** Từ Chương 4 loại giấy tờ thôi rẽ nhánh.
- Từ Chương 4 Veritas luôn hiện hình và nói trước mặt người khác.

## 13. Các điểm kịch bản không nói rõ

Người viết đã chốt các điểm dưới đây (05/10/2026); các mục phía trên viết theo đúng các quyết định này.

| # | Điểm | Quyết định | Ghi chú |
|---|---|---|---|
| 1 | Nút 6-C thiếu thì hiện mờ hay ẩn hẳn | Ẩn hẳn, không báo (như nút [Đòn] ở Chương 2) | |
| 2 | Lối D1 không có lựa chọn: có cộng `kiem_chung` +1 không | Có, coi như đã chọn 6-C | |
| 3 | Kết cục 3/7 có hai lối vào: đếm là mấy kết cục | Một. Bài học hiện theo lối người chơi vừa đi | |
| 4 | Màn gắn ở Cảnh 3: mắt xích bỏ trống hiện ra sao | Hiện mờ kèm chữ "còn trống", vẫn bấm được (Veritas có câu đáp riêng) | Kịch bản đã ghi |
| 5 | Lời thì thầm của Kael ở bản trên (Cảnh 2, 3) hiển thị ra sao | Chữ nhỏ hơn hoặc nghiêng, tùy dev | |
| 6 | Vật phẩm cũ sau nhánh 6-C | "Tờ lời bà Helena soạn" rời sổ tay ở T2 (Kael để lại trên bàn). "Thẻ đặc phái" ở T2 vẫn nằm trong sổ tay, không còn tác dụng | |
| 7 | Nút quay lại ở ba kết cục: quay về đâu | Quay về Lựa chọn 6, đúng như kịch bản ghi | Tạm theo kịch bản. Với T2, D3 mà sổ thiếu thì quay lại đó vẫn chỉ có hai kết cục; người viết có thể mở thêm điểm quay về sau |
| 8 | Mô tả của các vật phẩm mới ("Tờ lời bà Helena soạn", "Xấp nhãn hòm thuốc", "Giấy của Ban Cố vấn cho chuyến thang chiều") | Chỉ hiện tên, không có mô tả riêng (như "Thẻ công nhân xưởng đúc số 4" ở Chương 2) | Tạm; người viết có thể soạn mô tả sau |

Gặp điểm nào khác mà kịch bản và tài liệu này đều không nói: hỏi người viết, đừng tự quyết.

## 14. Không tự ý đổi

Như các tài liệu trước, thêm:

- Không đánh dấu, gợi ý hay tô màu đáp án đúng trong hai màn chọn. Người chơi phải đối chiếu thứ mình đang giữ.
- Điều 1 của Cảnh 3 chỉ chọn một lần. Không cho chọn lại, không báo trước là chỉ có một lần.
- Không hiện con số `so_cai`, không báo "sổ đủ" hay "sổ thiếu" ngoài lời thoại đã có.
- Veritas không hiện hình trước mặt người khác cho tới nhịp 3 của nhánh 6-C. Ở bản trên, sau khi rời phòng Helena, cô chỉ hiện trong lồng thang và tắt trước khi cửa mở.
- Cảnh 4 mở không có nhạc. Kết chương không có nhạc.
- Các con số: lô 4.414 và 4.415; ba mã tệp 4350-03, 4385-21, 4406-16; hai mươi bốn bể tảo, mười chín bể chạy, năm nền bỏ không; hạn ống 2305, 2310, 2318.
