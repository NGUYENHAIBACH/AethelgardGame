# Tài nguyên còn thiếu: nền, nhạc, hiệu ứng

Ngày soạn: 08/10/2026. Soạn sau khi kịch bản xong đủ Mở đầu và năm chương.

**Đã có ở bên dev (theo lời người viết):** nền BG01 đến BG10; nhạc BGM01 đến BGM09; hiệu ứng SE01 đến SE06.

**Còn thiếu:** 6 nền bắt buộc, 1 bản nhạc, 1 hiệu ứng (8 lớp người của hình cuối do người viết làm, xem mục 3). Ngoài ra có 2 nền tùy chọn.

Tên file phải đặt đúng như trong bảng, vì kịch bản gọi tài nguyên bằng tên ấy.

| Loại | Tên file | Dùng ở đâu | Mức |
|---|---|---|---|
| Nền | `BG11_Loi_ALICE_Ruc_Sang` | Chương 5 Cảnh 3 (trận cuối) | Bắt buộc |
| Nền | `BG14_San_Thang_Tang_Dinh` | Chương 5 Cảnh 2, chặng một | Bắt buộc |
| Nền | `BG15_Day_Phong_Ban_Co_Van` | Chương 5 Cảnh 2, chặng hai | Bắt buộc |
| Nền | `BG16_Truoc_Cua_Loi` | Chương 5 Cảnh 2 chặng ba; đầu Cảnh 4 | Bắt buộc |
| Nền | `BG17_Mat_Dat_Mua` | Chương 5 Cảnh 4, đoạn "Mặt đất" | Bắt buộc |
| Nền | `BG18_Mat_Dat_Binh_Minh` | Chương 5 Cảnh 4, đoạn "Sáng hôm sau" và hình cuối | Bắt buộc |
| Lớp hình cuối | 8 file `KT_…` | Đặt lên BG18 thành nền kết truyện "Bình minh" | **Đã có (08/10/2026):** thư mục `Hinh_cuoi/`, xem mục 3 |
| Nền | `BG14b_San_Thang_Co_Linh`, `BG16b_Truoc_Cua_Loi_Co_Linh` | Chương 5 Cảnh 2 | Tùy chọn (mục 4) |
| Nhạc | `BGM10_ALICE` | Chương 5 Cảnh 3 | Bắt buộc |
| Hiệu ứng | `SE07_Mua` | Chương 5 Cảnh 4 | Bắt buộc |

**Hai điều cần dev xác nhận lại:**

- **BGM05 có ba bản:** kịch bản gọi `BGM05_Sad_Piano_1`, `BGM05_Sad_Piano_2`, `BGM05_Sad_Piano_3`. Nếu bên dev mới có một file BGM05 thì còn thiếu hai bản; mô tả ở mục 5.
- **BGM09_Investigation** là bản thêm mới từ Chương 3. Nếu "BGM 1 đến 9" bên dev chưa gồm bản này thì nó cũng thiếu; mô tả ở mục 5.

`BG12_Bau_Troi_Binh_Minh_Mat_Dat` có trong danh sách cũ nhưng kịch bản không dùng nữa (đã thay bằng BG18). Không cần làm. `BG13_Truoc_Loi_ALICE` (bản Lõi có sẵn bóng người đứng xem) cũng không cần: số người vào Lõi đổi theo lượt chơi, vẽ sẵn thì sai; dùng BG11.

## 1. Yêu cầu chung cho nền

