"""Dựng bản tĩnh của game vào thư mục dist/ (chạy: python build.py).

Bản tĩnh không cần máy chủ .NET: bộ máy kịch bản chạy trong trình duyệt bằng WebAssembly. Mọi thứ lấy từ ../Game,
không sửa tay gì trong dist/. Các bước:
  1. publish dự án WebAssembly (bộ máy + sáu file kịch bản nhúng sẵn);
  2. chạy tạm bản máy chủ ở chế độ Production để lấy HTML của hai trang (menu, màn chơi);
  3. chép js, css, tài nguyên; đổi các đường dẫn tuyệt đối thành tương đối và đổi chỗ gọi máy chủ thành gọi WebAssembly;
  4. ghi manifest.json (danh sách file tài nguyên) thay cho /Game/Manifest.
"""
import hashlib, io, json, os, re, shutil, socket, subprocess, sys, time, urllib.request

sys.stdout.reconfigure(encoding='utf-8')
HERE = os.path.dirname(os.path.abspath(__file__))
GAME = os.path.join(HERE, '..', 'Game')
DIST = os.path.join(HERE, 'dist')
PUB = os.path.join(HERE, 'obj', 'publish')


def run(cmd, cwd):
    print('>', ' '.join(cmd))
    subprocess.run(cmd, cwd=cwd, check=True)


def read(p): return io.open(p, encoding='utf-8-sig', newline='').read()
def write(p, t): io.open(p, 'w', encoding='utf-8', newline='').write(t)


def sub(text, old, new, count=None, what=''):
    """Thay chuỗi và bắt lỗi nếu số chỗ thay khác dự kiến: game đổi thì script báo ngay, không âm thầm dựng sai."""
    n = text.count(old)
    if n == 0 or (count is not None and n != count):
        raise SystemExit('build.py: %s: tìm thấy %d chỗ "%s", cần %s' % (what, n, old, count if count is not None else 'ít nhất 1'))
    return text.replace(old, new)


# ── 1. publish WebAssembly
if os.path.isdir(PUB): shutil.rmtree(PUB)
run(['dotnet', 'publish', 'AethelgardStatic.csproj', '-c', 'Release', '-o', PUB, '-nologo', '-v', 'q'], HERE)

