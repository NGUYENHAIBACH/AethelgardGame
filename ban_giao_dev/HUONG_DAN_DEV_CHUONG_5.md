# Hướng dẫn cho dev: Chương 5

**Người viết kịch bản duyệt 08/10/2026,** kèm 10 quyết định ở mục 14.

Tài liệu này nối tiếp `HUONG_DAN_DEV.md` (Mở đầu, Chương 1), `HUONG_DAN_DEV_CHUONG_2.md`, `HUONG_DAN_DEV_CHUONG_3.md`, `HUONG_DAN_DEV_CHUONG_4.md` và `HUONG_DAN_DEV_SAN_KHAU.md` (luật sprite và nhạc). Mọi quy ước ở các tài liệu đó vẫn giữ nguyên. Ở đây chỉ ghi cái mới và cái khác. Khi tài liệu này và kịch bản lệch nhau thì kịch bản đúng.

## 1. Gói bàn giao

| File | Nội dung |
|---|---|
| `05_chuong_5.md` | Chương 5: "A.L.I.C.E". 4 cảnh, Lựa chọn 10, trận cuối "Gọi ai?", 2 kết cục kèm bài học (5/7 lối Chương 5, 7/7), Mảnh 26 đến 32 |
| `Kich_ban_Chuong_5.docx` | File trên ở dạng đọc và in. Chữ giống hệt bản `.md` |

Lời thoại đã chốt. Dev không sửa chữ, không đổi thứ tự câu, không gộp hay tách lượt thoại. Thấy chỗ nào vô lý hoặc không làm được thì báo lại người viết.

Đây là chương cuối. Chương này có bốn việc mới so với các chương trước:

1. **Trận cuối không dùng hai thanh đo và không dùng ghi chú.** Người chơi chọn *gọi ai ra trả lời* trong năm nút cố định (mục 6).
2. **Một đoạn được gọi như chương trình con:** "Khi một chốt vỡ" (mục 6).
3. **Hình cuối ghép từ nhiều lớp, đổi theo nhánh** (mục 9).
4. **Khối "Lối bạn hay chọn" ở màn kết cục, áp dụng cho cả bảy kết cục của game,** kể cả các kết cục đã làm ở Chương 1 đến 4 (mục 10).

Chương này không sửa chữ nào ở Mở đầu và Chương 1 đến 4.

## 2. Ký hiệu mới trong kịch bản Chương 5

Các ký hiệu đã có ở Chương 4 (`- Nếu 7-A`, `- Nếu 8-B`, `- Nếu veritas = dong_hanh`, `- Nếu rian_len = co`, `- Nếu không.`, tiêu đề in đậm, dòng `→ biến`, `[TRẢ: …]`, `[GIEO: …]`…) xử lý như cũ. Dưới đây là cái mới.

| Trong kịch bản | Xử lý |
|---|---|
| `- Nếu bà Helena là đồng minh` | `helena` = dong_minh (biến lập ở Chương 4: kết Thuyết phục, hoặc Bất phân mà đã chọn 8-A) |
| `- Nếu bà Helena đứng ngoài` | `helena` = dung_ngoai (kết Bất phân và 8-B) |
| `- Nếu Kael là người của bà Helena`, `kết với bà Helena (ở Chương 1) là Bị thuyết phục` | `ket_doi_chat_helena` = bi_thuyet_phuc |
| `- Nếu kết với bà Helena là Thuyết phục` / `Bất phân`; `có lính gác của bà Helena` / `không có lính gác của bà Helena` | `ket_doi_chat_helena`. **"Có lính gác của bà Helena" chỉ là kết Thuyết phục,** hẹp hơn "đồng minh" |
| `- Nếu lối T1`, `lối T1 hoặc T2`, `bà Helena chưa gặp Veritas` | `loi_vao` của Chương 3. "Chưa gặp Veritas" là `loi_vao` khác T1 và khác T2 |
| `- Nếu 0-B`, `2-A`, `2-B`, `đã chọn 1-C` | `lua_chon_0`, `lua_chon_2`, `lua_chon_1` |
| `- Nếu ở Chương 1 Kael có ghi chú "Lời bà cụ"` / `"Khóa ngắt dự phòng"` | Đang giữ ghi chú ấy (tương đương 1-B / 1-C) |
| `- Nếu ở Chương 2 Kael cầm Thẻ đặc phái` | `giay_to` = dac_phai |
| `- Nếu ở Chương 2 Kael thuyết phục được Vane` | Biến suy ra `thang_vane` (mục 3): kết trận Vane ở Chương 2 là thắng. Không dùng thẳng giá trị hiện tại của `vane`, vì `vane` có thể đã đổi ở Chương 4 |
| `- Nếu thư ký đã đòi thiết bị` | `doi_thiet_bi` = co (Chương 4) |
| `- Nếu có người bước vào lồng theo Kael` / `không ai bước vào lồng theo Kael` | Ít nhất một trong `rian_len`, `vane_len`, `tao_len` là co / cả ba là khong |
| `- Nếu 10-A`, `10-B`, `10-C`, `không phải 10-B`, `không phải 10-C`, `đã chọn 10-B` | `lua_chon_10` |
| `- Nếu giu_lo = kip` / `muon`, `giu_thang = …`, `nha_len = …` | Biến mới (mục 3) |
| `- Nếu Tầng Đáy đông` / `thưa` | Điểm lực lượng `tang_day` từ 3 trở lên / dưới 3, **tính lúc vào Cảnh 2** (sau Lựa chọn 10) |
| `- Nếu Tầng Trung đông` / `thưa` | Điểm lực lượng `tang_trung` từ 1 trở lên / bằng 0, tính lúc vào Cảnh 2 |
| `- Nếu có người Tầng Trung lên` (Cảnh 4) | Tầng Trung đông, **hoặc** `nha_len` = kip |
| `- Nếu Kael bị giữ` / `Kael không bị giữ` | Biến suy ra `bi_giu` (mục 3) |
| `- Nếu có người đi cùng Kael` | Biến suy ra `co_nguoi_di_cung` (mục 3). Rộng hơn "có người bước vào lồng theo Kael" |
| `- Nếu ở Cảnh 1 Kael đã nói "Lên tới nơi tôi tháo"` | Biến suy ra `hen_thao` (mục 3) |
| `- Nếu mat_thiet_bi = co`, `doan_mong dưới 2`, `vao_day = rian` / `tao` / `khong`, `vao_trung = co`, `vao_thuoc = co` | Biến mới của Cảnh 2 (mục 3) |
| `- Nếu vào một mình` | `vao_day` = khong **và** `vao_trung` = khong |
| `→ Gọi ai?` | Hiện màn chọn năm nút của trận cuối (mục 6) |
| `**Gọi "…" (đúng)**`, `**Gọi "…" (sai)**` | **Không hiện.** Khối chạy sau khi người chơi bấm nút mang tên ấy. Chữ "(đúng)", "(sai)" là ghi chú cho dev, không bao giờ lên màn hình |
| `→ vung +2.`, `→ vung +1.`, `→ mat_chot +1.`, `→ kho_goc = co.` | **Không hiện.** Đặt biến |
| `→ Chạy "Khi một chốt vỡ".` | **Không hiện.** Gọi đoạn `### Khi một chốt vỡ`, chạy xong thì quay về (mục 6) |
| `- Nếu vung từ 6 trở lên: sang "Thắng".` / `vung dưới 6: sang "Thua".` | **Không hiện.** Nhảy tới đoạn `### Thắng` hoặc `### Thua` |
| `- Nếu ở Câu 5 đã không gọi "Cái lô chiều nay"` | Biến suy ra `sai_cau_5` (mục 3) |
| `A.L.I.C.E:` (không có chữ "loa") | A.L.I.C.E đang đứng trên sân khấu, có sprite (Cảnh 3) |
| `A.L.I.C.E (loa):` | Tiếng loa như các chương trước; không đổi gì trên sân khấu, kể cả khi sprite của nó đang hiện |
| Dòng hát (Cảnh 4): lời trong ngoặc kép của Mẹ và Veritas, bốn khổ | Hiện như thoại thường, **không phát bản thu ở đây**. Bản thu bài hát phát ở hình cuối (mục 9) |
| `━━ KẾT CỤC 7/7: BÌNH MINH ━━` | Màn hình kết cục, sau khi giữ hình cuối vài giây (mục 9) |
| `(Hết Cảnh 3.)`, `(Hết Chương 5.)` | **Không hiện.** Mốc cho dev |