- Tỉ lệ 16:9, ngang, cạnh dài từ 1920 điểm ảnh trở lên, PNG hoặc JPG.
- Cùng kiểu vẽ với BG01 đến BG10: anime, nét viền tối, tô mảng. Khi gen bằng AI nên đưa kèm một hai tấm nền đã có để giữ kiểu vẽ.
- **Nền không có người và không có chữ.** Nhân vật là sprite đặt lên trên; chữ thêm bằng code. Ngoại lệ duy nhất là hai tấm tùy chọn (mục 4); người của hình cuối nằm ở các lớp riêng (mục 3).
- **Một phần ba phía dưới để thoáng,** không đặt chi tiết quan trọng: sprite đứng ở đó.
- Bối cảnh: Aethelgard là một thành phố kín dưới lòng đất, hai trăm năm tuổi. Hai tầng dưới (BG01 đến BG10) cũ, gỉ, vá víu, tối.
- **Tầng Đỉnh (BG11, BG14, BG15, BG16) thì ngược lại:** sạch, sáng đều, thép nhạt màu, không gỉ, không hơi nước. Đèn trắng ấm. Người từ dưới lên phải thấy chói mắt. Cũ kiểu được lau chùi mỗi ngày, không phải cũ kiểu bỏ mặc.
- Trên vách các tấm Tầng Đỉnh có vài cái loa gắn tường, hộp tròn, cùng kiểu với loa ở các nền tầng dưới. Cái máy quản lý thành phố nói qua những cái loa ấy.

## 2. Sáu nền bắt buộc

### BG11_Loi_ALICE_Ruc_Sang

**Dùng cho:** trận cuối. Kael bước vào Lõi, nơi đặt cái máy quản lý thành phố. Hóa ra Lõi chính là cái kho lưu trữ ngày xưa.

**Cần thấy:**

- Một cái sảnh tròn, cao, bằng thép nhạt.
- **Vách kín những ô nhỏ bằng bàn tay, xếp đều từ sàn tới trần,** như một tủ ngăn kéo khổng lồ. **Phần lớn các ô đã tối;** chỉ lác đác vài ô còn sáng mờ. (Ngày xưa chúng sáng hết.)
- Giữa sảnh, lùi về phía sau: **một cái bệ thấp, để trống,** phát ánh sáng trắng ngả vàng. Sprite A.L.I.C.E sẽ đứng trên đó. Mặt trước của bệ có một cái khay nhỏ thò ra, vừa bằng một chiếc đồng hồ đeo tay.
- Ánh sáng cả sảnh tới từ cái bệ; càng lên cao vách càng tối.
- Không khí: yên, sạch, lạnh, hơi giống một thư viện bỏ không.

**Không vẽ:** con số nào trên vách (kịch bản có nhắc một dòng số, để lời dẫn tả), người, bóng người.

### BG14_San_Thang_Tang_Dinh

**Dùng cho:** cái lồng thang hàng lên tới Tầng Đỉnh. Lính đứng kín sân, rồi từng lồng người từ hai tầng dưới đổ lên và đẩy hàng lính vỡ ra.

**Cần thấy:**

- Một cái sân rộng lát thép nhạt, trần cao, đèn trắng ấm chiếu đều.
- Ở một bên khung hình: **cửa thang hàng,** một cửa lồng bằng thanh sắt đen, to và nặng (cái thang hàng chạy suốt ba tầng, cửa ở tầng nào cũng một kiểu), có một vạch vàng kẻ trên sàn trước cửa. Cái cửa lồng đen và xước là thứ cũ kỹ duy nhất trong cả cái sân sạch bóng.
- Phía xa, chính diện: một lối đi rộng dẫn vào trong, hai bên là cột thép. Đó là hướng đoàn người sẽ đi.
- Hai ba cái loa tròn trên vách.
- Giữa sân để trống.

### BG15_Day_Phong_Ban_Co_Van

**Dùng cho:** một hành lang của Ban Cố vấn. Cố vấn ra lệnh hạ một tấm cửa ngăn; thợ kê búa và ghé vai đỡ nó. Dọc hành lang có căn phòng làm ra thứ thuốc đã gửi xuống Tầng Đáy.

**Cần thấy:**

- Hành lang rộng, sàn sáng bóng, đèn trắng ấm.
- Trên trần có một rãnh thép chạy ngang. **Một tấm cửa ngăn bằng thép, dày bằng gang tay, đang hạ lưng chừng và dừng ở tầm ngực người,** chắn ngang cả hành lang. Nhìn phải thấy nó nặng.
- Một bên: dãy cửa gỗ sáng màu, đóng kín.
- Bên kia: **một vách kính lớn.** Sau kính là một căn phòng trắng, một cái bàn dài, những khay ống thủy tinh nhỏ xếp thẳng hàng, và **một cái hòm sắt xám mở nắp, có quai hai bên, đang xếp dở.** Cái hòm phải cùng kiểu với cái hòm thuốc đã tả ở Chương 2 và 4.

