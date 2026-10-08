# Hướng dẫn cho dev: Mở đầu và Chương 1

> **Cập nhật 05/10/2026:** cách hiện sprite và các chỗ tắt nhạc đã đổi (sân khấu tối đa ba người, thẻ `[SPRITE: … tắt]`, mọi chỗ tắt nhạc đều có thẻ). Đọc `HUONG_DAN_DEV_SAN_KHAU.md`; chỗ nào file này nói khác thì theo file đó (bảng đối chiếu ở mục 5 của nó).

Người viết kịch bản duyệt ngày 03/10/2026.

Tài liệu này nói cách biến hai file kịch bản thành game: dòng nào hiện lên màn hình, dòng nào là lệnh, cần lưu biến gì, tính điểm thế nào. Nó không thay kịch bản. Khi tài liệu này và kịch bản lệch nhau thì kịch bản đúng.

## 1. Gói bàn giao

| File | Nội dung |
|---|---|
| `00_mo_dau.md` | Mở đầu: "Tệp không chịu xóa". 3 cảnh, Lựa chọn 0, Mảnh 01 |
| `01_chuong_1.md` | Chương 1: "Lời bà Helena". 3 cảnh, Lựa chọn 1 và 2, trận đối chất, 4 cách kết, Mảnh 02 đến 06, 1 bài học kết cục |
| `Kich_ban_Mo_dau_va_Chuong_1.docx` | Hai file trên gộp lại để đọc và in. Chữ giống hệt bản `.md` |

**Cập nhật 04/10/2026** (`01_chuong_1.md` và file `.docx` đã xuất lại): hai thẻ nhạc BGM05 đổi tên (mục 6, 8, 10); trong trận đối chất, trên nút chỉ còn phần nêu ý của mỗi đáp án (mục 7). Mở đầu không đổi.

Lời thoại đã chốt. Dev không sửa chữ, không đổi thứ tự câu, không gộp hay tách lượt thoại. Thấy chỗ nào vô lý hoặc không làm được thì báo lại người viết.

Chương 2 đã giao ngày 04/10/2026, hướng dẫn riêng ở `HUONG_DAN_DEV_CHUONG_2.md`. Chương 3 đến 5 chưa giao. Mục 11 liệt kê dữ liệu phải lưu để chương sau dùng.

## 2. Cách đọc kịch bản

| Trong kịch bản | Ví dụ | Xử lý |
|---|---|---|
| `[BG: tên]` | `[BG: BG02_Phong_Luu_Tru_Du_Lieu]` | Đổi ảnh nền |
| `[BGM: tên]` | `[BGM: BGM05_Sad_Piano_1]` | Đổi nhạc nền, lặp cho tới thẻ `[BGM]` kế tiếp |
| `[SE: tên]` | `[SE: SE04_Tieng_No_Thap_Nang_Luong]` | Phát hiệu ứng một lần |
| `[SPRITE: tên]` | `[SPRITE: Kael_Surprised]` | Đổi sprite đang hiện |
| `[Helena_Smile]` đứng đầu một dòng trả lời trong trận đối chất | `- [Helena_Smile] Helena: "Cậu hiểu nhanh hơn tôi tưởng…"` | Cũng là đổi sprite, chỉ viết tắt |
| `Dẫn chuyện: …` | | Hiện trong khung thoại, ô tên để trống hoặc ghi "Dẫn chuyện" |
| `(…)` đứng riêng một dòng | `(Kael cất thẻ vào ví.)` | Hiện thành chữ trong khung thoại, **không có ô tên**, in nghiêng |
| `Tên: "…"` | `Doran: "Không. Mất suất thì có."` | Lời thoại, ô tên hiện tên |
| `Kael (nghĩ): …` | | Suy nghĩ. Ô tên "Kael", chữ nhạt hoặc nghiêng để khác lời thoại |
| `A.L.I.C.E (loa): "…"` | | Lời qua loa. Ô tên "A.L.I.C.E". Không có sprite |
| `???: "…"` | | Lời thoại, ô tên hiện "???" |
| `(Từ đây ô tên hiện "X".)` | | **Lệnh, không hiện chữ.** Từ câu sau, nhân vật đó hiện tên thật |
| `[VẬT PHẨM MỚI: tên]` | | Thêm vào ngăn Vật phẩm, biểu tượng cổ tay nháy. Mô tả là các dòng `>` ngay bên dưới |
| `[GHI CHÚ MỚI: tên]` | | Thêm vào ngăn Ghi chú, biểu tượng cổ tay nháy. Tên ghi chú chính là nội dung, không có mô tả riêng |
| `→ Mở Mảnh NN.` | | Thêm mảnh vào ngăn Mảnh lưu trữ, biểu tượng cổ tay nháy |
| Dòng `- Kael: "…"` ngay dưới một đáp án của trận đối chất | | Phần Kael **nói tiếp sau khi người chơi đã chọn**. Không nằm trên nút (thêm 04/10/2026, xem mục 7) |
| `- [0-A] "…" → Làm đúng lệnh +1` | | Một nút lựa chọn. Chữ trên nút là phần trong ngoặc kép. Phần sau `→` là biến, **không hiện** |
| Dòng bắt đầu bằng `→` | `→ Lung lay +2. Lập trường: duy tâm.` | **Không hiện.** Cộng biến theo đúng nội dung |
| `[GIEO: …]` | | **Không hiện.** Ghi chú của người viết về chi tiết cài trước |
| `(Ghi chú cho người làm game: …)` | | **Không hiện.** Chỉ dẫn cho dev |
| `**Nhánh 0-A**`, `**Nếu đã chọn 2-A (…)**`, `Nếu đã chọn 0-B (…)`, `- Nếu 0-A. Helena: "…"` | | **Không hiện.** Điều kiện rẽ: chỉ chạy đoạn bên dưới khi biến khớp |
| `**Nhập lại (cả ba nhánh)**` | | **Không hiện.** Các nhánh chạy chung từ đây |
| `> …` | | Chữ trong sổ tay: mô tả vật phẩm, nội dung mảnh, bài học kết cục |

