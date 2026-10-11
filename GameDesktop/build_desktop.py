"""Dựng bản tải về (chạy: python build_desktop.py). Kết quả ở release/:

  Aethelgard-Windows.zip   giải nén, bấm đúp Aethelgard.exe: game mở trong một cửa sổ riêng
  Aethelgard-Mac.zip       giải nén, mở "Choi Aethelgard.command": game mở trong trình duyệt mặc định

Cả hai dùng chung thư mục game/, chính là bản tĩnh do ../GameStatic/build.py dựng. Thêm --giu-dist để dùng lại
../GameStatic/dist đang có, không dựng lại bản tĩnh.
"""
import os, shutil, stat, subprocess, sys, zipfile

sys.stdout.reconfigure(encoding='utf-8')
HERE = os.path.dirname(os.path.abspath(__file__))
STATIC = os.path.join(HERE, '..', 'GameStatic')
DIST = os.path.join(STATIC, 'dist')
REL = os.path.join(HERE, 'release')


def run(cmd, cwd):
    print('>', ' '.join(cmd))
    subprocess.run(cmd, cwd=cwd, check=True)


def publish(proj, rid, out, extra=()):
    if os.path.isdir(out): shutil.rmtree(out)
    run(['dotnet', 'publish', proj, '-c', 'Release', '-r', rid, '--self-contained', 'true', '-o', out, '-nologo', '-v', 'q',
         '-p:PublishSingleFile=true', '-p:DebugType=none', *extra], HERE)


def copy_game(dst):
    # bỏ file cấu hình của nơi lưu trang (Vercel, git): bản tải về không cần
    shutil.copytree(DIST, dst, ignore=lambda d, names: [n for n in names if n.startswith('.') or n == 'vercel.json'])


def zip_dir(folder, zip_path, exe_names=()):
    """Nén thư mục; với Mac thì ghi cả quyền chạy (x) cho các file trong exe_names, vì nén trên Windows không tự giữ quyền này."""
    if os.path.isfile(zip_path): os.remove(zip_path)
    base = os.path.dirname(folder)
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as z:
        for d, _, fs in os.walk(folder):
            for f in sorted(fs):
                p = os.path.join(d, f); arc = os.path.relpath(p, base).replace(os.sep, '/')
                zi = zipfile.ZipInfo.from_file(p, arc)
                # hình, nhạc đã nén sẵn: nén thêm chỉ tốn thời gian
                zi.compress_type = zipfile.ZIP_STORED if f.lower().endswith(('.webp', '.mp3', '.png', '.ogg')) else zipfile.ZIP_DEFLATED
                if exe_names:
                    zi.create_system = 3   # Unix
                    zi.external_attr = ((stat.S_IFREG | (0o755 if f in exe_names else 0o644)) << 16)
                with open(p, 'rb') as src: z.writestr(zi, src.read())
    print('  %s  %.0f MB' % (os.path.basename(zip_path), os.path.getsize(zip_path) / 1e6))


if '--giu-dist' not in sys.argv or not os.path.isfile(os.path.join(DIST, 'index.html')):
    run([sys.executable, 'build.py'], STATIC)
if os.path.isdir(REL): shutil.rmtree(REL)
os.makedirs(REL)

# ── Windows
win = os.path.join(REL, 'Aethelgard-Windows')
pub = os.path.join(HERE, 'obj', 'pub-win')
publish('Windows/Aethelgard.csproj', 'win-x64', pub,
        ['-p:IncludeNativeLibrariesForSelfExtract=true', '-p:EnableCompressionInSingleFile=true'])
os.makedirs(win)
shutil.copyfile(os.path.join(pub, 'Aethelgard.exe'), os.path.join(win, 'Aethelgard.exe'))
copy_game(os.path.join(win, 'game'))
with open(os.path.join(win, 'DOC TRUOC.txt'), 'w', encoding='utf-8-sig', newline='\r\n') as f:
    f.write('''AETHELGARD: Bản tình ca của thời đại

Cách chơi: giải nén CẢ thư mục này ra, rồi bấm đúp vào Aethelgard.exe. Không cần cài gì thêm.

- Lần đầu chạy, Windows có thể hiện bảng "Windows protected your PC" vì chương trình chưa có chữ ký số.
  Bấm "More info" rồi "Run anyway".
- F11: bật / tắt toàn màn hình.
- Thư mục "game" phải nằm cạnh Aethelgard.exe. Đừng chạy thẳng từ trong file zip.
- Bản lưu nằm ở %LOCALAPPDATA%\\Aethelgard trên máy bạn, không nằm trong thư mục này.
''')
zip_dir(win, os.path.join(REL, 'Aethelgard-Windows.zip'))

# ── Mac (dựng chéo từ Windows; chưa được chạy thử trên máy Mac thật)
mac = os.path.join(REL, 'Aethelgard-Mac')
os.makedirs(os.path.join(mac, 'bin'))
for rid, name in (('osx-arm64', 'Aethelgard-arm64'), ('osx-x64', 'Aethelgard-x64')):
    pub = os.path.join(HERE, 'obj', 'pub-' + rid)
    publish('Mac/AethelgardMac.csproj', rid, pub, ['-p:PublishTrimmed=true'])
    shutil.copyfile(os.path.join(pub, 'Aethelgard'), os.path.join(mac, 'bin', name))
copy_game(os.path.join(mac, 'bin', 'game'))
CMD = 'Choi Aethelgard.command'
with open(os.path.join(mac, CMD), 'w', encoding='utf-8', newline='\n') as f:
    f.write('''#!/bin/bash
# Mở game Aethelgard trên Mac. Chạy xong thì trình duyệt tự mở; đóng cửa sổ Terminal này là tắt game.
cd "$(dirname "$0")/bin" || exit 1
# gỡ dấu "tải từ mạng" và ký tạm tại chỗ: chương trình chưa có chữ ký của Apple, không làm hai việc này thì macOS không cho chạy
xattr -dr com.apple.quarantine . 2>/dev/null
if [ "$(uname -m)" = "arm64" ]; then APP=./Aethelgard-arm64; else APP=./Aethelgard-x64; fi
chmod +x "$APP"
codesign --force -s - "$APP" 2>/dev/null
exec "$APP"
''')
with open(os.path.join(mac, 'DOC TRUOC.txt'), 'w', encoding='utf-8', newline='\n') as f:
    f.write('''AETHELGARD: Bản tình ca của thời đại (bản cho Mac)

Cách chơi: giải nén cả thư mục này, rồi bấm chuột phải (hoặc Control + bấm) vào "Choi Aethelgard.command",
chọn Open, rồi Open lần nữa ở bảng hỏi. Một cửa sổ Terminal hiện ra và trình duyệt tự mở game.

- Phải mở bằng chuột phải > Open ở lần đầu, vì chương trình chưa có chữ ký của Apple; bấm đúp thẳng sẽ bị chặn.
- Giữ cửa sổ Terminal mở trong lúc chơi. Đóng nó là tắt game.
- Nếu vẫn không chạy được, hãy chơi bản trên web.
''')
zip_dir(mac, os.path.join(REL, 'Aethelgard-Mac.zip'), exe_names=('Aethelgard-arm64', 'Aethelgard-x64', CMD))
print('xong:', REL)
