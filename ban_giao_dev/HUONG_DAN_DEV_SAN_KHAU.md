# Hướng dẫn dev: sân khấu nhiều sprite, thẻ tắt sprite, thẻ tắt nhạc

Cập nhật 06/10/2026 (bản 3: thêm sprite của mẹ Kael và Doran, bóng người phụ, biểu cảm mới, luật xoay vòng khi quá ba người; mục 2.1, 2.2, 2.5, 2.6, 2.11 và mục 3 "Bản 3"). Bản 2 (05/10/2026): hình chiếu Veritas vẽ nhỏ cạnh Kael, không chiếm chỗ đứng. Áp dụng cho Mở đầu, Chương 1, Chương 2, Chương 3 và mọi chương sau.

File này viết cho Claude (hoặc một AI lập trình khác) đang sửa game ở `AethelgardGame/Game`. Nó mô tả một thay đổi về cách hiển thị, không đổi cốt truyện, điểm số hay nhánh. Đọc hết một lượt trước khi sửa code: phần 2 là luật, phần 4 là cách làm trong code hiện có, phần 6 là cách tự kiểm.

## 1. Vì sao có thay đổi này

Game biên dịch thẳng các file `Story/*.md` và làm đúng từng thẻ. Bản game hiện tại chỉ có **một ô sprite**: thẻ `[SPRITE: X]` đặt X vào, và X đứng đó tới thẻ kế tiếp hoặc tới tiêu đề `## Cảnh`. Tên người nói không ảnh hưởng gì tới sprite.

Kịch bản thì được viết cho người đọc, nên nó bỏ ngỏ những điều người đọc tự hiểu. Kết quả trong game:

- **Người nói không hiện.** Ở Chương 2, Rian nói 21 câu trong lúc màn hình là sprite của Kael hoặc Vane, vì trước các câu ấy không có thẻ.
- **Sprite treo.** Cuối Chương 1, Kael đã ra hành lang nói chuyện với Veritas mà Helena vẫn đứng trên màn hình. Ở Chương 2, hình chiếu Veritas theo Kael ra giữa đám đông.
- **Hội thoại nhiều người chỉ thấy một người**, và sprite nháy đổi qua lại mỗi lượt thoại.

Người viết đã chốt cách giải quyết: game giữ một **sân khấu** gồm những người đang có mặt, tự đưa người nói lên, và kịch bản chỉ ghi những điều máy không tự biết được (ai rời đi, hình chiếu Veritas bật hay tắt, biểu cảm).

## 2. Luật sân khấu

Đây là đặc tả. Chỗ nào code hiện tại làm khác thì sửa code theo đây.

### 2.1. Ai có sprite

Kael, Helena, Vane, Rian, **mẹ Kael và Doran** đứng trên sân khấu như người. Veritas là hình chiếu, hiện theo cách riêng (mục 2.6). Sáu người phụ có **bóng** (mục 2.11). Mọi vai khác (người bốc hàng, thợ học việc, bà cụ, công nhân, A.L.I.C.E qua loa, Dẫn chuyện…) chỉ có ô tên và không bao giờ lên sân khấu.

**Mẹ Kael:** ô tên trong kịch bản là `Mẹ`, nhưng file ảnh và thẻ dùng tiền tố `Me` (`Me_Neutral.png`, `[SPRITE: Me_LookAway]`, `[SPRITE: Me tắt]`). Code cần một bảng đổi tên người nói sang tiền tố sprite: `Mẹ → Me`; các người khác thì tên và tiền tố trùng nhau.

Danh sách ảnh đầy đủ (file mới và file đã sửa nằm ở `tai_nguyen_sua/Sprites/` của thư mục kịch bản, 900×1200, nền trong suốt; chép đè vào thư mục sprite của game):

| Nhân vật | Ảnh |
|---|---|
| Kael | Neutral, Surprised, Trando, Angry, Confident, Satisfied, Smile (không đổi) |
| Helena | Neutral, Angry, Smile, Surprised, **Thoughtful (mới)** |
| Vane | **Neutral và Surprised (đã sửa: thêm chữ VOLKOV 01 trên ngực giáp)**, Aggressive, **Silent (mới)** |
| Rian | Neutral, **Angry (vẽ lại, thay file cũ)**, **Serious (mới)** |
| Veritas | Hologram, Glitch, Smile, **Serious (mới)** |
| Mẹ Kael | **Me_Neutral, Me_LookAway (mới)** |
| Doran | **Doran_Neutral (mới)** |
| Bóng người phụ | **Bong_Tho_gia, Bong_Tho_tre, Bong_Chi_tho, Bong_Nguoi_gac_thang, Bong_Tho_be_tao, Bong_Chi_tho_tao (mới)** |

### 2.2. Lên sân khấu

| Cách | Áp dụng cho | Biểu cảm khi vào |
|---|---|---|
| Nói hoặc nghĩ một câu (`Kael: "…"`, `Kael (nghĩ): …`) | Kael, Helena, Vane, Rian, Mẹ, Doran | `Neutral` |
| Thẻ `[SPRITE: Tên_BiểuCảm]` | Kael, Helena, Vane, Rian, Me, Doran | Theo thẻ |