**Ô tên.**

| Ô tên | Hình | Ghi chú |
|---|---|---|
| Soren, Ilsa | Sprite `Soren_Neutral`, `Ilsa_Neutral` (chờ vẽ) | Tự lên sân khấu khi nói và chiếm chỗ đứng như Vane; rời bằng thẻ `tắt` |
| A.L.I.C.E | Sprite `ALICE_Neutral` (chờ vẽ) | Chỉ ở Cảnh 3; lên bằng thẻ, ở lại tới hết cảnh |
| Cố vấn Corvin | Bóng `Bong_Co_van` (chờ vẽ) | |
| Người làm thuốc | Bóng `Bong_Nguoi_lam_thuoc` (chờ vẽ) | |
| Thợ van | Bóng `Bong_Tho_van` (chờ vẽ) | Chính là người mang ô tên "Công nhân" ở Chương 1 |
| Người nhà | Chưa có bóng: chỉ hiện ô tên (có `Bong_Nguoi_nha` thì dùng) | Ô tên mới. Người vợ có chồng nằm trạm |
| Lính | Bóng `Bong_Linh` (chờ vẽ), dùng chung | |
| Lính gác | Chỉ hiện ô tên | Lính gác của bà Helena |
| Người dỡ hàng | Chỉ hiện ô tên | Ô tên mới (người dỡ hàng ở Tầng Trung; khác "Người bốc hàng" ở Tầng Đáy) |
| Thợ già, Chị thợ, Thợ bể tảo | Bóng đã có từ Chương 2 và 4 | |
| Người gác thang, Thư ký, Bà cụ, Người trong đám đông | Như các chương trước; chưa có bóng thì chỉ hiện ô tên | Bà cụ chỉ nói khi có ghi chú "Lời bà cụ" |
| Dẫn chuyện | Không có hình | Dùng nhiều ở Kết cục 5/7 và đoạn "Sáng hôm sau" của Cảnh 4 |

**Veritas.** Hình chiếu bật suốt cả chương, và được bật lại bằng thẻ sau mỗi `[BG]`. Hai ngoại lệ: quãng Kael bị tháo thiết bị ở Cảnh 2 (`bi_giu`), và lối thua của Cảnh 3 từ lúc thiết bị bị tháo. Ở hai quãng ấy cô chỉ có tiếng hoặc im; kịch bản đã ghi thẻ `[SPRITE: Veritas tắt]`.

## 3. Biến

### Biến cũ chương này đọc

`lua_chon_0`, `lua_chon_1`, `lua_chon_2`, `ket_doi_chat_helena`, `giay_to`, `vane_nghe` (để tính `thang_vane`), `loi_vao`, `veritas`, `lua_chon_7`, `lua_chon_8`, `rian_tin`, `vane` (chỉ ở một khối của Cảnh 2), `helena`, `biet_trung`, `biet_hom`, `doi_thiet_bi`, `rian_len`, `vane_len`, `tao_len`; điểm lực lượng `tang_day`, `tang_trung`; và việc đang giữ các ghi chú "Lời bà cụ", "Khóa ngắt dự phòng", "Bà Helena đã chép lời Veritas kể vào sổ riêng của bà", "Tệp ống số 7 và mười hai phiếu y tế cũng về máy 2231".

Chương này **không đổi giá trị** biến cũ nào, trừ việc cộng khuynh hướng và lực lượng ở Lựa chọn 10.

### Biến mới