# ── 2. HTML của hai trang, lấy từ bản máy chủ chạy tạm
srv_out = os.path.join(HERE, 'obj', 'server')
run(['dotnet', 'build', 'AethelgardGame.csproj', '-c', 'Release', '-o', srv_out, '-nologo', '-v', 'q'], GAME)
s = socket.socket(); s.bind(('127.0.0.1', 0)); port = s.getsockname()[1]; s.close()
env = dict(os.environ, ASPNETCORE_ENVIRONMENT='Production')
srv = subprocess.Popen(['dotnet', os.path.join(srv_out, 'AethelgardGame.dll'), '--urls', 'http://127.0.0.1:%d' % port,
                        '--contentRoot', os.path.abspath(GAME)], cwd=GAME, env=env, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
base = 'http://127.0.0.1:%d' % port
try:
    for _ in range(60):
        try: urllib.request.urlopen(base + '/', timeout=2).read(); break
        except Exception: time.sleep(0.5)
    else: raise SystemExit('build.py: bản máy chủ tạm không lên')
    home = urllib.request.urlopen(base + '/').read().decode('utf-8')
    play = urllib.request.urlopen(base + '/Game/Play').read().decode('utf-8')
    manifest = json.loads(urllib.request.urlopen(base + '/Game/Manifest').read().decode('utf-8'))
finally:
    srv.kill()

# ── 3. chép và đổi
# làm rỗng dist/ chứ không xóa hẳn thư mục: có thể đang có máy chủ thử chạy trong đó
os.makedirs(DIST, exist_ok=True)
for n in os.listdir(DIST):
    if n in ('.vercel', '.gitignore'): continue   # dự án Vercel đã nối: giữ lại để lần đưa lên sau không bị hỏi lại
    q = os.path.join(DIST, n)
    shutil.rmtree(q) if os.path.isdir(q) else os.remove(q)
shutil.copytree(os.path.join(GAME, 'wwwroot'), DIST, dirs_exist_ok=True)
shutil.copytree(os.path.join(PUB, 'wwwroot', '_framework'), os.path.join(DIST, '_framework'))


def html(t, name):
    t = re.sub(r'(href|src)="/(css|js|assets)/', r'\1="\2/', t)
    t = t.replace('href="/favicon.ico"', 'href="favicon.ico"')
    t = t.replace('href="/Game/Play?new=1"', 'href="play.html?new=1"').replace('href="/Game/Play"', 'href="play.html"')
    t = t.replace('href="/"', 'href="index.html"')
    bad = re.findall(r'(?:href|src|action)="/[^"]*"', t)
    if bad: raise SystemExit('build.py: %s còn đường dẫn tuyệt đối: %s' % (name, bad[:5]))
    return t


home = html(home, 'index.html')
play = html(play, 'play.html')
# .NET khởi động ngay khi mở màn chơi, song song với việc tải hình; game.js chờ nó ở lần gọi đầu
play = sub(play, '<script src="js/game.js', '<script src="_framework/blazor.webassembly.js" autostart="false"></script>\n'
           '    <script>window.__dotnet = Blazor.start();</script>\n    <script src="js/game.js', 1, 'play.html')
write(os.path.join(DIST, 'index.html'), home)
write(os.path.join(DIST, 'play.html'), play)

for f in os.listdir(os.path.join(DIST, 'css')):
    p = os.path.join(DIST, 'css', f); t = read(p)
    write(p, t.replace('url("/assets/', 'url("../assets/'))

js = os.path.join(DIST, 'js')
p = os.path.join(js, 'game.js'); t = read(p)
m = re.search(r"const r = await fetch\('/Game/' \+ action, \{.*?return await r\.json\(\);", t, flags=re.S)
if not m: raise SystemExit('build.py: không tìm thấy hàm api() trong game.js')
t = t[:m.start()] + ("await window.__dotnet;   // bản tĩnh: bộ máy kịch bản chạy trong trình duyệt, không gọi máy chủ\n"
                     "      return JSON.parse(await DotNet.invokeMethodAsync('AethelgardStatic', 'Call', action, JSON.stringify(body || {})));") + t[m.end():]
t = sub(t, "fetch('/Game/Manifest')", "fetch('manifest.json')", 1, 'game.js')
t = sub(t, "m.href = '/';", "m.href = 'index.html';", 3, 'game.js')
t = sub(t, "location.href = '/';", "location.href = 'index.html';", 1, 'game.js')
write(p, t)
p = os.path.join(js, 'home.js'); t = read(p)
t = sub(t, "fetch('/Game/Manifest')", "fetch('manifest.json')", 1, 'home.js')
write(p, t)
p = os.path.join(js, 'menu.js'); t = read(p)
t = sub(t, "location.href = '/Game/Play';", "location.href = 'play.html';", 1, 'menu.js')
write(p, t)
for f in os.listdir(js):
    p = os.path.join(js, f); t = read(p)
    t = t.replace("'/assets/", "'assets/").replace('url("/assets/', 'url("assets/').replace('`/assets/', '`assets/')
    bad = re.findall(r"""['"`(]/(?:assets|Game|css|js)/[^'"`)]*""", t)
    if bad: raise SystemExit('build.py: %s còn đường dẫn tuyệt đối: %s' % (f, bad[:5]))
    write(p, t)

# ── 3b. nén hình sang WebP (chỉ trong dist/, file gốc ở ../Game giữ nguyên). Bỏ qua bằng: python build.py --png
#     Nền: nén có mất mát, chất lượng 82. Sprite, bóng, hình cuối: màu chất lượng 88, kênh trong suốt giữ nguyên vẹn.
#     Bản đã nén được giữ ở obj/webp theo tên + cỡ + ngày sửa của file gốc, lần dựng sau không phải nén lại.
if '--png' not in sys.argv:
    from PIL import Image
    cache = os.path.join(HERE, 'obj', 'webp'); os.makedirs(cache, exist_ok=True)
    before = after = 0
    for folder, q in (('bg', 82), ('sprites', 88), ('ending', 88)):
        d = os.path.join(DIST, 'assets', folder)
        for f in sorted(os.listdir(d)):
            if not f.lower().endswith('.png'): continue
            src = os.path.join(d, f); st = os.stat(os.path.join(GAME, 'wwwroot', 'assets', folder, f))
            key = os.path.join(cache, '%s_%s_%d_%d_q%d.webp' % (folder, f[:-4], st.st_size, int(st.st_mtime), q))
            if not os.path.isfile(key):
                im = Image.open(src)
                im = im.convert('RGBA' if im.mode in ('RGBA', 'LA', 'P') else 'RGB')
                im.save(key, 'WEBP', quality=q, method=6, alpha_quality=100, exact=False)
            before += os.path.getsize(src); after += os.path.getsize(key)
            os.remove(src); shutil.copyfile(key, src[:-4] + '.webp')
    for sub_dir, names in (('js', os.listdir(js)), ('css', os.listdir(os.path.join(DIST, 'css')))):
        for f in names:
            p = os.path.join(DIST, sub_dir, f); t = read(p)
            write(p, re.sub(r"""(assets/(?:bg|sprites|ending)/[^"'`)\s]*?)\.png""", r"\1.webp", t))
    for k in ('bg', 'sprites', 'ending'):
        manifest[k] = sorted(os.listdir(os.path.join(DIST, 'assets', k)))
    print('nén hình: %.0f MB -> %.0f MB' % (before / 1e6, after / 1e6))

# ── 4. manifest: như /Game/Manifest; dấu phiên bản lấy theo nội dung thư mục sprite
h = hashlib.sha1()
for f in sorted(os.listdir(os.path.join(DIST, 'assets', 'sprites'))):
    h.update(f.encode()); h.update(str(os.path.getsize(os.path.join(DIST, 'assets', 'sprites', f))).encode())
manifest['v'] = h.hexdigest()[:12]
write(os.path.join(DIST, 'manifest.json'), json.dumps(manifest, ensure_ascii=False))
# git không được đổi kiểu xuống dòng của các file .NET: Blazor kiểm tra mã băm từng file khi tải
write(os.path.join(DIST, '.gitattributes'), '* -text\n')

# bản nén sẵn .br/.gz của .NET: nơi lưu trang tĩnh (Vercel, itch.io...) tự nén khi phát, giữ lại chỉ nặng thêm gói đưa lên
for d, _, fs in os.walk(os.path.join(DIST, '_framework')):
    for f in fs:
        if f.endswith('.br') or f.endswith('.gz'): os.remove(os.path.join(d, f))
# cấu hình cho Vercel: hình và nhạc được trình duyệt giữ lại một ngày, khỏi tải lại mỗi lần mở game
write(os.path.join(DIST, 'vercel.json'), json.dumps({
    'cleanUrls': False, 'trailingSlash': False,
    'headers': [{'source': '/assets/(.*)', 'headers': [{'key': 'Cache-Control', 'value': 'public, max-age=86400'}]}],
}, indent=2))

size = sum(os.path.getsize(os.path.join(d, f)) for d, _, fs in os.walk(DIST) for f in fs)
fw = sum(os.path.getsize(os.path.join(d, f)) for d, _, fs in os.walk(os.path.join(DIST, '_framework')) for f in fs)
print('xong: %s  (%.0f MB, trong đó .NET %.1f MB)' % (DIST, size / 1e6, fw / 1e6))