**Ba dòng trong kịch bản là ghi chú soạn thảo, không phải nội dung game, bỏ qua:**

- `00_mo_dau.md`: dòng "Bản 3. Cảnh 1 giữ nguyên. Cảnh 2 và 3 viết lại…" ngay dưới tiêu đề.
- `01_chuong_1.md`: dòng "Bản 2, viết lại theo các quy tắc đã chốt ở Mở đầu." ngay dưới tiêu đề.
- `01_chuong_1.md`, đầu Cảnh 3: dòng "Bản 3, viết lại toàn bộ. Mỗi câu hỏi là một vấn đề triết học…".

**Sửa 08/10/2026:** lời bài hát đã chốt, nên chú thích `(lời hát tạm, sẽ thay khi có bài hát thật)` sau câu hát ở Mở đầu Cảnh 3 đã bỏ khỏi kịch bản. Câu hát không đổi chữ nào. `00_mo_dau.md` và `Kich_ban_Mo_dau_va_Chuong_1.docx` đã xuất lại; nếu code có chỗ lọc chú thích ấy thì gỡ đi.

## 3. Giao diện cần có

**Thiết bị cổ tay.** Một biểu tượng ở góc màn hình, không phải sprite. Bấm vào mở sổ tay có ba ngăn:

| Ngăn | Chứa gì | Mỗi mục có |
|---|---|---|
| Vật phẩm | Đồ Kael đang giữ | Tên và mô tả (mô tả có thể chứa ký ức của Kael) |
| Ghi chú | Điều Kael đã biết | Chỉ có tên |
| Mảnh lưu trữ | Kiến thức triết học | Tiêu đề và nội dung; có bộ đếm kiểu "4/…" (tổng số sẽ chốt khi xong cả game) |

- Khi có vật phẩm, ghi chú hoặc mảnh mới: biểu tượng nháy và hiện một dòng thông báo ngắn. **Không tự bật sổ tay**, không ngắt truyện.
- Dòng thông báo ghi: "Vật phẩm mới: [tên]", "Ghi chú mới: [tên]", "Mảnh lưu trữ mới". Riêng mảnh không ghi tên, để người chơi tự mở ra xem.
- Ở nhánh 0-A và 0-C của Mở đầu có dòng "(Biểu tượng cổ tay ở góc màn hình nháy xanh ngọc một lần…)". Đây là tình tiết truyện (có thứ vừa nhảy vào thiết bị), không phải thông báo mục mới. Nên dùng màu xanh ngọc riêng, khác màu thông báo thường.

