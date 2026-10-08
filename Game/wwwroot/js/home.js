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

  // ── nhân vật đã gặp: chưa chơi thì trống; chơi tới đâu, gặp ai thì người ấy ra đứng ở màn menu ──
  // Kael và những người đứng về phía anh ở bên phải, những người giữ thành phố ở bên trái; ai gặp trước đứng ngoài cùng.
  const SIDES = { R: ['Kael', 'Doran', 'Me', 'Rian'], L: ['Helena', 'Vane', 'Soren', 'Ilsa'] };
  const met = Saves.met(), castBox = $('heroCast');
  ['R', 'L'].forEach(side => {
    SIDES[side].filter(n => met.includes(n)).forEach((n, k) => {
      const im = document.createElement('img');
      im.alt = ''; im.className = 'c ' + (side === 'R' ? 'r' : 'l');
      im.style.setProperty('--k', k);
      im.style.zIndex = 20 - k;
      im.src = `/assets/sprites/${n}_Neutral.png`;
      castBox.appendChild(im);
    });
  });
  // A.L.I.C.E: cỗ máy trông coi cả thành phố, đứng chính giữa, cao hơn và ở sau mọi người (chữ tiêu đề che một phần cũng được)
  if (met.includes('ALICE')) {
    const im = document.createElement('img');
    im.alt = ''; im.className = 'c boss'; im.src = '/assets/sprites/ALICE_Neutral.png';
    castBox.appendChild(im);
  }
  if (met.includes('Veritas') && met.includes('Kael')) {
    const im = document.createElement('img');
    im.alt = ''; im.className = 'c holo'; im.src = '/assets/sprites/Veritas_Hologram.png?v=2';
    castBox.appendChild(im);
  }

  // ── liên kết điều hướng gọi lại các nút của menu.js ──
  document.querySelectorAll('[data-click]').forEach(a => a.addEventListener('click', e => {
    e.preventDefault(); const t = $(a.dataset.click); if (t && !t.hidden) t.click(); setMenu(false);
  }));
  const chap = document.querySelector('[data-click="btnChapters"]');
  const syncChap = () => document.querySelectorAll('[data-click="btnChapters"]').forEach(a => a.hidden = $('btnChapters').hidden);
  syncChap();

  // ── bảng bản lưu: đóng bằng Esc / bấm ra ngoài ──
  const slots = $('slots');
  document.addEventListener('keydown', e => { if (e.key === 'Escape') slots.hidden = true; });
  document.addEventListener('click', e => {
    if (!slots.hidden && !slots.contains(e.target) && !e.target.closest('[data-click],#btnLoad,#btnChapters,#btnEndings,#btnShards')) slots.hidden = true;
  });

  // ── kết cục đã xem: bảy ô, ô nào đã tới thì mở được bài học của nó (đi cả hai lối của một kết cục thì có cả hai) ──
  $('btnEndings').addEventListener('click', () => {
    const seen = Saves.endings();
    slots.innerHTML = ''; slots.hidden = false;
    const h = document.createElement('b');
    h.textContent = 'Kết cục đã xem: ' + Object.keys(seen).length + '/7';
    slots.appendChild(h);
    for (let n = 1; n <= 7; n++) {
      const list = seen[n] || [];
      if (!list.length) {
        const row = document.createElement('div'); row.className = 'slot ending-row locked';
        row.innerHTML = '<div class="info"><b></b><span>Chưa tới.</span></div>';
        row.querySelector('b').textContent = 'Kết cục ' + n + '/7';
        slots.appendChild(row); continue;
      }
      list.forEach((e, k) => {
        const d = document.createElement('details'); d.className = 'slot ending-row';
        const s = document.createElement('summary');
        s.textContent = e.title.replace('KẾT CỤC', 'Kết cục') + (list.length > 1 ? ' · lối ' + (k + 1) : '');
        d.appendChild(s);
        (e.lesson || []).forEach((t, i) => { const p = document.createElement('p'); p.textContent = t; if (i === 0) p.className = 'lesson-h'; d.appendChild(p); });
        slots.appendChild(d);
      });
    }
  });

  // ── mảnh lưu trữ đã mở, gộp qua mọi lượt chơi: mảnh nào đã mở thì đọc lại được; mảnh chưa mở không lộ tên ──
  let shardsTotal = 0;
  fetch('/Game/Manifest').then(r => r.json()).then(m => { shardsTotal = m.shardsTotal || 0; }).catch(() => { });
  $('btnShards').addEventListener('click', () => {
    const seen = Saves.shards();
    const total = Math.max(shardsTotal, ...Object.keys(seen).map(Number), 0);
    slots.innerHTML = ''; slots.hidden = false;
    const h = document.createElement('b');
    h.textContent = 'Mảnh lưu trữ đã mở: ' + Object.keys(seen).length + '/' + total;
    slots.appendChild(h);
    const note = document.createElement('p'); note.className = 'slots-note';
    note.textContent = 'Gộp qua mọi lượt chơi trên trình duyệt này. Có mảnh chỉ mở ở một nhánh, nên một lượt chơi không gom đủ được.';
    slots.appendChild(note);
    for (let n = 1; n <= total; n++) {
      const sh = seen[n], num = String(n).padStart(2, '0');
      if (!sh) {
        const row = document.createElement('div'); row.className = 'slot ending-row locked';
        row.innerHTML = '<div class="info"><b></b><span>Chưa mở.</span></div>';
        row.querySelector('b').textContent = 'Mảnh ' + num;
        slots.appendChild(row); continue;
      }
      const d = document.createElement('details'); d.className = 'slot ending-row';
      const s = document.createElement('summary'); s.textContent = 'Mảnh ' + num + ': “' + sh.title + '”'; d.appendChild(s);
      (sh.paras || []).forEach(p => {
        const e = document.createElement('p'); e.textContent = p.text;
        if (p.kind === 'head') e.className = 'lesson-h'; else if (p.kind === 'foot') e.className = 'foot';
        d.appendChild(e);
      });
      slots.appendChild(d);
    }
    slots.scrollTop = 0;
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