**Hình chiếu Veritas chỉ bật bằng thẻ** (`[SPRITE: Veritas_Hologram]`, `[SPRITE: Veritas_Glitch]`, `[SPRITE: Veritas_Serious]`, `[SPRITE: Veritas_Smile]`: thẻ `Veritas_…` nào cũng bật hình chiếu). Cô nói mà hình chiếu chưa bật nghĩa là cô chỉ có tiếng: hiện ô tên và lời thoại, không hiện hình, không tự bật. Lý do: hình chiếu bật hay tắt là một việc trong truyện (cô phải giấu mình trước người lạ cho tới cuối Chương 3), nên chỉ người viết quyết.

Dòng có ô tên `???` không đưa ai lên, vì game không biết đó là ai. Kịch bản luôn đặt thẻ sprite trước những câu ấy.

Người đã ở trên sân khấu mà nói tiếp thì giữ nguyên biểu cảm đang có.

### 2.3. Đổi biểu cảm

`[SPRITE: Vane_Surprised]` khi Vane đã có mặt chỉ đổi ảnh của Vane. Không ai khác bị ảnh hưởng, và chỗ đứng không đổi.

Dạng viết tắt đứng đầu một dòng trả lời trong trận đối chất, `[Helena_Smile]`, có nghĩa y như `[SPRITE: Helena_Smile]`.

Biểu cảm giữ tới thẻ kế tiếp của chính người đó. Khi một người rời sân khấu rồi vào lại, họ vào với biểu cảm của cách vào mới (mục 2.2), không nhớ biểu cảm cũ.

### 2.4. Rời sân khấu

| Thẻ hoặc sự kiện | Tác dụng |
|---|---|
| `[SPRITE: Vane tắt]` | Gỡ riêng Vane. Nếu Vane không có mặt thì không làm gì và **không báo lỗi** (thẻ này hay nằm ở chỗ nhiều nhánh nhập lại) |
| `[SPRITE: tắt]` | Gỡ hết |
| Tiêu đề `## Cảnh …` | Gỡ hết (code hiện tại đã làm) |
| Bất kỳ thẻ `[BG: …]` nào, kể cả khi tên nền trùng nền đang hiện | Gỡ hết |
| Sang chương mới | Gỡ hết, tắt nhạc |
| Màn hình kết cục (`━━ KẾT CỤC … ━━`) | Gỡ hết |

Các thẻ trên cùng một dòng chạy từ trái sang phải. `[BG: BG06_…] [SPRITE: Veritas_Hologram]` nghĩa là đổi nền, gỡ hết, rồi Veritas vào.

Tiêu đề `## Lựa chọn`, `### …`, các nhãn đậm `**…**` và `## Kết Chương 3` **không** gỡ sprite.

### 2.5. Chỗ đứng

Tối đa ba người, **không tính Veritas**. Game giữ một danh sách theo thứ tự **từ trái sang phải trên màn hình**:

- Người thứ nhất vào: đứng giữa.
- Người thứ hai vào: thêm vào bên phải. Hai người tách ra hai bên.
- Người thứ ba vào: chen vào **giữa** hai người kia.
- Có người rời đi: bỏ khỏi danh sách, những người còn lại giữ thứ tự trái phải và dàn lại (còn hai thì hai bên, còn một thì ra giữa).

Chuyển chỗ thì trượt ngang khoảng 300 ms, đừng nhảy cóc. Người mới vào thì mờ dần lên tại chỗ của mình.

**Xoay vòng (bản 3).** Từ khi mẹ Kael có sprite, các cảnh trong xưởng số 4 có bốn người cùng nói (Kael, mẹ, Rian, Vane). Đây không còn là lỗi kịch bản mà là luật:

- Sân khấu đang đủ ba người mà một người thứ tư lên (do nói, nghĩ, hoặc do thẻ `[SPRITE: Tên_BiểuCảm]`): gỡ **người đã lâu nhất chưa nói, trừ Kael**, rồi đưa người mới vào chỗ trống theo luật chỗ đứng ở trên. Kael không bao giờ bị xoay ra.
- "Lâu nhất chưa nói" tính theo lượt thoại hoặc lượt nghĩ gần nhất của từng người; người vừa được thẻ đưa lên mà chưa nói thì tính thời điểm lên.
- Người bị xoay ra mà nói lại thì tự lên lại như mục 2.2 (biểu cảm `Neutral`), và lại xoay một người khác ra.
- Không ghi cảnh báo cho việc này. Thẻ `[SPRITE: Tên tắt]` trong kịch bản vẫn có nghĩa như cũ và được ưu tiên: người đã bị thẻ gỡ thì không tính vào ba chỗ.

### 2.6. Hình chiếu Veritas

Trong truyện, Veritas là một hình chiếu cao bằng gang tay đứng trên cổ tay Kael. Từ cuối Chương 3 cô hiện gần như suốt, nên cô không được chiếm một trong ba chỗ đứng: nếu chiếm thì Kael chỉ còn nói chuyện được với một người.

- Hình chiếu là một trạng thái riêng: tắt, hoặc bật với một trong các ảnh `Veritas_Hologram`, `Veritas_Glitch`, `Veritas_Smile`, `Veritas_Serious`. Nó không nằm trong danh sách trái phải của mục 2.5 và không làm ai đổi chỗ.
- **Vẽ nhỏ, sát bên Kael:** cao chừng một phần ba sprite người, đặt ngang tầm tay Kael, ở phía trong màn hình (Kael đứng trái thì hình chiếu nằm bên phải anh, và ngược lại; Kael đứng giữa thì bên phải). Kael đổi chỗ thì hình chiếu trượt theo.
- **Bật hình chiếu khi Kael chưa có trên sân khấu thì đưa Kael lên luôn** (biểu cảm `Neutral`). Hình chiếu ở trên cổ tay anh, không đứng một mình.
- Tắt bằng `[SPRITE: Veritas tắt]`, hoặc cùng lúc với mọi sự kiện gỡ hết ở mục 2.4. Nếu Kael bị gỡ riêng (`[SPRITE: Kael tắt]`) thì hình chiếu tắt theo.
- Giữ hiệu ứng `holo` và `glitch` đang có.