**Ô tên.** Hiện "???" cho tới khi kịch bản có dòng `(Từ đây ô tên hiện "X".)`. Áp dụng cho Doran (Mở đầu, Cảnh 2) và Veritas. Vai phụ ghi thẳng tên vai (Công nhân, Bà cụ, Người trong đám đông).

**Sprite.** Mỗi thẻ `[SPRITE]` thay sprite đang hiện; kịch bản không dựng hai sprite cạnh nhau. Doran, Công nhân, Bà cụ, A.L.I.C.E không có sprite, chỉ có ô tên. **(Câu này hết hiệu lực từ 06/10/2026: mẹ Kael và Doran có sprite, sáu người phụ có bóng. Xem `HUONG_DAN_DEV_SAN_KHAU.md` mục 2.1 và 2.11.)**

**Trận đối chất (Chương 1, Cảnh 3).** Hai thanh hiện lên từ dòng "(Hai thanh hiện lên…)" cho tới hết Câu 4:

- **Lung lay** của Helena, từ 0 đến 8, ngưỡng 6.
- **Dao động** của Kael, từ 0 đến 8, ngưỡng 5.
- Hai thanh chỉ hiện mức đầy dần, **không hiện vạch ngưỡng**.
- Lung lay không xuống dưới 0: có một đáp án trừ 1, nếu thanh đang ở 0 thì giữ nguyên 0.

**Kết cục.** Màn hình tên kết cục ("KẾT CỤC 1/7: CA LÀM HOÀN HẢO"), ngay sau đó là khối "Bài học kết cục", rồi nút quay lại. Nút cho người chơi chọn một trong ba điểm để chơi lại: Lựa chọn 0, Lựa chọn 1, Lựa chọn 2.

**Các nút thông thường của visual novel:** Log, Auto, Skip, lưu và tải.

## 4. Biến cần lưu

Tên biến là đề xuất; dev đặt lại theo quy ước của mình cũng được, miễn đủ nghĩa.

| Biến | Giá trị | Đặt ở đâu |
|---|---|---|
| `lua_chon_0` | A / B / C | Mở đầu, Lựa chọn 0 |
| `lua_chon_1` | A / B / C | Chương 1, Lựa chọn 1 |
| `lua_chon_2` | A / B | Chương 1, Lựa chọn 2 |
| `ket_doi_chat_helena` | thuyet_phuc / bat_phan / bi_thuyet_phuc / ket_cuc_som | Cuối Chương 1 |
| Khuynh hướng: `lam_dung_lenh`, `long_tin`, `con_so`, `ngon_lua`, `kiem_chung` | Số nguyên, bắt đầu từ 0 | Cộng dồn qua cả game. **Người chơi không bao giờ thấy.** `con_so` và `ngon_lua` chưa dùng trong hai chương này nhưng phải có sẵn |
| Lực lượng: `tang_trung`, `tang_day` | Số nguyên, bắt đầu từ 0 | `tang_day` chưa dùng trong hai chương này |
| `lung_lay`, `dao_dong` | Số nguyên | Chỉ dùng trong trận đối chất |
| Bộ đếm lập trường: `duy_tam`, `duy_vat_may_moc`, `bat_kha_tri`, `sieu_hinh`, `duy_vat_bien_chung` | Số nguyên | Trận đối chất; dùng cho Mảnh 06 |
| Vật phẩm, ghi chú, mảnh | Danh sách mục đã có | Xem mục 5, 6, 9 |

## 5. Luồng Mở đầu

| Bước | Nội dung | Tài nguyên | Sổ tay |
|---|---|---|---|
| Cảnh 1: Thành phố | Lời dẫn về Aethelgard, bản tin sáng qua loa | BG01, BGM01, SE01 | |
| Cảnh 2: Cổng xưởng | Kael quẹt thẻ, nói chuyện với Doran | Kael_Neutral | Vật phẩm "Thẻ cũ của mẹ" (có mô tả hai đoạn) |
| Cảnh 3: Xưởng lưu trữ | Tệp không xóa được, tự mở | BG02, SE02, SE03, Kael_Surprised | Ghi chú "Bảng số ống 7" |
| **Lựa chọn 0** | Ba nút | | |