| Biến | Giá trị | Đặt ở đâu |
|---|---|---|
| `lua_chon_10` | A / B / C | Cảnh 1 |
| `giu_lo`, `giu_thang`, `nha_len` | kip / muon | Ngay sau Lựa chọn 10. **Đúng một biến là kip:** 10-A → `giu_lo`; 10-B → `giu_thang`; 10-C → `nha_len`. Hai biến kia là muon |
| `thang_vane` | co / khong | Suy ra: co nếu `vane` = dong_minh **và** `vane_nghe` = khong (ở Chương 4 `vane` chỉ đổi thành dong_minh khi `vane_nghe` = co). Hoặc chép lại giá trị của `vane` ở cuối Chương 2 |
| `hen_thao` | co / khong | Suy ra, Cảnh 1: co nếu `doi_thiet_bi` = co **và** `vane_len` = khong (đó là khối duy nhất Kael nói câu "Lên tới nơi tôi tháo") |
| `bi_giu` | co / khong | Suy ra, cố định từ đầu Cảnh 2: co nếu `vane_len` = khong **và** `ket_doi_chat_helena` khác thuyet_phuc **và** `nha_len` = muon |
| `co_nguoi_di_cung` | co / khong | Suy ra, Cảnh 2: co nếu một trong `rian_len`, `vane_len`, `tao_len` là co, **hoặc** `ket_doi_chat_helena` = thuyet_phuc, **hoặc** `nha_len` = kip |
| `mat_thiet_bi` | co / khong | Cảnh 2: co khi `bi_giu`. Thiết bị được trả lại ngay trong cảnh; biến chỉ để rẽ vài dòng |
| `doan_mong` | 0, 1, 2 | Cảnh 2. +1 khi thợ xưởng 5 phải ở lại giữ sân thang (Tầng Trung thưa **và** `vane_len` = khong). +1 khi thợ xưởng số 4 phải ở lại đỡ tấm cửa ngăn (không có lính gác của bà Helena) |
| `vao_day` | rian / tao / khong | Cuối Cảnh 2 (bảng dưới) |
| `vao_trung` | co / khong | Cuối Cảnh 2: co nếu `nha_len` = kip |
| `vao_thuoc` | co / khong | Cuối Cảnh 2: co khi và chỉ khi `vao_day` = tao |
| `vung` | 0 đến 10 | Cảnh 3. Người chơi không bao giờ thấy con số này |
| `mat_chot` | 0 đến 3 | Cảnh 3 |
| `kho_goc` | co / khong | Cảnh 3: co khi Câu 4 được gọi đúng ở bản mạnh |
| `sai_cau_5` | co / khong | Suy ra, Cảnh 3: co nếu ở Câu 5 người chơi không bấm "Cái lô chiều nay". **Chỉ dùng trong đoạn "Thắng"** (người thắng dù sai Câu 5: cái máy tự đọc tệp 4416-01 ở đó) |

**Ai lọt vào Lõi cùng Kael (`vao_day`):**

| Điều kiện | `vao_day` | Kèm |
|---|---|---|
| Tầng Đáy đông, `doan_mong` dưới 2, `tao_len` = co | tao | `vao_thuoc` = co |
| Tầng Đáy đông, `doan_mong` dưới 2, `tao_len` = khong | rian | |
| Tầng Đáy thưa, hoặc `doan_mong` = 2 | khong | |

**Khuynh hướng và lực lượng** cộng theo dòng `→` sau mỗi nút của Lựa chọn 10: 10-A `kiem_chung` +1; 10-B `tang_day` +1; 10-C `tang_trung` +1. Vì thế "Tầng Đáy đông", "Tầng Trung đông" ở Cảnh 2 phải tính **sau** khi đã cộng điểm của Lựa chọn 10. Cảnh 2, 3, 4 không cộng khuynh hướng nào.

## 4. Luồng Chương 5

| Bước | Nội dung | Tài nguyên | Sổ tay và biến |
|---|---|---|---|
| **Cảnh 1: Quảng trường tháp** | Trưa Ngày 5. Cái lồng dừng ở Tầng Trung. Bốn đoạn rồi Lựa chọn 10 | BG03_Thap_Nang_Luong_Sup_Do (nền quảng trường như Chương 1). Không nhạc cả cảnh. Helena dùng bốn biểu cảm; Doran chỉ lên ở ba nhánh của lựa chọn | Mảnh 26 (lúc quảng trường im) |
| **Lựa chọn 10** | Ba nút, ai cũng thấy đủ ba, không nút nào là kết cục | | `lua_chon_10`, `giu_lo`, `giu_thang`, `nha_len`, `hen_thao`; khuynh hướng hoặc lực lượng +1 |
| **Cảnh 2: Tầng Đỉnh** | Đầu giờ chiều. Ba chặng, mỗi chặng một nền: sân thang, dãy phòng của Ban (có phòng làm thuốc), trước cửa Lõi. Không có lựa chọn (mục 5) | BG14, BG15, BG16 (chờ vẽ). Không nhạc tới đoạn "Tiếng xích"; BGM04 từ đó tới trước câu "Mời vào"; SE05, SE06 | `bi_giu`, `mat_thiet_bi`, `doan_mong`, `vao_day`, `vao_trung`, `vao_thuoc`. Mảnh 27 |
| **Cảnh 3: A.L.I.C.E** | Chiều Ngày 5, trong Lõi. Đoạn mở, rồi năm câu của trận (mục 6) | BG11 (chờ vẽ). Không nhạc ở đoạn mở; BGM10_ALICE từ Câu 1 tới hết Câu 5. Sprite `ALICE_Neutral` | `vung`, `mat_chot`, `kho_goc`. Mảnh 28, 30, 31, 29 |
| Thắng | Veritas được chép; cái loa đề nghị lính dừng tay; cửa Lõi mở | Không nhạc; SE06 | Sang Cảnh 4 |
| Thua | **Kết cục 5/7 "Lồng kính", lối Chương 5** (mục 8) | BG10, BGM05_Sad_Piano_2 | Nút quay lại về đầu Câu 1 |
| **Cảnh 4: Cái cửa** | Cuối chiều Ngày 5 tới sáng Ngày 6. Năm đoạn, không có lựa chọn (mục 7) | BG16 → BG17 → BG18. Không nhạc tới hết đoạn "Nước"; SE05 lúc bánh xe quay; BGM07 và SE07_Mua ở "Mặt đất"; BGM08 từ "Sáng hôm sau" | Mảnh 32 |
| **Kết cục 7/7 "Bình minh"** | Hình cuối ghép lớp (mục 9), dòng kết cục, bài học, khối "Lối bạn hay chọn" (mục 10), màn kết game | BG18, tám lớp `KT_…`, **`BGM11_Mua_Ve_Tren_Mai`** (bài hát), thay cho BGM08 | Không có nút quay lại |

## 5. Cảnh 1 và Cảnh 2: các chỗ rẽ dễ sót

**Cảnh 1.**

- Bà Helena có mặt ở **mọi** nhánh. Bà làm gì rẽ theo bốn trạng thái: đồng minh với kết Thuyết phục; đồng minh với kết Bất phân (8-A); đứng ngoài; Kael là người của bà. Ở đoạn "Cái lồng lên tiếp", khối "Kael là người của bà Helena" còn rẽ theo 10-B hay không.
- Đoạn "Chín phần" có một chuỗi ba khối anh em `- Nếu 8-B:` / `- Nếu 8-A và kết với bà Helena là Thuyết phục:` / `- Nếu không:`. **Xét từ trên xuống, chạy đúng khối đầu tiên khớp;** `Nếu không` ở đây là "không khớp cả hai khối trên". Quy tắc này dùng cho mọi chuỗi khối cùng mức thụt lề kết bằng `Nếu không`.
- Bà cụ, người dỡ hàng (8-B), cái bánh xe (1-C), mười hai tờ phiếu (2-A / 2-B), Thẻ đặc phái, 7-A, `tao_len`, `vane_len`, `rian_len`: mỗi thứ là một khối nhỏ chen vào mạch chung. Chạy theo thứ tự trên trang.
- Lựa chọn 10: xáo thứ tự nút, không hiện nhãn A, B, C. Sau mỗi nhánh là dòng đặt ba biến `giu_lo`, `giu_thang`, `nha_len`.
- Cả cảnh không có nhạc.