### 2.7. Ai đang nói

- Người đang nói có mặt trên sân khấu: người ấy sáng bình thường, những người khác tối đi (ví dụ `filter: brightness(.55)`). Veritas nói khi hình chiếu đang bật thì hình chiếu sáng, ba chỗ đứng tối đi.
- Dòng dẫn chuyện, dòng tả trong ngoặc tròn, lời qua loa, lời của người không có sprite, lời Veritas khi cô chỉ có tiếng: không ai bị làm tối.

### 2.8. Hai dấu ngoặc cũ đổi nghĩa

- `Veritas (rất khẽ): "…"` giờ **chỉ còn là kiểu chữ** (thì thầm). Nó không ẩn sprite nữa. Cô hiện hay không là do hình chiếu đang bật hay tắt. Bỏ cờ `HideSprite` cho trường hợp này.
- `A.L.I.C.E (loa): "…"` không đụng tới sân khấu: ai đang đứng thì cứ đứng. Bỏ cờ `HideSprite` cho trường hợp này.

### 2.9. Trận đối chất

Giữ hai luật đã có, chỉ đổi cách thực hiện cho khớp sân khấu:

- Tiêu đề `### Câu N`: đưa **biểu cảm** của người đối chất (Helena hoặc Vane) về `Neutral`. Không gỡ ai, không đụng tới Kael hay Rian.
- Hết trận (lệnh `resolve_helena`, `resolve_vane`): cũng chỉ đưa biểu cảm của người đối chất về `Neutral`.

### 2.10. Nhạc

- `[BGM: tên]` đổi nhạc, lặp tới thẻ `[BGM]` kế tiếp. `[BGM: tắt]` tắt nhạc.
- Sang chương mới thì tắt nhạc; chương nào cũng tự đặt nhạc ở dòng đầu nếu cần.
- **Không còn chỗ nào dev phải tự đặt lệnh tắt nhạc.** Mọi chỗ tắt đã có thẻ. Kịch bản không còn viết "Nhạc tắt" trong ngoặc tròn.

### 2.11. Bóng người phụ (bản 3)

Sáu người phụ có một ảnh bóng: thân đen, nét xám, cùng cỡ với sprite người.

| Ô tên trong kịch bản | Ảnh |
|---|---|
| `Thợ già` | `Bong_Tho_gia` |
| `Thợ trẻ` | `Bong_Tho_tre` |
| `Chị thợ` | `Bong_Chi_tho` |
| `Người gác thang` | `Bong_Nguoi_gac_thang` |
| `Thợ bể tảo` | `Bong_Tho_be_tao` |
| `Chị thợ tảo` (từ Chương 4) | `Bong_Chi_tho_tao` |

- Kịch bản **không có thẻ nào** cho bóng. Game tự làm: một người trong bảng nói thì bóng của người ấy hiện ở **một chỗ riêng sát mép phải màn hình**, lùi ra sau các sprite người (vẽ dưới lớp sprite, có thể nhỏ hơn một chút).
- Bóng không nằm trong danh sách ba chỗ đứng, không làm ai đổi chỗ, không tính vào luật xoay vòng.
- Bóng gỡ khi người kế tiếp nói không phải là một người phụ có bóng (kể cả dòng dẫn chuyện, lời qua loa thì giữ nguyên bóng; chỉ lời thoại hoặc lời nghĩ của một người khác mới gỡ). Một người phụ khác nói thì bóng cũ đổi thành bóng mới tại chỗ.
- Mọi sự kiện gỡ hết ở mục 2.4 cũng gỡ bóng.
- Khi bóng đang nói: bóng sáng bình thường, ba chỗ đứng tối đi như mục 2.7.

## 3. Kịch bản đã đổi những gì

Bốn file `00_mo_dau.md`, `01_chuong_1.md`, `02_chuong_2.md`, `03_chuong_3.md` trong thư mục này đã được xuất lại. Lời thoại không đổi chữ nào. Hai dòng tả ở Chương 2 bị bỏ mấy chữ "Nhạc tắt" vì đó là ghi chú kỹ thuật lọt vào chữ người chơi đọc.

**Việc đầu tiên: chép cả bốn file sang `Game/Story/`**, ghi đè bản cũ. (Chương 3 có thêm những tiêu đề và điều kiện mà bộ biên dịch hiện tại chưa hiểu; việc đó theo `HUONG_DAN_DEV_CHUONG_3.md`, không thuộc file này. Nếu chưa làm Chương 3 thì chỉ chép ba file đầu.)

Bảng dưới để đối chiếu, tìm theo câu neo vì số dòng hai bản khác nhau.

### Mở đầu

| Chỗ | Thẻ thêm | Để làm gì |
|---|---|---|
| Đầu Nhánh 0-A, 0-B, 0-C | `[SPRITE: Kael_Neutral]` | Trả Kael về bình thường sau `Kael_Surprised` lúc tệp tự mở |