| Lựa chọn 0 | Biến | Điều riêng của nhánh |
|---|---|---|
| 0-A: xóa cưỡng chế | `lam_dung_lenh` +1 | Biểu tượng cổ tay nháy xanh ngọc |
| 0-B: chuyển sang thiết bị cổ tay | `kiem_chung` +1 | Sprite Veritas_Glitch. "???" đổi thành "Veritas" ngay trong nhánh này |
| 0-C: báo lên Ban Cố vấn | `long_tin` +1 | Ghi chú "Phản hồi ký tên "H."". Biểu tượng cổ tay nháy xanh ngọc |

Sau cả ba nhánh: mở **Mảnh 01**. Hết Mở đầu.

## 6. Luồng Chương 1

**Cảnh 1: Giấy gọi** (BG01, BGM01, Kael_Surprised). Ghi chú "Giấy gọi 18 giờ". Có hai chỗ đổi theo `lua_chon_0`:

- Dòng "Về việc" trên giấy gọi: ba câu khác nhau, ghi trong chỉ dẫn cho dev ngay dưới ghi chú.
- Đoạn nói chuyện với Veritas:
  - 0-B: đoạn ngắn, sprite Veritas_Hologram, ô tên đã là "Veritas".
  - 0-A hoặc 0-C: đoạn dài, sprite Veritas_Glitch, ô tên bắt đầu là "???" rồi đổi thành "Veritas".

**Cảnh 2: Quảng trường tháp** (BG03, BGM02, SE01, Helena_Neutral, rồi Kael_Neutral). Mở **Mảnh 02** ngay sau dòng "(Im lặng một lúc. Người thợ van nhìn mấy người lính, rồi gật đầu, quay đi trước. Đám đông tan dần theo anh ta.)"; kịch bản không có dòng mở mảnh ở chỗ này nên dev tự đặt. Cảnh chạy tiếp tới **Lựa chọn 1**:

| Lựa chọn 1 | Biến | Tài nguyên | Sổ tay |
|---|---|---|---|
| 1-A: đi làm | `lam_dung_lenh` +1 | BG02, SE04, Kael_Trando | Mảnh 04. Không có ghi chú |
| 1-B: nán lại | `tang_trung` +1 | SE04, Kael_Surprised (nền vẫn là BG03) | Ghi chú "Lời bà cụ: "lần trước cũng thế này""; Mảnh 05 |
| 1-C: đo ống | `kiem_chung` +1 | SE04, Kael_Surprised (nền vẫn là BG03) | Ghi chú "Số đo ống 7: 0,9 mm"; ghi chú "Khóa ngắt dự phòng dưới lưới sắt, gỉ kẹt"; Mảnh 03 |

Ở 1-B và 1-C kịch bản có câu "máy quẹt thẻ trừ nửa suất". Đó là lời kể, **không có biến suất**.

**Nhập lại** (BG02, BGM05_Sad_Piano_1, Kael_Trando), rồi **Lựa chọn 2**:

| Lựa chọn 2 | Biến | Sổ tay |
|---|---|---|
| 2-A: Xóa | `lam_dung_lenh` +1 | |
| 2-B: Không xóa | `kiem_chung` +1 | Ghi chú "Mười hai tệp chưa xóa trong lô 4.413" |

**Cảnh 3: Đối chất với Helena** (BG04, BGM02, Helena_Neutral). Câu mở của Helena đổi theo `lua_chon_0` (ba dòng "Nếu 0-A / 0-B / 0-C"). Sau đó là bốn câu hỏi, rồi một trong bốn cách kết.

## 7. Trận đối chất: luật và bảng điểm