**Cảnh 2.**

- Cảnh mở bằng thẻ `[BG]` ba lần (ba chặng). Sau mỗi lần sân khấu được gỡ sạch; kịch bản đã ghi thẻ bật lại hình chiếu.
- Đoạn "Tiếng xích" có hai khối loại trừ nhau, `**Nếu giu_thang = kip**` và `**Nếu giu_thang = muon**`, rồi tới `**Chung**` (mọi nhánh đều chạy).
- Trong khối `giu_thang = muon`, hai dòng `- Nếu vane = dung_ngoai.` / `- Nếu vane = chu.` chỉ xét khi `vane_len` = khong (Vane đang ở dưới Tầng Đáy).
- Đoạn "Sân thang" có bốn khối: `vane_len`, có lính gác của bà Helena, `nha_len` = kip, Kael bị giữ. Ba khối đầu độc lập, có thể cùng chạy; khối thứ tư chỉ chạy khi cả ba không chạy (đó chính là định nghĩa `bi_giu`).
- Quãng `bi_giu`: thiết bị cổ tay bị tháo, hình chiếu tắt, Veritas chỉ còn tiếng cho tới dòng Kael đeo lại.
- **Ai ở lại chặng nào** (kịch bản tả bằng lời; bảng này để kiểm thử):

| Chốt | Người giữ |
|---|---|
| Sân thang | Tầng Trung đông: người Tầng Trung ở lại (`nha_len` = kip thì có cả mười hai nhà). `vane_len` = co: Vane và hai người đồ đen ở lại. Hai nhóm này độc lập, có thể cùng có. Chỉ khi không có cả hai (Tầng Trung thưa và `vane_len` = khong) thì thợ xưởng 5 ở lại (`doan_mong` +1) |
| Tấm cửa ngăn | Có lính gác của bà Helena: bốn người lính ấy. Không thì thợ xưởng số 4 (`doan_mong` +1) |
| Cái khe giữa hàng khiên | Mẹ Kael và những người thợ còn lại, ở mọi nhánh |

- Cuối cảnh, đoạn "Trước cửa Lõi": đặt `vao_trung`, rồi `vao_day` và `vao_thuoc` theo bảng ở mục 3.

Bảng kiểm thử nhanh cho Cảnh 2:

| Người chơi | `bi_giu` | `doan_mong` | Vào Lõi cùng Kael |
|---|---|---|---|
| Kết Thuyết phục, `tang_day` ≥ 3, `tang_trung` ≥ 1, `tao_len` = co, 10-C | khong | 0 | Người vợ, người thợ bể tảo, người làm thuốc |
| Kết Bất phân, `vane_len` = khong, 10-A, `tang_trung` = 0, `tang_day` ≥ 3 | co | 2 | Không ai (`doan_mong` = 2) |
| Kết Bất phân, `vane_len` = co, 10-B, `tang_trung` = 0, `tang_day` ≥ 3, `tao_len` = khong | khong | 1 | Rian |
| `tang_day` dưới 3, 10-C | khong | 0 hoặc 1 | Chỉ người vợ |

## 6. Cảnh 3: trận "Gọi ai?"

### Luật

- Trận có **năm câu**. Mỗi câu: A.L.I.C.E nói, rồi dòng `→ Gọi ai?`.
- Màn chọn luôn hiện **đúng năm nút, cùng năm cái tên ở cả năm câu:** "Tầng Đáy", "Tầng Trung", "Người ký và người gác", "Veritas", "Cái lô chiều nay".
- **Thứ tự năm nút xáo ngẫu nhiên mỗi lần hiện.** Nút đã bấm ở câu trước không bị ẩn, không bị làm mờ, không được đánh dấu.
- Trên nút chỉ có tên. Lời Kael, hoặc lời người được gọi, chạy sau khi bấm.
- Mỗi câu có một nút đúng:

| Câu | Nút đúng | Bản mạnh (+2) khi | Bản yếu (+1) khi |
|---|---|---|---|
| 1 | Tầng Đáy | `vao_day` = rian; hoặc `vao_day` = tao; hoặc `vao_day` = khong và `giu_thang` = kip | `vao_day` = khong và `giu_thang` = muon |
| 2 | Tầng Trung | `vao_trung` = co; hoặc `giu_lo` = kip; hoặc 2-B | `vao_trung` = khong, `giu_lo` = muon, 2-A |
| 3 | Người ký và người gác | `vao_thuoc` = co; hoặc `vane_len` = co; hoặc `helena` = dong_minh | Không có cả ba |
| 4 | Veritas | `veritas` = dong_hanh (đặt thêm `kho_goc` = co) | `veritas` = do_du |
| 5 | Cái lô chiều nay | `vung` sau bốn câu đầu từ 4 trở lên | `vung` sau bốn câu đầu dưới 4 |

- Dưới mỗi khối `**Gọi "…" (đúng)**` là các khối `- Nếu …` **loại trừ nhau, xét từ trên xuống, chạy khối đầu tiên khớp.** Mỗi khối kết bằng dòng `→ vung +2.` hoặc `→ vung +1.`
- Gọi sai: chạy khối `**Gọi "…" (sai)**` của nút ấy , không cộng `vung`, rồi `→ Chạy "Khi một chốt vỡ"`.
- Sau khối đúng hoặc sai, chạy `**Nhập lại**` của câu ấy, rồi sang câu sau. **Người chơi không được chọn lại:** mỗi câu chỉ bấm một lần.
- Hết Câu 5: `vung` từ 6 trở lên là thắng; dưới 6 là thua.

### Đoạn "Khi một chốt vỡ"

Đoạn `### Khi một chốt vỡ` nằm sau Câu 5 trong file nhưng **không nằm trên đường chạy thẳng**. Nó chỉ chạy khi được gọi:

1. `mat_chot` +1.
2. Chạy khối khớp với giá trị mới của `mat_chot` (1: sân thang; 2: tấm cửa ngăn; 3: cái khe).
3. `mat_chot` = 1 hoặc 2: quay về `**Nhập lại**` của câu đang hỏi.
4. `mat_chot` = 3: **không quay về**; sang thẳng `### Thua`, bỏ qua các câu còn lại.