### Chương 1

| Chỗ (câu neo) | Thẻ | Để làm gì |
|---|---|---|
| Trước "Kael (nghĩ): Nghe cũng xuôi tai." | `[SPRITE: Helena tắt]` đứng trước `[SPRITE: Kael_Neutral]` | Bài nói đã xong, Kael nói chuyện riêng với Veritas (chỉ có tiếng) |
| Ba cách kết, ngay trước dòng Kael ra hành lang | `[SPRITE: Helena tắt]` | Kael rời phòng |
| Ba cách kết, ngay sau dòng ấy | `[SPRITE: Veritas_Hologram]` | Chỉ còn hai người, Veritas hiện hình (người viết chốt) |

### Chương 2

| Chỗ (câu neo) | Thẻ | Để làm gì |
|---|---|---|
| Sau "(Hai lính gác Ban Cố vấn đưa anh ra mép quảng trường.)" | `[SPRITE: Helena tắt]` | Kael rời chỗ Helena |
| Dòng `[BG: BG05_…] [SE: SE06_…]` ở "Chân thang" | thêm `[BGM: tắt]` | Trước đây là chữ "Nhạc tắt." trong dòng tả; dòng tả giờ là "(Cửa lồng mở.)" |
| Dòng `[BGM: BGM05_Sad_Piano_3]` | thêm `[SPRITE: Rian tắt]` | Kael đi vào cuối xưởng gặp mẹ; Rian vào lại khi nói "Nói đi chứ" |
| Trước "Veritas (rất khẽ): "Họ hát khác tôi hai chữ."" | `[SPRITE: Rian tắt] [SPRITE: Kael_Neutral] [SPRITE: Veritas_Hologram]` | Rian đã đi trước một quãng xa; Kael hết sững; hình chiếu bật |
| Dòng `[BG: BG06_…]` đầu Cảnh 3 | thêm `[BGM: tắt]` | Dòng tả giờ là "(Hình chiếu trên cổ tay Kael tắt: cuối lối đi, Rian đã đứng lại chờ.)" |
| Dòng thẻ ở `### Câu 1` trận Vane | thêm `[SPRITE: Kael_Neutral]` | `Kael_Trando` chỉ dành cho lúc anh cứng họng trước trận |
| Sau `Vane: "Về chỗ tôi đếm được."` | `[SPRITE: Vane tắt]` | Vane đi |
| Sau `Vane: "Hết hôm nay thôi, bà tổ trưởng."` (trong nhánh 5-B) | `[SPRITE: Vane tắt]` | Vane đi |
| Sau "(Ông ta để cái hòm mở trên bệ búa, đi ra.)" | `[SPRITE: Vane tắt]` | Vane đi |
| Đầu `### Đoạn 2: mẹ và Kael` | `[SPRITE: Rian tắt]` | Kael đi về cuối xưởng, chỉ còn anh với mẹ |

### Chương 3

| Chỗ (câu neo) | Thẻ | Để làm gì |
|---|---|---|
| Dòng `[BGM: BGM05_Sad_Piano_2]` ở lối T2 của Nhánh 6-B | thêm `[SPRITE: tắt]` | Từ đây là lời dẫn chuyện của kết cục |
| Sau "(Ông ta đi ra. Mấy người đã tiêm nhìn theo ông ta, rồi nhìn nhau.)" | `[SPRITE: Vane tắt]` | Vane đi |
| Sau "(Lồng thang đi xuống. Hình chiếu của Veritas đứng trên cổ tay anh.)" | `[SPRITE: Veritas_Hologram]` | Trước đây thiếu thẻ |
| Sau "(Lồng thang chậm lại. Hình chiếu tự tắt trước khi cửa mở.)" | `[SPRITE: Veritas tắt]` | |
| Đầu "Bản dưới" của Kết Chương 3 | `[BGM: tắt]` | Trước đây chỉ bản trên có thẻ tắt |
| Cuối dòng "- Nếu D1 hoặc D2. (Vane nghe xong thì đi thẳng ra, không nói gì.)" | `[SPRITE: Vane tắt]` | Vane đi |

Nhiều thẻ `[SPRITE: Rian_Neutral]` / `[SPRITE: Vane_Neutral]` đổi qua đổi lại trong Chương 2 giờ là thừa (người nói tự lên). Chúng vô hại theo luật 2.3 và được để nguyên. Đừng xóa trong `Story/`: file ấy là bản sao, sửa ở đó sẽ lệch với nguồn.

### Bản 3 (06/10/2026): sprite mới, mẹ Kael, Doran

Bốn file kịch bản trong thư mục này đã xuất lại lần nữa. Lời thoại và lời tả không đổi chữ nào; chỉ thẻ `[SPRITE]` đổi, và bốn dòng `(Ghi chú cho người làm game: …)` ở Chương 2 được sửa cho khớp (người gác thang, mẹ và các thợ, thợ bể tảo, `Vane_Silent` ở câu 3). Chép đè cả bốn file sang `Game/Story/`.

Cột "Câu neo" là dòng đứng ngay sau thẻ trong file mới.

**Mở đầu**