- Bốn câu hỏi nối tiếp. Mỗi câu Helena nói vài lượt rồi hiện ba đáp án. Chọn xong thì chạy mấy dòng phản ứng nằm ngay dưới đáp án đó, rồi sang câu sau.
- **Trên nút chỉ có phần nêu ý (sửa 04/10/2026).** Trước đây cả câu đáp nằm trên nút, đáp án cho điểm dài hơn hẳn nên người chơi nhìn độ dài là đoán ra. Nay chữ trên nút chỉ là phần trong ngoặc kép ngay sau nhãn đáp án (một hoặc hai câu, đủ để người chơi hiểu lựa chọn ấy muốn nói gì); các câu dẫn chứng đã được cắt xuống thành những dòng `- Kael: "…"` ngay bên dưới. Chọn xong: hiện câu trên nút thành một lượt thoại của Kael, chạy tiếp các dòng `- Kael:` như lượt thoại thường, rồi tới phản ứng của Helena. Chữ không đổi so với bản đã giao, chỉ đổi chỗ đặt (riêng đáp án "Lời bà cụ" ở Câu 3 đưa câu nêu ý lên đầu). Dev đã làm trận này theo bản cũ thì sửa lại theo `01_chuong_1.md` mới.
- **Đáp án [C] của Câu 1, 2, 3 có bản mạnh và bản yếu.** Người chơi chỉ thấy **một** bản: có ghi chú yêu cầu thì hiện bản mạnh, không có thì hiện bản yếu. Hai bản không bao giờ cùng hiện.
- **Câu 3 có hai bản mạnh** khác nhau, tùy ghi chú đang giữ. Một người chơi không thể có cả hai ghi chú đó (một cái của 1-B, một cái của 1-C).
- **Câu 4 có hai bộ** câu hỏi và đáp án, chọn theo `lua_chon_2`. Ba lượt mở đầu của Helena là chung.
- **Không hiện chữ A, B, C trên nút, và xáo ngẫu nhiên thứ tự ba đáp án ở mỗi câu.** Trong kịch bản [C] luôn là đáp án cho điểm Lung lay; giữ nguyên thứ tự thì người chơi đoán được mà không cần nghĩ. Chữ A, B, C trong kịch bản chỉ để người viết và dev gọi tên.
- Sau một phản ứng có đổi sprite (Helena_Smile, Helena_Angry), Helena về Helena_Neutral khi bắt đầu câu hỏi kế tiếp hoặc đoạn kết.

| Câu | Đáp án | Điều kiện hiện | Điểm | Bộ đếm lập trường | Sprite |
|---|---|---|---|---|---|
| 1 | A | | Dao động +2 | duy_tam | Helena_Smile |
| 1 | B | | Dao động +1 | duy_vat_may_moc | |
| 1 | C mạnh | Có ghi chú "Số đo ống 7" | Lung lay +2 | duy_vat_bien_chung | |
| 1 | C yếu | Không có ghi chú đó | Lung lay +1 | duy_vat_bien_chung | |
| 2 | A | | Dao động +2 | bat_kha_tri | |
| 2 | B | | Không cộng gì | bat_kha_tri (kịch bản ghi rõ: "hoài nghi" tính vào bất khả tri) | |
| 2 | C mạnh | Có ghi chú "Số đo ống 7" | Lung lay +2 | duy_vat_bien_chung | |
| 2 | C yếu | Không có ghi chú đó | Lung lay +1 | duy_vat_bien_chung | |
| 3 | A | | Dao động +2 | duy_tam | |
| 3 | B | | Dao động +1 | duy_vat_may_moc | |
| 3 | C mạnh (khóa ngắt) | Có ghi chú "Khóa ngắt dự phòng" | Lung lay +2 | duy_vat_bien_chung | |
| 3 | C mạnh (bà cụ) | Có ghi chú "Lời bà cụ" | Lung lay +2 | duy_vat_bien_chung | |
| 3 | C yếu | Không có cả hai | Lung lay +1 | duy_vat_bien_chung | |
| 4, bộ 2-A | A | | Dao động +2 | sieu_hinh | Helena_Smile |
| 4, bộ 2-A | B | | Dao động +1 | duy_vat_may_moc | |
| 4, bộ 2-A | C | | Lung lay +2 | duy_vat_bien_chung | |
| 4, bộ 2-B | A | | Dao động +2 | sieu_hinh | Helena_Smile |
| 4, bộ 2-B | B | | Lung lay −1 | Không tính vào mục nào (kịch bản ghi rõ) | Helena_Angry |
| 4, bộ 2-B | C | | Lung lay +2 | duy_vat_bien_chung | |

Điểm Lung lay cao nhất theo đường đi, dùng để kiểm thử:

| Lựa chọn 1 | Ghi chú có được | Lung lay tối đa | Thuyết phục được Helena? |
|---|---|---|---|
| 1-A | Không | 1 + 1 + 1 + 2 = 5 | Không bao giờ |
| 1-B | Lời bà cụ | 1 + 1 + 2 + 2 = 6 | Được, nếu chọn [C] ở cả bốn câu |
| 1-C | Số đo, Khóa ngắt | 2 + 2 + 2 + 2 = 8 | Được, còn dư hai điểm |