Khi chạy thẳng hết Câu 5 (không được gọi), game nhảy từ `**Nhập lại**` của Câu 5 tới "Thắng" hoặc "Thua", không đi qua đoạn này.

### Mảnh trong trận

Mảnh 28, 30, 31 mở ở `**Nhập lại**` của Câu 1, 2, 3. Mảnh 29 mở trong lời dẫn của Câu 5, trước `→ Gọi ai?`. Cả bốn mảnh ai cũng nhận, gọi đúng hay sai. Người thua vì `mat_chot` = 3 trước Câu 5 thì chưa có Mảnh 29 (và các mảnh của câu chưa tới); chơi lại thì có.

### Sân khấu của Cảnh 3

A.L.I.C.E lên bằng thẻ `[SPRITE: ALICE_Neutral]`, chiếm một chỗ đứng, ở lại tới hết cảnh, không có biểu cảm nào khác. Trên sân khấu: A.L.I.C.E, Kael, và nhiều nhất một người thứ ba (Rian ở Câu 1, có thẻ gỡ ngay sau; mẹ Kael ở hai đoạn kết). Người thợ bể tảo, người làm thuốc, người nhà là bóng.

### Bảng kiểm thử

| Người chơi | `vung` sau bốn câu | Câu 5 | Tổng | Kết |
|---|---|---|---|---|
| Gọi đúng cả năm, câu nào cũng bản mạnh | 8 | +2 | 10 | Thắng |
| Gọi đúng cả năm, yếu nhất có thể (10-B, `vao_day` = khong, 2-A, không Vane, bà Helena không phải đồng minh, `do_du`): 2 + 1 + 1 + 1 | 5 | +2 | 7 | Thắng. **Gọi đúng cả năm thì đường nào cũng thắng.** Câu 1 và Câu 2 không thể cùng yếu, vì luôn có đúng một trong `giu_lo`, `giu_thang`, `nha_len` là kip |
| Sai Câu 1, bốn câu sau đúng và mạnh | 6 | +2 | 8 | Thắng, `mat_chot` = 1 |
| Sai Câu 1 và 2, Câu 3 và 4 mạnh, Câu 5 đúng | 4 | +2 | 6 | Thắng, `mat_chot` = 2 |
| Sai Câu 1 và 2, Câu 3 mạnh, Câu 4 yếu, Câu 5 đúng | 3 | +1 | 4 | Thua (hết năm câu) |
| Người chơi ở dòng trên, nhưng sai Câu 5 | 5 | 0 | 5 | Thua (hết năm câu) |
| Bốn câu đầu đúng và mạnh, sai Câu 5 | 8 | 0 | 8 | Thắng, `mat_chot` = 1, `sai_cau_5` = co: phải thấy khối "ở Câu 5 đã không gọi…" trong đoạn "Thắng" |
| Sai ba câu bất kỳ | | | | Thua ngay ở lần sai thứ ba |

## 7. Cảnh 4: các chỗ rẽ

Cảnh không có lựa chọn, không có biến mới. Chỉ người thắng trận mới tới đây.

| Đoạn | Chỗ rẽ |
|---|---|
| Mở | Không rẽ. Tướng Ilsa có thẻ lên và thẻ tắt |
| Người tới | Một chuỗi khối chạy theo thứ tự trên trang, mỗi khối độc lập: `vao_day` = tao; `vane_len` = co (rẽ tiếp theo `mat_chot` = 0 hay từ 1); Tầng Trung thưa và `vane_len` = khong (thợ xưởng 5); có người Tầng Trung lên; có / không có lính gác của bà Helena (rẽ theo `mat_chot` dưới 2 hay bằng 2). Rồi chị thợ (mọi nhánh), rồi `tao_len` |
| Cái bánh xe | 1-C hay không (một câu của Kael). Rồi **hai khối loại trừ nhau:** "đã chọn 1-C và có người Tầng Trung lên" (người thợ van và chai dầu) / "không" (thợ già gõ gỉ). Rồi `vane_len`, `tao_len`, `rian_tin` |
| Nước | `veritas` = dong_hanh / do_du (cô hát theo ngay, hay xin phép). `tao_len` (đứa bé). Mảnh 32 mở ở đây |
| Mặt đất | `vane_len` (một dòng tả) |
| Sáng hôm sau | Chuỗi dòng `Dẫn chuyện` chạy theo thứ tự trên trang (bảng dưới) |

**Lưu ý về người thợ van:** khối chai dầu chỉ chạy được khi 1-C **và** 10-C. Điều kiện ghi trong kịch bản là "1-C và có người Tầng Trung lên"; hai điều kiện này cho cùng kết quả, vì 1-B (nguồn duy nhất của `tang_trung` trước Chương 5) loại trừ 1-C, nên với người chọn 1-C thì "có người Tầng Trung lên" chỉ có thể tới từ 10-C. Cứ làm đúng điều kiện trong kịch bản.

**Đoạn "Sáng hôm sau", các dòng dẫn chuyện:**

| Thứ tự | Điều kiện | Số dòng |
|---|---|---|
| 1 | Mọi nhánh | Ba dòng (sọt đá đỏ; cái khuôn ống; mười hai nhà) |
| 2 | 1-C | Một dòng (cái bánh xe dưới lưới) |
| 3 | **Đúng một trong bốn khối** về cái hòm và bà Helena: (a) kết Bị thuyết phục và không 10-B; (b) kết Bị thuyết phục và 10-B; (c) kết Bất phân và 8-B; (d) `helena` = dong_minh | Hai dòng mỗi khối |
| 4 | Mọi nhánh | Một dòng (phòng làm thuốc, tướng Soren) |
| 5 | `vane_len` = co / khong | Một dòng |
| 6 | `giu_lo` = kip / muon | Một dòng |
| 7 | `rian_len` = khong | Một dòng |
| 8 | `tao_len` = co | Một dòng |

Bốn khối ở bước 3 phủ kín mọi người chơi: kết Thuyết phục luôn rơi vào (d); kết Bất phân rơi vào (c) nếu 8-B, vào (d) nếu 8-A.

Sau chuỗi ấy là ba lượt thoại cuối của mẹ, Kael và Veritas, rồi thẻ `[SPRITE: tắt]`.

**Tiếng mưa `SE07_Mua`:** chạy lặp suốt đoạn "Mặt đất", tắt khi sang "Sáng hôm sau". Đây là hiệu ứng chạy lặp đầu tiên của game; nếu engine chỉ có hiệu ứng phát một lần thì coi nó như một kênh nhạc nền thứ hai (mục 14, điểm 5).

## 8. Kết cục 5/7 "Lồng kính", lối Chương 5