| Trước | Nay | Câu neo |
|---|---|---|
| `(không có)` | `[SPRITE: Doran_Neutral]` | ???: "Lại ngắm cái thẻ cũ đấy à, Kael?" |
| `(không có)` | `[SPRITE: Doran tắt]` | Kael (nghĩ): Ở trên này chỉ cần sai một lần là người ta có cớ trả mình |
| `(không có)` | `[SPRITE: Doran tắt]` | [SE: SE03_Glitch_Loi_He_Thong] |
| `(không có)` | `[SPRITE: Kael_Neutral]` | (Hết ca. Doran đã về. Tệp vẫn mở trên màn hình…) |

**Chương 1**

| Trước | Nay | Câu neo |
|---|---|---|
| `(không có)` | `[SPRITE: Doran tắt]` | [SE: SE04_Tieng_No_Thap_Nang_Luong] |
| `(không có)` | `[SPRITE: Veritas_Serious]` | Veritas: "...Xin lỗi. Cái giấy gọi là tại tôi." |
| `(không có)` | `[SPRITE: Kael_Neutral]` | [GHI CHÚ MỚI: Giấy gọi 18 giờ] |
| `(không có)` | `[SPRITE: Kael_Neutral]` | Kael: "Tôi chưa đồng ý gì hết." |
| `[SPRITE: Kael_Surprised]` trước câu Kael: "Không. Không không không…" | Thẻ ấy dời lên, đứng trước dòng tả bên phải | (Kael đứng sững giữa đường. Anh nhìn quanh. Không ai nhìn lại.) |

**Chương 2**