## 8. Bốn cách kết Chương 1

Ngay sau Câu 4, **mở Mảnh 06 trước** (ở mọi cách kết, kể cả kết cục sớm), rồi xét theo đúng thứ tự này:

1. Nếu `dao_dong` ≥ 5 **và** `lua_chon_0` = A **và** `lua_chon_1` = A **và** `lua_chon_2` = A: **Kết cục sớm "Ca làm hoàn hảo"**. Chạy đoạn kết (BG02, BGM05_Sad_Piano_2), màn hình "KẾT CỤC 1/7", khối Bài học kết cục, nút quay lại. Game dừng ở đây.
2. Nếu `lung_lay` ≥ 6: **Thuyết phục**. Vật phẩm "Giấy thông hành hai chiều". `kiem_chung` +2.
3. Nếu `dao_dong` ≥ 5: **Bị thuyết phục**. Vật phẩm "Thẻ đặc phái của Ban Cố vấn". `long_tin` +3. Sprite Kael_Trando.
4. Còn lại: **Bất phân**. Vật phẩm "Lệnh điều chuyển một chiều". Sprite Kael_Trando.

Hai ngưỡng không thể cùng đạt: muốn Lung lay 6 phải có ít nhất ba câu cho điểm Lung lay, khi đó Dao động nhiều nhất là 2.

Sau khi kết (trừ kết cục sớm): hết Chương 1, sang Chương 2.

## 9. Mảnh lưu trữ

| Mảnh | Tên | Ai nhận | Mở lúc nào |
|---|---|---|---|
| 01 | "Xóa rồi thì có hết không?" | Mọi người | Cuối Mở đầu, sau nhánh của Lựa chọn 0 |
| 02 | "Đèn tắt vì đâu?" | Mọi người | Chương 1 Cảnh 2, ngay sau khi đám đông tan (dòng cụ thể ở mục 6) |
| 03 | "Cái thước" | Chỉ 1-C | Dòng `→ Mở Mảnh 03.` |
| 04 | "Biết sao được" | Chỉ 1-A | Dòng `→ … Mở Mảnh 04.` |
| 05 | "Nghe kể và tận mắt" | Chỉ 1-B | Dòng `→ Mở Mảnh 05.` |
| 06 | "Năm cách đáp lại bà Helena" | Mọi người | Ngay sau Câu 4 của trận đối chất, trước đoạn kết chương |

Hết Chương 1 mỗi người chơi có đúng 4 mảnh: 01, 02, 06 và một trong ba mảnh 03, 04, 05.

**Mảnh 06 có số liệu động.** Năm mục, mỗi mục có dòng "bạn đã chọn: [số] lần". Thay `[số]` bằng bộ đếm lập trường tương ứng. Tổng năm con số có thể là 3 thay vì 4 (đáp án 4-B của bộ 2-B không tính vào đâu). Mảnh cũng "đánh dấu cách người chơi đã dùng nhiều nhất": làm nổi mục có bộ đếm lớn nhất; nếu nhiều mục cùng cao nhất thì làm nổi tất cả.

Nội dung mảnh và bài học kết cục chép nguyên văn từ các khối `>` trong kịch bản.

## 10. Tài nguyên dùng trong hai chương

| Loại | Tên file | Tình trạng |
|---|---|---|
| Nền | BG01_ThanhPho_Aethelgard_Dem, BG02_Phong_Luu_Tru_Du_Lieu, BG03_Thap_Nang_Luong_Sup_Do, BG04_Hanh_Lang_Toi_Tang_Trung | **Chưa có file.** Thư mục `Backgrounds` đang trống |
| Nhạc | BGM01_Dark_Ambient_Cyberpunk, BGM02_Tension_Debate, BGM05_Sad_Piano_1, BGM05_Sad_Piano_2 | Có, trong thư mục `BGM` (cập nhật 04/10/2026: bản `BGM05_Sad_Piano` tách thành ba bản `_1`, `_2`, `_3`) |
| Hiệu ứng | SE01_Coi_Bao_Dong_Tram, SE02_Tieng_Go_Ban_Phim, SE03_Glitch_Loi_He_Thong, SE04_Tieng_No_Thap_Nang_Luong | Có, trong thư mục `SE` (cập nhật 04/10/2026) |
| Sprite | Kael_Neutral, Kael_Surprised, Kael_Trando, Helena_Neutral, Helena_Angry, Veritas_Hologram | Có, trong thư mục `Sprites` |
| Sprite | Helena_Smile, Veritas_Glitch | Có, nhưng nằm ở thư mục `Sprites_Phu` |