- Đến từ lối thua của Cảnh 3 (hết năm câu mà `vung` dưới 6, hoặc `mat_chot` = 3).
- **Cùng một kết cục với lối 9-A ở Chương 4:** đếm là một, đánh dấu "đã xem" theo số kết cục. Đoạn kể và bài học của lối này là riêng ("Được bảy phần là được việc rồi"); khi người chơi mở lại kết cục trong bộ sưu tập thì hiện lối họ đã đi (mục 14, điểm 2).
- Đoạn "Thua" rẽ theo: `mat_chot` dưới 3 hay bằng 3; `vao_day` (tao / rian); `kho_goc`. Chạy theo thứ tự trên trang.
- Nền BG10 và BGM05_Sad_Piano_2 từ thẻ trong kịch bản. Hình chiếu Veritas tắt từ lúc thiết bị bị tháo.
- **Nút quay lại đưa về đầu Câu 1** (không về đầu cảnh, không về Lựa chọn 10): đặt lại `vung` = 0, `mat_chot` = 0, `kho_goc` = khong, `sai_cau_5` = khong; giữ nguyên mọi biến mang vào cảnh. Các mảnh đã mở giữ nguyên.
- Sau bài học là khối "Lối bạn hay chọn" (mục 10).

## 9. Kết cục 7/7 "Bình minh" và hình cuối

Sau câu "Tôi quay mặt đi nhé." và thẻ `[SPRITE: tắt]`:

1. Ẩn ô thoại và mọi sprite, kể cả hình chiếu Veritas.
2. Trên nền `BG18_Mat_Dat_Binh_Minh`, ghép hình cuối từ các lớp `KT_…`. Mọi người trong hình đều quay lưng về phía người chơi, nhìn về mặt trời.
3. Cùng lúc hình hiện lên, đổi nhạc sang **`BGM11_Mua_Ve_Tren_Mai`** (thẻ `[BGM]` nằm ngay sau `[SPRITE: tắt]`): đây là bản thu bài hát "Mưa về trên mái". Giữ hình vài giây. Bài hát chạy tiếp qua mọi màn sau đó tới hết màn kết game; **không lặp**, hết bài thì im.
4. Hiện dòng `KẾT CỤC 7/7: BÌNH MINH`, khối bài học, khối "Lối bạn hay chọn", rồi màn kết game.

| Lớp | Hiện khi | Là ai |
|---|---|---|
| `KT_Kael_Me` | Luôn luôn. **Đặt chính giữa**, trước mặt trời | Kael và mẹ ôm nhau; trên cổ tay Kael có một đốm sáng xanh (Veritas) |
| `KT_Rian` | Luôn luôn | Rian, cây búa trên vai |
| `KT_Tho_xuong_4` | Luôn luôn | Thợ già, chị thợ, cậu thợ học việc |
| `KT_Vane` | `vane_len` = co | Vane |
| `KT_Helena` | `helena` = dong_minh | Bà Helena, đứng cách ra một quãng |
| `KT_Tho_be_tao` | `tao_len` = co | Người thợ bể tảo bế con |
| `KT_Tang_Trung` | Có người Tầng Trung lên (Tầng Trung đông, hoặc `nha_len` = kip) | Người thợ van, người vợ, người dỡ hàng |
| `KT_Doran` | `giu_lo` = kip | Doran |

- Ít nhất ba lớp, nhiều nhất tám lớp. Năm lớp có điều kiện độc lập với nhau: `KT_Tang_Trung` và `KT_Doran` có thể cùng thiếu (10-B với `tang_trung` = 0), cũng có thể cùng có (10-A với `tang_trung` từ 1 trở lên). Đừng giả định số lớp.
- Xếp: `KT_Kael_Me` ở giữa; các lớp còn lại xen đều hai bên, chân đặt trên cùng một đường ngang. Người thân với Kael đứng gần hơn: đề nghị thứ tự từ trong ra ngoài là `KT_Rian`, `KT_Tho_xuong_4`, `KT_Tho_be_tao`, `KT_Vane`, `KT_Tang_Trung`, `KT_Doran`, và `KT_Helena` luôn ở ngoài cùng một bên (mục 14, điểm 3).
- Tám lớp đã có file, nằm trong thư mục `Hinh_cuoi/` của gói bàn giao (người viết làm, 08/10/2026).
- **Kết cục 7/7 không có nút quay lại điểm chọn.**

## 10. Khối "Lối bạn hay chọn" (áp dụng cho cả bảy kết cục)

Người viết duyệt 08/10/2026. Đây là phần game tự tính; **kịch bản các kết cục không đổi chữ nào.**

- **Hiện ở đâu:** ở màn kết cục của **cả bảy kết cục**, ngay sau khối "Bài học kết cục". Áp dụng ngược cho các kết cục đã làm: 1/7 (Chương 1), 3/7 (Chương 2 và Chương 3), 2/7 và 6/7 (Chương 3), 4/7 và 5/7 (Chương 4).
- **Tính:** lấy khuynh hướng có điểm cao nhất của cả lượt chơi tính tới lúc kết cục (`lam_dung_lenh`, `long_tin`, `con_so`, `ngon_lua`, `kiem_chung`). Hiện **đoạn chính** của nó.
- **Câu ghép:** nếu khuynh hướng đứng thứ hai có từ 2 điểm trở lên thì hiện thêm một câu ghép theo cặp hai khuynh hướng cao nhất (mười cặp, không tính thứ tự). Không thì chỉ hiện đoạn chính.
- **Khi hòa điểm:** trong các khuynh hướng hòa nhau, lấy cái được cộng gần đây nhất. Vì thế game cần nhớ thứ tự cộng điểm.
- Tiêu đề khối: "Lối bạn hay chọn". Không hiện tên khuynh hướng, không hiện điểm.
- Lượt chơi mà mọi khuynh hướng đều 0 điểm thì không hiện khối này. Trên thực tế không xảy ra, vì cả ba nút của Lựa chọn 0 đều cộng một điểm khuynh hướng (mục 14, điểm 4).

**Năm đoạn chính** (chép nguyên văn; chữ in đậm đầu đoạn là tên khuynh hướng để dev tra, không hiện):