### BG16_Truoc_Cua_Loi

**Dùng cho:** hai lúc. Ở Cảnh 2, hàng lính cuối đứng trước cánh cửa Lõi. Ở đầu Cảnh 4, mọi người tụ lại dưới cái thang sắt để mở cái cửa lên mặt đất. **Cái thang và tấm sập là chỗ diễn ra cả nửa đầu Cảnh 4, nên phải nhìn thấy rõ.**

**Cần thấy:**

- Một tiền sảnh cao, hẹp dần về phía cánh cửa như cái phễu.
- Chính diện: **một cửa hai cánh bằng thép, cao tới trần, đóng kín,** không tay nắm, không ổ khóa. Qua khe giữa hai cánh có ánh sáng trắng ngả vàng hắt ra.
- Phía trên cửa: một cái loa lớn hơn các loa khác.
- **Ở một góc tiền sảnh: một cái thang sắt đen, cũ, bắt vào vách, dẫn lên một tấm sập thép trên trần.** Sát tấm sập có **một bánh xe sắt gỉ đỏ, to bằng vành nón** (kiểu vô lăng van). Đây là thứ cũ và gỉ duy nhất trong phòng.
- Sàn trước cửa để trống.

**Không vẽ:** chữ trên bánh xe (kịch bản có hai chữ dập trên vành, để lời dẫn tả).

### BG17_Mat_Dat_Mua

**Dùng cho:** những người sống dưới lòng đất lần đầu chui lên mặt đất. Trời tối, đang mưa. Hai trăm năm nay chưa ai trong số họ thấy bầu trời hay thấy mưa.

**Cần thấy:**

- Một khoảng đất trống rộng, thoai thoải, nhìn ra tới đường chân trời. **Đường chân trời đặt cao hơn giữa khung hình một chút.**
- Trời tối xám xanh, mây thấp. **Mưa rơi đều:** vệt mưa nghiêng theo gió, mặt đất ướt, có vũng nước.
- Ở góc trái tiền cảnh: **miệng một cái cửa sập bằng thép dày, đã mở hất lên,** gỉ và cũ, có ánh đèn trắng ấm hắt từ dưới lên. Đây là lối họ vừa chui lên.
- Cạnh cái cửa: **những tảng đá màu đỏ gỉ lộ khỏi mặt đất** (đá quặng sắt), và **một cái máng sắt cũ, mục gần hết, chạy từ chỗ mấy tảng đá về phía cái cửa.** Ngày xưa người ta chở thứ đá ấy xuống lò qua đây.
- Không nhà cửa, không thành phố đổ nát, không cây. Chỉ đất, đá, nước và trời. Có thể có vài đám cỏ thấp, rất ít.
- Dải giữa và một phần ba phía dưới để thoáng.

### BG18_Mat_Dat_Binh_Minh

**Dùng cho:** sáng hôm sau, và là nền của hình cuối. **Cùng chỗ, cùng góc nhìn, cùng bố cục với BG17;** người chơi phải nhận ra ngay đây là chỗ đêm qua. Cách chắc nhất: đưa BG17 vào AI và yêu cầu vẽ lại đúng cảnh ấy lúc bình minh.

**Cần thấy:**

- Cùng khoảng đất, cùng cái cửa sập mở ở góc trái tiền cảnh, cùng mấy tảng đá đỏ và cái máng sắt gỉ.
- Mưa đã tạnh. Mây tan dần, còn vài dải mỏng nhuộm vàng cam.
- **Mặt trời vừa nhô khỏi đường chân trời, ở chính giữa khung hình.** Ánh sáng ấm chiếu ngang, bóng đổ dài về phía người xem.
- Mặt đất còn ướt, các vũng nước phản chiếu trời sáng. Mấy đám cỏ thấp lấp lánh nước.
- **Dải giữa và một phần ba phía dưới phải thật thoáng:** hàng người của hình cuối sẽ đứng ngang ở đó, hai mẹ con đứng ngay trước mặt trời.
- Tấm này không có người. Người nằm ở các lớp riêng (mục 3).

