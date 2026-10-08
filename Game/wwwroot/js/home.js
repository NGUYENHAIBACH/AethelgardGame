(() => {
  const root = document.documentElement;
  const $ = id => document.getElementById(id);

  // ── nền luân phiên (các tấm BG có sẵn trong assets/bg) ──
  const BGS = ['BG01_ThanhPho_Aethelgard_Dem', 'BG08_Aethelgard_Dong_Bang', 'BG04_Hanh_Lang_Toi_Tang_Trung', 'BG09_Thanh_Pho_Tro_Tan',
    'BG02_Phong_Luu_Tru_Du_Lieu', 'BG07_Dai_Ban_Doanh_Khang_Chien', 'BG05_Khu_O_Chuot_Tang_Day', 'BG03_Thap_Nang_Luong_Sup_Do',
    'BG06_Duong_Ong_Ngam_Tang_Day', 'BG10_Tang_Day_No_Du_Nhung_U_Toi'];
  const box = $('bgs');
  const layers = BGS.map((n, i) => {
    const el = document.createElement('i');
    if (i === 0) el.style.backgroundImage = `url("/assets/bg/${n}.png")`;
    box.appendChild(el); return el;
  });
  let cur = 0; layers[0].classList.add('on');
  const preload = n => { const im = new Image(); im.src = `/assets/bg/${BGS[n]}.png`; };
  if (!matchMedia('(prefers-reduced-motion: reduce)').matches) {
    preload(1);
    setInterval(() => {
      const nx = (cur + 1) % BGS.length;
      if (!layers[nx].style.backgroundImage) layers[nx].style.backgroundImage = `url("/assets/bg/${BGS[nx]}.png")`;
      layers[cur].classList.remove('on'); layers[nx].classList.add('on'); cur = nx;
      preload((nx + 1) % BGS.length);
    }, 7000);
  }

  // ── liên kết điều hướng gọi lại các nút của menu.js ──
  document.querySelectorAll('[data-click]').forEach(a => a.addEventListener('click', e => {
    e.preventDefault(); const t = $(a.dataset.click); if (t && !t.hidden) t.click(); setMenu(false);
  }));
  const chap = document.querySelector('[data-click="btnChapters"]');
  const syncChap = () => document.querySelectorAll('[data-click="btnChapters"]').forEach(a => a.hidden = $('btnChapters').hidden);
  syncChap();

  // ── bảng ván lưu: đóng bằng Esc / bấm ra ngoài ──
  const slots = $('slots');
  document.addEventListener('keydown', e => { if (e.key === 'Escape') slots.hidden = true; });
  document.addEventListener('click', e => {
    if (!slots.hidden && !slots.contains(e.target) && !e.target.closest('[data-click],#btnLoad,#btnChapters')) slots.hidden = true;
  });

  // ── menu di động ──
  const bar = document.querySelector('.bar'), mb = $('menu-btn');
  function setMenu(open) {
    bar.dataset.open = open ? 'true' : 'false';
    mb.setAttribute('aria-expanded', open ? 'true' : 'false');
    mb.setAttribute('aria-label', open ? 'Đóng menu' : 'Menu');
  }
  setMenu(false);
  mb.addEventListener('click', e => { e.stopPropagation(); setMenu(bar.dataset.open !== 'true'); });
  document.addEventListener('click', e => { if (!bar.contains(e.target)) setMenu(false); });
  document.addEventListener('keydown', e => { if (e.key === 'Escape') setMenu(false); });

  // ── hoạt ảnh vào trang (chạy một lần) ──
  if (root.classList.contains('intro')) {
    let started = false;
    const done = () => { root.classList.remove('intro'); root.classList.remove('intro-play'); };
    const start = () => {
      if (started) return; started = true;
      root.classList.add('intro-play');
      const btns = document.querySelectorAll('.cta .lb:not([hidden])');
      const last = btns[btns.length - 1];
      if (last) last.addEventListener('animationend', e => { if (e.target === last) done(); });
      setTimeout(done, 3000);
    };
    const go = () => requestAnimationFrame(() => requestAnimationFrame(start));
    const t = setTimeout(go, 700);
    if (document.fonts && document.fonts.ready) document.fonts.ready.then(() => { clearTimeout(t); go(); });
  }
})();