> **Làm đúng lệnh.** Trên chặng đường này, bạn hay chọn làm cho xong việc được giao và để phần còn lại cho người có trách nhiệm. Việc được giao thường đúng. Chỗ hỏng là khi không ai còn hỏi việc ấy đang giữ cho cái gì đứng.
>
> **Lòng tin.** Trên chặng đường này, bạn hay chọn giữ cho người ta yên lòng trước đã. Lòng người yên thì ca vẫn chạy. Nhưng cái ống không đọc được lòng người: nó mòn theo cách của nó.
>
> **Con số.** Trên chặng đường này, bạn hay chọn theo phép tính: đủ hay thiếu, được hay mất bao nhiêu. Con số không nói dối. Nó chỉ không nói ai đã quyết phần nào về tay ai.
>
> **Ngọn lửa.** Trên chặng đường này, bạn hay chọn đứng về phía người đang chịu thiệt, và làm ngay. Không có cái nóng ấy thì không ai dừng tay. Nhưng dừng tay mới là nửa đầu; nửa sau là dựng cái gì vào chỗ ấy.
>
> **Kiểm chứng.** Trên chặng đường này, bạn hay chọn tự đi xem, tự đo, rồi mới nói. Điều bạn nói vì thế có thứ chống lưng. Nhưng biết đúng mới là một nửa; nửa kia là có bao nhiêu người cùng làm.

**Mười câu ghép:**

| Cặp | Câu ghép |
|---|---|
| Làm đúng lệnh + Lòng tin | Bạn làm đúng việc, và tin người giao việc. Hai thứ ấy đỡ nhau rất êm, nên cũng khó thấy nhất lúc cả hai cùng sai. |
| Làm đúng lệnh + Con số | Bạn làm đúng việc và tính đủ số: một ca làm không ai chê được. Câu còn thiếu là việc ấy, con số ấy, do ai đặt ra. |
| Làm đúng lệnh + Ngọn lửa | Có lúc bạn làm theo, có lúc bạn gạt phăng. Đáng hỏi lại: ở chỗ nào thì bạn đổi, và cái gì làm bạn đổi. |
| Làm đúng lệnh + Kiểm chứng | Bạn làm việc được giao, nhưng có đi xem trước. Từ chỗ ấy tới chỗ hỏi lại chính cái lệnh chỉ còn một bước. |
| Lòng tin + Con số | Bạn giữ lòng người và giữ sổ sách: một thành phố được giữ yên bằng đúng hai thứ ấy. Yên cho ai thì chưa có trong thứ nào. |
| Lòng tin + Ngọn lửa | Bạn tin người, và nóng thay cho người. Cả hai đều bắt đầu từ tấm lòng; thứ cần thêm là một cái gì đo được. |
| Lòng tin + Kiểm chứng | Bạn muốn người ta yên lòng, mà cũng muốn biết thật. Chỗ khó là lúc hai điều ấy không đi cùng nhau được, và bạn đã phải chọn. |
| Con số + Ngọn lửa | Bạn tính ra ai đang thiệt, rồi đứng về phía họ. Phép tính cho biết đập vào đâu; nó chưa cho biết dựng lại bằng gì. |
| Con số + Kiểm chứng | Bạn tin thứ đếm được, và tự đi đếm. Con số bạn có là số thật; việc còn lại là hỏi nó từ tay ai mà ra. |
| Ngọn lửa + Kiểm chứng | Bạn biết đúng, và không ngồi yên. Chỉ còn thiếu một thứ: đủ người cùng làm. |

Hai điểm lực lượng (`tang_day`, `tang_trung`) không tham gia khối này.

## 11. Vật phẩm và ghi chú của Chương 5

**Chương 5 không thêm vật phẩm hay ghi chú nào vào sổ tay,** và không gỡ thứ nào. Các ghi chú cũ được đọc ở chương này: "Lời bà cụ" (Cảnh 1), "Khóa ngắt dự phòng" (Cảnh 1), "Bà Helena đã chép lời Veritas kể vào sổ riêng của bà" (Cảnh 3, Câu 4), "Tệp ống số 7 và mười hai phiếu y tế cũng về máy 2231" (Cảnh 3, sau Câu 4).

Trận cuối **không có màn chọn ghi chú**: người chơi không mở sổ tay để đáp.

## 12. Mảnh lưu trữ

| Mảnh | Tên | Ai nhận | Mở lúc nào |
|---|---|---|---|
| 26 | "Bốn tháng và một buổi sáng" | Mọi người | Cảnh 1, lúc quảng trường im |
| 27 | "Cái máy tính tiền" | Mọi người | Cảnh 2, sau câu "Thế thì nó cũng không được làm ra để sửa" |
| 28 | "Người ngồi ghế bên" | Mọi người | Cảnh 3, "Nhập lại" của Câu 1 |
| 30 | "Lớp học khá" | Mọi người | Cảnh 3, "Nhập lại" của Câu 2 |
| 31 | "Cả chuyền dừng máy" | Ai chưa thua khi hết Câu 3 | Cảnh 3, "Nhập lại" của Câu 3 |
| 29 | "Cầu yếu, đừng đi xe qua" | Ai tới Câu 5 | Cảnh 3, lời dẫn Câu 5, trước khi chọn |
| 32 | "Nước ngoài hiên" | Ai thắng trận | Cảnh 4, đoạn "Nước" |

- Thứ tự mở trong trận là 28, 30, 31, 29: số mảnh không theo thứ tự câu. Trong sổ tay vẫn xếp theo số.
- Một người sai cả ba câu đầu sẽ thua ngay ở Câu 3 và mới có Mảnh 28, 30 (khối "Nhập lại" của Câu 3 không chạy khi `mat_chot` = 3). Quay lại đầu trận thì nhận nốt.
- Không mảnh nào có số liệu động. Nội dung chép nguyên văn từ các khối `>` ở cuối file.
- **Cả game có 32 mảnh** (01 đến 32) và 7 kết cục. "Chơi hết" là đủ 32 mảnh và 7 kết cục.

## 13. Tài nguyên dùng trong Chương 5