| Trước | Nay | Câu neo |
|---|---|---|
| `(không có)` | `[SPRITE: Helena_Smile]` | (Bà quay sang Kael, nói nhỏ, vẫn mỉm cười.) |
| `(không có)` | `[SPRITE: Kael_Smile]` | Veritas: "Cậu đang cười một mình đấy." |
| `(không có)` | `[SPRITE: Kael_Neutral]` | Kael: "Cô không mỉa một câu thì không chịu được à." |
| `(không có)` | `[SPRITE: Rian_Serious]` | (Xưởng im. Rian nói nhỏ hẳn đi.) |
| `(không có)` | `[SPRITE: Rian_Neutral]` | **Nếu 1-A (Kael chỉ đếm cáng từ xa)** |
| `[SPRITE: Rian tắt] [SPRITE: Kael_Neutral] [SPRITE: Veritas_Hologram]` | `[SPRITE: Rian tắt] [SPRITE: Me tắt] [SPRITE: Kael_Neutral] [SPRITE: Veritas_Serious]` | Veritas (rất khẽ): "Họ hát khác tôi hai chữ." |
| `(không có)` | `[SPRITE: Veritas_Hologram]` | - Nếu 3-B (Veritas mang Mảnh 09): |
| `(không có)` | `[SPRITE: Rian_Serious]` | (Rian nói nhỏ, không nhìn anh.) |
| `(không có)` | `[SPRITE: Rian_Neutral]` | **Nếu 4-B và Rian đã gạt đi (1-A)** |
| `(không có)` | `[SPRITE: Rian_Serious]` | Rian: "Ông tôi rót mẻ ấy." |
| `(không có)` | `- [SPRITE: Rian_Serious]` | - (Rian nói nhỏ, như nói với cái lò.) |
| `[SPRITE: Vane_Surprised]` | `[SPRITE: Vane_Silent]` | (Vane không đáp. Ông ta nhìn bàn tay sắt của mình, rồi nhìn Kael.) |
| `(không có)` | `- [SPRITE: Me_LookAway]` | - (Thợ trẻ nhìn ống tiêm, rồi nhìn sang mẹ. Bà quay mặt đi.) |
| `(không có)` | `- [SPRITE: Vane_Silent]` | - (Vane im rất lâu.) |
| `- [SPRITE: Vane_Surprised]` | `(bỏ)` | - Vane: "Dưới bớt ba lần. Thùng lên vẫn sáu. Trên không bớt." |
| `(không có)` | `- [SPRITE: Vane_Silent]` | - (Vane im rất lâu.) |
| `[SPRITE: Rian_Neutral]` | `[SPRITE: Rian_Angry]` | Rian: "Gác cho ai? Ông là chó giữ nhà cho tầng trên." |
| `(không có)` | `- [SPRITE: Vane tắt]` | - Mẹ: "Rian." |
| `(không có)` | `[SPRITE: Rian_Neutral]` | Vane: "Lò lớn tắt bốn mươi năm. Đai mỗi mẻ một mỏng. Cái loa thì sáng  |
| `[SPRITE: Rian_Neutral]` | `[SPRITE: Rian_Serious]` | Rian: "Đêm nay tôi ngủ ở xưởng, bác ạ." |
| `[SPRITE: Rian_Neutral]` | `[SPRITE: Rian_Serious]` | Rian: "Cái ống trên đó. Vỡ vì không có đai à?" |
| `(không có)` | `[SPRITE: Kael_Neutral] [SPRITE: Me_LookAway]` | (Mẹ không nhìn anh. Bà múc nửa bát của mình sang bát cậu thợ học việc. |
| `(không có)` | `[SPRITE: Veritas_Serious]` | Veritas: "Bài ấy tôi giữ hai trăm năm. Chưa lần nào tôi cần có ai hát  |
| `(không có)` | `[SPRITE: Veritas_Hologram]` | Veritas: "Mà cái giáp của ông ta. Trên ngực có dập chữ: VOLKOV 01." |
| `(không có)` | `- [SPRITE: Veritas tắt]` | - (Kael đi về phía chân thang. Bốn người đồ đen nhìn anh. Không ai bướ |
| `(không có)` | `- [SPRITE: Veritas tắt]` | - (Kael đi về phía chân thang. Một người đồ đen đòi xem giấy, lật đi l |
| `(không có)` | `[SPRITE: Kael_Neutral]` | (Ở cửa xưởng giờ có một người mặc đồ đen đứng gác.) (đầu Cảnh 4, ngay sau thẻ `[BG]`) |
| `(không có)` | `[SPRITE: Rian tắt]` | Mẹ: "Vane. Lần này mang cả hòm à?" |

**Chương 3**

| Trước | Nay | Câu neo |
|---|---|---|
| `(không có)` | `[SPRITE: Helena_Thoughtful]` | (Helena không đáp ngay. Bà nhìn cốc nước trước mặt mình.) |
| `(không có)` | `[SPRITE: Helena_Neutral]` | (Bà mở cuốn sổ, xoay về phía anh. Mỗi dòng một ngày, một con số, một c |
| `(không có)` | `[SPRITE: Vane_Silent]` | (Ông ta im một lúc lâu.) |
| `(không có)` | `[SPRITE: Vane_Neutral]` | Vane: "Tám hôm nay tối nào tôi cũng ghi lên trên hai chữ 'bình thường' |
| `(không có)` | `[SPRITE: Vane_Silent]` | (Ông ta nhìn bàn tay sắt của mình một lúc lâu.) |
| `[SPRITE: Rian_Neutral]` | `[SPRITE: Rian_Angry]` | Rian: "Ông ghi là bình thường? Thế tám ngày nay trên đó tưởng bọn tôi  |
| `(không có)` | `[SPRITE: Rian_Neutral]` | Vane: "Tối nay tôi ghi đúng cái tôi thấy: xưởng số 4 không đập, tổ trư |
| `(không có)` | `[SPRITE: Vane_Silent]` | - Nếu 5-B. Vane: "Hôm qua tôi cho lính vào, tôi tưởng thế là đủ để bà  |
| `(không có)` | `[SPRITE: Vane_Neutral]` | (Ông ta quay ra. Tới chỗ Kael thì dừng.) |
| `(không có)` | `[SPRITE: Vane tắt]` | Mẹ: "Con nghe cả rồi đấy." |
| `(không có)` | `[SPRITE: Me_LookAway]` | (Mẹ không ngẩng lên.) |
| `(không có)` | `[SPRITE: Doran tắt]` | (Kael cắm thiết bị cổ tay vào cổng đăng nhập. Lô 4.414 đang chờ, chưa  |
| `(không có)` | `- Nếu bản dưới. [SPRITE: Veritas_Serious]` | (Veritas thôi đùa.) |
| `(không có)` | `- Nếu bản dưới. [SPRITE: Veritas_Hologram]` | [GIEO: dòng về cái cửa, Chương 5] |
| `(không có)` | `[SPRITE: Helena_Thoughtful]` | (Bà nhìn trang sổ, rồi nhìn hai cột chữ trên bàn.) |
| `(không có)` | `[SPRITE: Helena_Neutral]` | Helena: "Mấy điều ấy tôi không biết thật. Nhưng cậu cũng không thể biế |
| `(không có)` | `[SPRITE: Helena_Angry]` | Helena: "Hôm nay tôi không báo. Nhưng tôi sẽ không quên là cậu vừa đặt |
| `(không có)` | `[SPRITE: Helena_Neutral]` | - Nếu veritas = dong_hanh: |
| `(không có)` | `[SPRITE: Vane_Silent]` | (Vane nhìn hai cột chữ. Rồi nhìn bàn tay sắt của mình.) |
| `(không có)` | `[SPRITE: Vane_Neutral]` | Vane: "Ai kể cho cậu? Cậu hai mươi mấy tuổi. Dưới này không ai nhớ quá |

## 4. Cách làm trong code hiện có

Tên file và hàm dưới đây là của bản game ngày 05/10/2026. Nếu code đã khác, giữ đúng luật ở phần 2 và tự tìm chỗ tương ứng.

### 4.1. `Models/Game/GameState.cs`

Thay trường `Sprite` (một chuỗi) bằng danh sách sân khấu, theo thứ tự trái sang phải:

```csharp
public sealed class Actor { public string Name = ""; public string Expr = "Neutral"; public int LastSpoke; }
public List<Actor> Stage = new();   // chỉ Kael, Helena, Vane, Rian
public string? Holo;                // null = tắt; "Hologram" | "Glitch" | "Smile"
```

Giữ được bản lưu cũ: khi đọc một `GameState` còn trường `Sprite` kiểu `"Helena_Smile"`, đổi nó thành một `Actor` duy nhất (hoặc thành `Holo` nếu tên bắt đầu bằng `Veritas_`, kèm Kael trên sân khấu). Đừng làm bản lưu cũ hỏng.

### 4.2. `Models/Story/Compiler.cs`

- Trong `Content`, thẻ `[SPRITE: …]` có ba dạng. Tách ngay lúc biên dịch thành ba lệnh riêng (ví dụ `CmdKind.Sprite`, `CmdKind.SpriteOff` với `Value` là tên hoặc `null` cho "gỡ hết"). Giá trị `tắt` không bao giờ được coi là tên file ảnh.
- Trong `Speech`, bỏ hai chỗ đặt `HideSprite` (luật 2.8). `Whisper` vẫn giữ.
- Trong `Stage`, bỏ nhánh đặc biệt `t.StartsWith("(Nhạc tắt. Hình chiếu")`: dòng ấy không còn trong kịch bản và nhạc đã có thẻ. Hai nhánh đặc biệt còn lại ("(Màn hình tối." và "(Im lặng một lúc. Người thợ van") giữ nguyên.
- Trong `H3`, chỗ phát lệnh sprite cho `### Câu N`: đổi thành lệnh "đặt biểu cảm nếu đang có mặt" (luật 2.9), không phải lệnh đưa người lên.

### 4.3. `Models/Game/GameEngine.cs`

Viết ba hàm nhỏ trên `GameState` và cho mọi chỗ khác gọi chúng, đừng sửa danh sách rải rác:

- `Enter(name, expr)`: nếu đã có mặt thì chỉ đổi `Expr`; chưa có thì chèn theo luật 2.5 (hai người đầu thêm vào cuối, người thứ ba chèn vào vị trí 1).
- `Leave(name)`: gỡ nếu có, không có thì thôi.
- `Clear()`: gỡ hết và tắt hình chiếu.
- `HoloOn(expr)`, `HoloOff()` theo luật 2.6. Thẻ `[SPRITE: Veritas_…]` và `[SPRITE: Veritas tắt]` đi vào hai hàm này, không vào `Enter` / `Leave`. `Leave("Kael")` gọi luôn `HoloOff()`.

Rồi nối vào:

- `SayI` loại `Say` hoặc `Think` có `Name` là Kael, Helena, Vane hoặc Rian: nếu chưa có mặt thì `Enter(name, "Neutral")`. Ghi lại `LastSpoke`. So tên đúng nguyên chữ; đừng dùng `StartsWith`, vì có ô tên kiểu "Thợ bể tảo" và sau này có thể có "Lính của Vane".
- `CmdKind.Bg` và `CmdKind.SceneStart`: `Clear()`.
- `EndScreenI` và lúc sang chương mới: `Clear()`; sang chương mới thì thêm tắt nhạc.
- `EndBattle`: đặt biểu cảm người đối chất về `Neutral` thay cho việc ghi đè cả ô sprite.

Mỗi lần sân khấu đổi, gửi cho trình duyệt **cả danh sách** (ví dụ bước `{"t":"stage","v":[{"n":"Vane","e":"Neutral"}, …],"holo":"Hologram"}`) thay cho bước `spr` cũ. Gửi cả danh sách thì tải lại, lùi lại hay bỏ qua nhanh đều không lệch. Mỗi bước `say` kèm tên người nói như đã có, để trình duyệt biết làm sáng ai.

### 4.4. `wwwroot/js/game.js` và `wwwroot/css/game.css`

- Thay một thẻ `<img id="sprite">` bằng một khung chứa các ảnh, mỗi người một phần tử gắn theo tên (đừng tạo lại phần tử khi chỉ đổi biểu cảm, kẻo ảnh nháy).
- Ba vị trí bằng lớp CSS, ví dụ `pos-1of1`, `pos-1of2`, `pos-2of2`, `pos-1of3`, `pos-2of3`, `pos-3of3`, dùng `left` hoặc `transform` có `transition` để trượt.
- Hình chiếu Veritas là một phần tử riêng, nhỏ, bám theo vị trí của Kael (luật 2.6). Giữ nguyên lớp `holo` và `glitch`.
- Thêm lớp `dim` cho người không nói (luật 2.7). Bỏ lớp `voice`.
- Trên màn hình hẹp, ba sprite cho phép chồng mép lên nhau; người đang nói nằm trên cùng (`z-index`).
- Hàm khôi phục cảnh khi tải bản lưu đọc danh sách sân khấu thay cho một tên sprite.

### 4.5. `Controllers/CheckController.cs`

Thêm vào trang `/Check`:

- Tên sprite trong thẻ mà không có file ảnh: báo lỗi như cũ, nhưng `tắt` và `Tên tắt` không phải tên file.
- Trong 500 ván chạy thử: báo nếu có lúc sân khấu quá ba người (không tính Veritas).
- Một danh sách để người viết rà (không phải lỗi): mọi câu Veritas nói khi hình chiếu đang tắt mà dòng không ghi `(rất khẽ)`, kèm số dòng. Đây là chỗ dễ sót thẻ hình chiếu nhất khi viết chương mới.
- Nên có `/Check/Stage?keys=…`: in ra sân khấu sau mỗi dòng của một ván, để soát bằng mắt.

## 5. Những điều trong các hướng dẫn cũ đã hết hiệu lực

Ba file hướng dẫn trước vẫn đúng về mọi thứ khác. Riêng các câu sau bị file này thay:

| File | Câu cũ | Giờ là |
|---|---|---|
| `HUONG_DAN_DEV.md` mục 2 | "`[SPRITE: tên]`: Đổi sprite đang hiện" | Luật 2.2 đến 2.4 |
| `HUONG_DAN_DEV.md` mục 5 | "Mỗi thẻ `[SPRITE]` thay sprite đang hiện; kịch bản không dựng hai sprite cạnh nhau" | Sân khấu tối đa ba người |
| `HUONG_DAN_DEV.md` mục 7 | "Helena về Helena_Neutral khi bắt đầu câu hỏi kế tiếp hoặc đoạn kết" | Vẫn đúng, hiểu là chỉ đổi biểu cảm (luật 2.8) |
| `HUONG_DAN_DEV_CHUONG_2.md` mục 3 | "`Veritas (rất khẽ)`… không hiện sprite" | Chỉ là kiểu chữ; hiện hay không do hình chiếu bật hay tắt (luật 2.8) |
| `HUONG_DAN_DEV_CHUONG_2.md` mục 5 | "Hai chỗ nhạc phải tắt mà không có thẻ, dev tự đặt" và "BGM01 chạy… tới khi BGM05_Sad_Piano_3 thay nó" | Cả hai chỗ đã có `[BGM: tắt]`. BGM01 tắt ở chân thang Tầng Đáy; từ đó tới lúc gặp mẹ không có nhạc |
| `HUONG_DAN_DEV_CHUONG_3.md` mục về Veritas | "có tiếng, không có sprite" cho `(rất khẽ)` | Như trên. Ở bản trên của Cảnh 2 và 3 cô vẫn chỉ có tiếng, vì ở đó không có thẻ hình chiếu |

## 6. Tự kiểm sau khi sửa

Chạy `/Check` trước: không có dòng nào không đọc được, không thiếu file, 500 ván không lỗi.

Rồi chơi (hoặc dùng `/Check/Stage`) tới các điểm sau và so sân khấu. Người viết theo thứ tự trái sang phải; hình chiếu Veritas ghi riêng sau dấu cộng, luôn nhỏ và sát Kael:

| Điểm | Sân khấu phải là |
|---|---|
| Mở đầu, Cảnh 3, câu "Lô bốn nghìn bốn trăm mười hai…" | Kael_Neutral |
| Mở đầu, nhánh 0-B, câu "Cái… Tệp biết nói à?!" | Kael_Neutral + hình chiếu Veritas_Glitch |
| Chương 1, quảng trường, Helena đang nói với người thợ van | Helena_Neutral |
| Chương 1, quảng trường, "Cậu đứng lại rồi kìa." | Kael_Neutral (Veritas chỉ có tiếng, không ai bị làm tối) |
| Chương 1, trận Helena, sau một câu Helena cười | Helena_Smile, Kael_Neutral |
| Chương 1, kết "Thuyết phục", "Tôi im có giỏi không?" | Kael_Neutral + hình chiếu |
| Chương 1, kết "Bất phân", "Tới khi tôi biết nên tin cậu tới đâu." | Helena_Neutral, Kael_Trando |
| Chương 1, kết cục sớm, đoạn "Bốn mươi mốt ngày sau…" | trống |
| Chương 2, chân thang Tầng Đáy, suốt Lựa chọn 3 | Kael_Neutral; không có nhạc |
| Chương 2, Lựa chọn 4, Rian đang nói | Kael_Trando, Rian_Neutral; Rian sáng |
| Chương 2, Cảnh 4, trước Lựa chọn 5 | Vane_Neutral, Rian_Neutral |
| Chương 2, nhánh 5-A, sau câu đầu của Kael | Vane, Kael, Rian (Kael chen vào giữa) |
| Chương 2, trận Vane, từ Câu 1 | Vane_Neutral, Kael_Neutral; thêm Rian ở giữa từ lúc cậu ta nói |
| Chương 2, Cảnh 6 khi thắng, sau "Về chỗ tôi đếm được." | Rian_Neutral |
| Chương 2, Đoạn 2, mẹ và Kael | Không có Rian, không có Vane. Kael lên từ câu đầu tiên anh nói |
| Chương 2, kết cục "Không còn đói", các dòng dẫn chuyện | trống |
| Chương 3, Cảnh 2 bản trên (xưởng lưu trữ) | Kael; Veritas chỉ có tiếng |
| Chương 3, Nhánh 6-C trước mặt Helena, từ Nhịp 3 | Kael, Helena + hình chiếu (cạnh Kael; không ai đổi chỗ khi nó bật) |
| Chương 3, kết chương bản trên, trong thang | Kael_Neutral + hình chiếu (thẻ bật hình chiếu kéo Kael lên cùng); không có nhạc |
| Chương 3, Nhánh 6-C trước mặt Vane, từ Nhịp 3 | Kael, Vane + hình chiếu |
| Chương 3, kết chương bản dưới, lúc "(Kael chạy.)" | Kael + hình chiếu (Vane đã đi ở cả ba lối); không có nhạc |

Ba điều cần nhìn bằng mắt, vì bảng không bắt được: sprite trượt chứ không nhảy; đổi biểu cảm không làm ảnh nháy; bỏ qua nhanh (giữ Ctrl) rồi dừng thì sân khấu vẫn đúng.

## 7. Đừng làm

- Đừng tự bật hình chiếu Veritas khi cô nói, và đừng cho hình chiếu chiếm một chỗ đứng. Lần cô hiện hình trước mặt người khác ở cuối Chương 3 là một bước ngoặt của truyện.
- Đừng thêm, bớt hay đổi thẻ trong `Story/*.md` để "chữa" một chỗ hiển thị sai. Nếu thấy một người đứng sai chỗ, sai lúc, đó là lỗi kịch bản: ghi lại số dòng và báo người viết. Nguồn là `kich_ban/` của dự án truyện; `Story/` chỉ là bản sao.
- Đừng đoán biểu cảm từ nội dung câu thoại. Biểu cảm chỉ đổi bằng thẻ.
- Đừng dùng các sprite chưa được kịch bản gọi tên (Kael_Injured, Vane_Smug, Rian_Angry…), dù file có trong thư mục.