## 3. Hình cuối của kết "Bình minh": tám lớp `KT_…` (người viết làm)

Hình cuối là nền `BG18_Mat_Dat_Binh_Minh` cộng một hàng người đứng quay lưng về phía người chơi, nhìn về mặt trời. **Tám lớp người đã có, do người viết làm, nằm trong thư mục `Hinh_cuoi/`; bên dev không phải vẽ.** Bên dev chỉ cần làm BG18 đúng như mục 2 (dải giữa và một phần ba phía dưới thật thoáng, mặt trời ở chính giữa), rồi dùng các lớp này đặt lên BG18 để thành tấm nền kết truyện.

Tám file, nền trong suốt: `KT_Kael_Me`, `KT_Rian`, `KT_Tho_xuong_4`, `KT_Vane`, `KT_Helena`, `KT_Tho_be_tao`, `KT_Tang_Trung`, `KT_Doran`. **Cả tám file đã có ở `Hinh_cuoi/` (08/10/2026):** mỗi file cao 1000 điểm ảnh, cắt sát người, nền trong suốt; khi ghép thì đặt chân mọi lớp trên cùng một đường ngang. `KT_Tho_xuong_4` và `KT_Tang_Trung` là nhóm ba người nên rộng hơn các lớp khác.

Lớp nào hiện ở lượt chơi nào, và xếp ra sao: `HUONG_DAN_DEV_CHUONG_5.md` mục 9.

## 4. Hai nền tùy chọn có lính

Có thì cảnh khởi nghĩa ở Chương 5 Cảnh 2 có sức hơn. Không có thì game dùng BG14 và BG16. Kịch bản hiện chưa gọi hai tên này; nếu làm thì báo người viết để thêm thẻ.

**Chưa có sprite lính nào để đưa vào AI.** Vì thế vẽ lính thành **bóng tối không rõ mặt,** đội mũ có kính che. Muốn giữ kiểu vẽ thì đưa `Vane_Neutral` vào làm mẫu về chất liệu giáp, và dặn rõ không vẽ lại Vane.

- **`BG14b_San_Thang_Co_Linh`:** đúng cảnh BG14, thêm một hàng khoảng hai chục lính đứng sát vai kín ngang sân, quay mặt về phía cửa thang. Đồng phục sẫm, **khiên tròn** ở tay trái. Hàng lính nằm ở tầm giữa khung hình, không che một phần ba phía dưới. Nên đưa BG14 vào AI và yêu cầu thêm hàng lính.
- **`BG16b_Truoc_Cua_Loi_Co_Linh`:** đúng cảnh BG16, thêm một hàng khoảng mười lính giáp nặng đứng im trước cánh cửa, quay mặt về phía người xem. To và nặng hơn lính ở BG14b, **khiên dài hình chữ nhật cắm xuống sàn.** Nên đưa BG16 vào AI.

Hai vị tướng (Soren, Ilsa) **không** vẽ vào nền: họ là sprite riêng.

## 5. Nhạc

### BGM10_ALICE (thiếu chắc chắn)

**Dùng cho:** trận cuối với A.L.I.C.E, chạy liên tục từ câu đầu tới hết câu thứ năm.

**Nghe như thế nào:**

- **Đều, sạch, không gấp. Giống nhạc chờ điện thoại, hoặc nhạc nền ở sảnh một tòa nhà văn phòng.** Cái đáng sợ của nó là nó dễ chịu: cái máy lịch sự, luôn cảm ơn, và không bao giờ đổi giọng, kể cả lúc ngoài cửa đang có người bị đẩy ngã.
- Nhạc cụ: đàn điện tử tiếng ấm, tiếng chuông nhỏ hoặc piano điện, một lớp đệm mỏng. Không trống, không dàn dây căng thẳng, không tiếng méo.
- Nhịp chậm tới vừa, một vòng hợp âm ngắn lặp đi lặp lại, không lên cao trào, không có đoạn chuyển.
- **Phải lặp được liền mạch:** người chơi có thể ở trong trận năm tới mười phút. Dài chừng một phút rưỡi tới hai phút là đủ.

**Không phải:** nhạc trùm cuối, nhạc hồi hộp, nhạc đối chất (BGM02 và BGM03 đã là hai bản ấy).