| Loại | Tên file | Ghi chú |
|---|---|---|
| Nền đã có | BG03_Thap_Nang_Luong_Sup_Do, BG10_Tang_Day_No_Du_Nhung_U_Toi | |
| **Nền mới, chờ vẽ** | BG11_Loi_ALICE_Ruc_Sang, BG14_San_Thang_Tang_Dinh, BG15_Day_Phong_Ban_Co_Van, BG16_Truoc_Cua_Loi, BG17_Mat_Dat_Mua, BG18_Mat_Dat_Binh_Minh | BG17 và BG18 cùng một góc nhìn, khác thời tiết và ánh sáng. Khi chưa có file: chưa có tấm nào thì giữ nền đang hiện. Mô tả từng tấm: `TAI_NGUYEN_CON_THIEU.md` |
| Nhạc đã có | BGM04_Fast_Paced_Action, BGM05_Sad_Piano_2, BGM07_Bittersweet_Ambient, BGM08_Epic_Symphony | BGM04 và BGM08 lần đầu dùng |
| **Nhạc mới, đã có file** | BGM11_Mua_Ve_Tren_Mai | Bản thu bài hát "Mưa về trên mái". File `BGM11_Mua_Ve_Tren_Mai.mp3` nằm ngay trong gói bàn giao (bản gốc người viết gửi: `Mưa Về Trên Mái (Instrumental Interludes).mp3`, cùng nội dung). Phát từ hình cuối của Kết cục 7/7 |
| **Nhạc mới, chưa có file** | BGM10_ALICE | Đều, sạch, không gấp, như nhạc chờ. Chạy từ Câu 1 tới hết Câu 5 |
| Hiệu ứng | SE05_Xa_Van_Khi_Kim_Loai, SE06_Cua_Thep_Mo_Ra | |
| **Hiệu ứng mới, chưa có file** | SE07_Mua | Tiếng mưa đều ngoài trời, chạy lặp |
| Sprite đã có | Kael, Veritas_Hologram, Veritas_Serious, Veritas_Smile, Helena_Neutral, Helena_Smile, Helena_Surprised, Helena_Thoughtful, Vane_Neutral, Rian_Neutral, Rian_Serious, Rian_Angry, Me_Neutral, Me_LookAway, Doran_Neutral | |
| **Sprite mới, chờ vẽ** | Soren_Neutral, Ilsa_Neutral, ALICE_Neutral | Khi chưa có file: chỉ hiện ô tên, sân khấu giữ nguyên |
| **Bóng mới, chờ vẽ** | Bong_Tho_van, Bong_Co_van, Bong_Nguoi_lam_thuoc, Bong_Linh | Khi chưa có file: chỉ hiện ô tên |
| **Lớp hình cuối, đã có file (thư mục `Hinh_cuoi/`)** | KT_Kael_Me, KT_Rian, KT_Tho_xuong_4, KT_Vane, KT_Helena, KT_Tho_be_tao, KT_Tang_Trung, KT_Doran | Nền trong suốt, người nhìn từ sau lưng (mục 9) |

- Cả Cảnh 1, đoạn mở Cảnh 2 và Cảnh 3, lối thắng của Cảnh 3, và Cảnh 4 từ đầu tới hết đoạn "Nước" **không có nhạc**: đừng tự thêm nhạc nền.
- Sân khấu của từng cảnh, kể cả các chỗ game phải tự xoay vòng, ghi trong `(Ghi chú cho người làm game: …)` cuối mỗi cảnh.

## 14. Các điểm kịch bản không nói rõ

Các điểm dưới đây **người viết đã chốt 08/10/2026** (cột "Đề nghị" là điều đã chốt). Các mục phía trên viết theo đúng chúng.

| # | Điểm | Đề nghị |
|---|---|---|
| 1 | Nút quay lại của Kết cục 5/7 lối Chương 5 quay về đâu, giữ gì | Về đầu Câu 1; đặt lại bốn biến của trận; giữ mảnh đã mở (mục 8) |
| 2 | Kết cục 5/7 có hai lối, hai bài học: trong bộ sưu tập kết cục hiện bài học nào | Hiện lối người chơi đã đi; đã đi cả hai thì cho xem cả hai |
| 3 | Thứ tự xếp các lớp hình cuối | Như mục 9: `KT_Kael_Me` giữa, người thân đứng gần, `KT_Helena` ngoài cùng |
| 4 | Khối "Lối bạn hay chọn" khi mọi khuynh hướng đều 0 | Không hiện khối |
| 5 | `SE07_Mua` chạy lặp | Coi như kênh nhạc nền thứ hai nếu engine không lặp được hiệu ứng |
| 6 | Bài hát ở Cảnh 4 (bốn khổ) có tiếng hát hay chỉ có chữ | Ở đoạn "Nước" và "Mặt đất": chỉ có chữ, hiện như thoại. Bản thu `BGM11_Mua_Ve_Tren_Mai` phát từ hình cuối tới hết màn kết game, thay cho BGM08 (người viết chốt 08/10/2026) |
| 7 | Có gắn mỗi đoạn "Lối bạn hay chọn" với một mảnh "nên đọc lại" không | Không gắn |
| 8 | Biến suy ra `thang_vane`, `hen_thao`, `bi_giu`, `co_nguoi_di_cung`, `sai_cau_5` | Thêm như mục 3; kịch bản không gọi tên chúng |
| 9 | Sau Kết cục 7/7 có gì | Màn kết game thường dùng (danh đề); không thêm đoạn kể nào |
| 10 | Người thua ở lần sai thứ ba không nhận mảnh của câu đang hỏi | Đúng như kịch bản: khối "Nhập lại" không chạy; chơi lại thì nhận |

Gặp điểm nào khác mà kịch bản và tài liệu này đều không nói: hỏi người viết, đừng tự quyết.

## 15. Không tự ý đổi

Như các tài liệu trước, thêm:

- **Không gợi ý nút đúng trong trận cuối:** không tô màu, không làm mờ nút đã bấm, không sắp nút đúng ở vị trí cố định, không hiện "đúng", "sai" sau khi bấm. Người chơi biết mình gọi sai nhờ tiếng động từ ngoài cửa (đoạn "Khi một chốt vỡ").
- Không hiện `vung`, `mat_chot` dưới bất kỳ dạng nào (số, thanh đo, biểu tượng ba chốt).
- Không cho chọn lại trong một câu. Không thêm nút "nghe theo nó" hay nút bỏ trận.
- A.L.I.C.E không có biểu cảm thứ hai: nụ cười của nó không đổi, kể cả khi nó đổi ý. Không thêm hiệu ứng rung, méo tiếng, đổi màu cho lời nó.
- Không ai trong truyện nhận xét A.L.I.C.E giống Veritas; game cũng không chú thích điều ấy.
- Trong Chương 5, không hiện chữ "mưa" ở đâu trước dòng hát của mẹ ở Cảnh 4 (kể cả trong tên file hiển thị, phụ đề hiệu ứng, tên nền).
- Không báo trước ai sẽ có trong hình cuối, và không chú thích tên người trên hình cuối.
- Con số trên bản tin sáng Ngày 6 là **73.001**, và câu ấy không còn chữ "ổn định". Mọi bản tin trước đó vẫn là "ngày ổn định thứ 73.000".
- Các con số khác: lô 4.416, tệp 4416-01; ba mươi sáu lượt hỏi; mười hai nhà, mười hai tờ phiếu; hai mươi bốn bể, mười chín bể chạy; hai chục máy ở xưởng lưu trữ; mười bốn lính ở vạch vàng; năm 2150; giáp VOLKOV 02 (Soren), VOLKOV 03 (Ilsa).