- Kael_Trando đang là bản tạm (theo `CON_THIEU.txt`), sẽ được vẽ lại.
- Tên file trong kịch bản là tên đã quy ước. Khi có file thật, đặt đúng tên này.
- Hai chương này không dùng sprite Kael_Angry, Kael_Confident, Kael_Satisfied, Kael_Smile, Helena_Surprised, Veritas_Smile.

## 11. Dữ liệu phải mang sang Chương 2

Bản lưu cuối Chương 1 phải giữ đủ:

- `lua_chon_0`, `lua_chon_1`, `lua_chon_2`, `ket_doi_chat_helena`.
- Năm khuynh hướng và hai lực lượng.
- Toàn bộ vật phẩm, ghi chú, mảnh đã có. Chương 2 kiểm ghi chú để mở câu đáp mạnh, giống trận đối chất ở Chương 1.
- Năm bộ đếm lập trường (để Mảnh 06 vẫn hiện đúng).

Chương 2 rẽ theo: giấy tờ Kael cầm (ba loại, theo `ket_doi_chat_helena`), `lua_chon_1` (nhánh 1-C có bàn tay trái quấn băng), và `lua_chon_0`.

## 12. Các điểm người viết đã chốt thêm (03/10/2026)

Kịch bản không nói rõ 12 điểm dưới đây. Người viết đã chốt, và các mục phía trên đã viết theo đúng các quyết định này. Bảng để dev tra nhanh.

| # | Điểm | Quyết định | Nằm ở mục |
|---|---|---|---|
| 1 | Mảnh 02 mở ở dòng nào | Ngay sau khi đám đông tan theo người thợ van | 6, 9 |
| 2 | Đáp án [C] của Câu 2 tính vào bộ đếm nào | `duy_vat_bien_chung` | 7 |
| 3 | Mảnh 06 mở lúc nào | Ngay sau Câu 4, trước đoạn kết, ở mọi cách kết | 8, 9 |
| 4 | Thứ tự hiển thị ba đáp án | Không hiện chữ A, B, C; xáo ngẫu nhiên mỗi câu | 7 |
| 5 | Thanh Lung lay có xuống dưới 0 không | Không, chặn ở 0 | 3 |
| 6 | Hai thanh có hiện vạch ngưỡng không | Không | 3 |
| 7 | Helena về sprite bình thường khi nào | Khi bắt đầu câu hỏi kế tiếp hoặc đoạn kết | 7 |
| 8 | Mảnh 06 khi hai lập trường bằng điểm | Làm nổi tất cả các mục cùng cao nhất | 9 |
| 9 | Nút quay lại ở kết cục sớm | Cho chọn một trong ba: Lựa chọn 0, 1, 2 | 3 |
| 10 | Trình bày dòng ngoặc tròn và suy nghĩ | Ngoặc tròn in nghiêng, không ô tên; suy nghĩ có ô tên "Kael", chữ nhạt hoặc nghiêng | 2 |
| 11 | Dòng thông báo mục mới | "Vật phẩm mới: [tên]", "Ghi chú mới: [tên]", "Mảnh lưu trữ mới" | 3 |
| 12 | Đáp án dài trong trận đối chất (chốt 04/10/2026) | Nút chỉ hiện phần nêu ý; các câu dẫn chứng Kael nói sau khi chọn | 2, 7 |

Gặp điểm nào khác mà kịch bản và tài liệu này đều không nói: hỏi người viết, đừng tự quyết.

## 13. Không tự ý đổi

- Lời thoại, lời dẫn, tên nhân vật, con số, ngày giờ.
- Không thêm chữ giải thích lựa chọn nào đúng hay sai. Không có đáp án nào được gắn nhãn đúng.
- Người chơi không được thấy năm khuynh hướng, hai lực lượng, và dòng "Lập trường".
- Con số "73.000" trong bản tin sáng **không được tăng** giữa các ngày và không được làm nổi. Đây là chi tiết cài cho chương cuối.
- Veritas không nói gì khi mảnh mở ra. Mảnh chỉ báo bằng biểu tượng cổ tay.