**Từ khóa để tìm:** nhạc chờ (hold music), nhạc sảnh (lobby, elevator music), ambient điện tử nhẹ, corporate calm.

### BGM09_Investigation (chỉ thiếu nếu bên dev chưa có)

**Dùng cho:** Chương 3 Cảnh 2 và 3. Kael và Veritas ngồi xếp các manh mối thành một cuốn sổ.

**Nghe như thế nào:** nhạc lần manh mối. Chậm, tò mò, hơi căng nhưng không sợ. Một nét đàn ngắn lặp lại (piano, đàn dây gảy, hoặc tiếng điện tử nhỏ giọt) trên một lớp đệm trầm. Lặp được liền mạch.

**Từ khóa:** investigation, mystery, thinking, puzzle.

### BGM05_Sad_Piano_1, _2, _3 (chỉ thiếu nếu bên dev mới có một bản)

Ba bản piano buồn, mỗi bản một sắc thái. Nếu chỉ có một file thì đặt nó là `_2` (bản dùng nhiều nhất, bảy lần) và tìm thêm hai bản kia.

| File | Dùng ở đâu | Sắc thái |
|---|---|---|
| `BGM05_Sad_Piano_1` | Chương 1: mười hai tệp là mười hai người bỏng | **Mất mát.** Thưa nốt, lặng, như đếm từng người |
| `BGM05_Sad_Piano_2` | Các màn kết cục buồn, từ Chương 1 tới Chương 5 | **Mất tự do.** Buồn kiểu cam chịu, đều đều, không có lối ra; không bi thảm |
| `BGM05_Sad_Piano_3` | Chương 2: Kael gặp lại mẹ sau mười một năm | **Con về gặp mẹ.** Ấm hơn hai bản kia, có chút ngập ngừng, xót mà không tuyệt vọng |

### Bài hát "Mưa về trên mái" (đã có, không phải tìm)

Bản thu bài hát của game đã có sẵn trong gói bàn giao: `BGM11_Mua_Ve_Tren_Mai.mp3`. Nó phát ở hình cuối của kết "Bình minh" và chạy tới hết màn kết game (xem `HUONG_DAN_DEV_CHUONG_5.md` mục 9).

## 6. Hiệu ứng

### SE07_Mua

**Dùng cho:** Chương 5 Cảnh 4, đoạn "Mặt đất". **Chạy lặp** suốt đoạn ấy, chồng lên nhạc BGM07, và tắt khi sang sáng hôm sau.

**Nghe như thế nào:**

- **Mưa đều, vừa phải, ngoài trời rộng,** rơi xuống đất trống và đá. Không phải mưa rào, không phải mưa bão.
- **Không có sấm, không có gió rít, không có tiếng xe, tiếng chim, tiếng mái tôn hay máng xối.** Ở đó không có mái nhà nào cả.
- Nếu tìm được bản có lẫn vài tiếng giọt rơi lên kim loại thì hay (mưa gõ lên giáp, lên cái cửa thép).
- Dài từ ba mươi giây trở lên, **đầu và cuối nối liền được** để lặp không bị khựng.
- Âm lượng để dưới nhạc và dưới lời thoại.

**Từ khóa để tìm:** steady rain on open ground, rain ambience loop, no thunder.

**Lưu ý cho dev:** đây là hiệu ứng chạy lặp đầu tiên của game (SE01 đến SE06 đều phát một lần). Nếu engine không lặp được hiệu ứng thì coi nó như một kênh nhạc nền thứ hai.

## 7. Ngoài ba loại trên

File này chỉ kê nền, nhạc, hiệu ứng. Sprite và bóng còn thiếu (ba sprite `Soren_Neutral`, `Ilsa_Neutral`, `ALICE_Neutral`; bốn bóng `Bong_Tho_van`, `Bong_Co_van`, `Bong_Nguoi_lam_thuoc`, `Bong_Linh`) do người viết gen riêng; danh sách ở `HUONG_DAN_DEV_CHUONG_5.md` mục 13. **Cập nhật 08/10/2026: ba sprite và năm bóng (cả `Bong_Nguoi_nha`) đã có ở `Sprites_moi/`.**
