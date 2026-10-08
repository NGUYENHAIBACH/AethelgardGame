(() => {
  const $ = id => document.getElementById(id);
  const auto = Saves.auto();
  if (auto) $('btnContinue').hidden = false;

  $('btnNew').addEventListener('click', e => {
    if (auto && !confirm('Chơi mới sẽ thay bản tự động lưu hiện có (các ô lưu thủ công vẫn giữ). Tiếp tục?')) e.preventDefault();
  });

  const chapters = Saves.chapters();
  if (chapters.length > 1 || (chapters.length === 1 && auto)) $('btnChapters').hidden = false;

  function listSlots(title, rows) {
    const box = $('slots'); box.innerHTML = ''; box.hidden = false;
    const h = document.createElement('b'); h.textContent = title; box.appendChild(h);
    rows.forEach(r => {
      const row = document.createElement('div'); row.className = 'slot';
      row.innerHTML = '<div class="info"><b></b><span></span><em></em></div>';
      row.querySelector('b').textContent = r.label;
      row.querySelector('span').textContent = r.sub;
      row.querySelector('em').textContent = r.note || '';
      const b = document.createElement('button'); b.className = 'btn'; b.textContent = r.btn; b.disabled = !r.rec;
      b.onclick = () => { Saves.setPending(r.rec); location.href = '/Game/Play'; };
      row.appendChild(b); box.appendChild(row);
    });
  }

  $('btnLoad').addEventListener('click', () => {
    const rows = [];
    const a = Saves.auto();
    rows.push({ label: 'Tự động', sub: a ? (a.chapter || '') + ' · ' + new Date(a.time).toLocaleString('vi-VN') : 'Trống', note: a ? a.snippet : '', rec: a, btn: 'Tải' });
    for (let n = 1; n <= Saves.SLOTS; n++) {
      const s = Saves.slot(n);
      rows.push({ label: 'Ô ' + n, sub: s ? (s.chapter || '') + ' · ' + new Date(s.time).toLocaleString('vi-VN') : 'Trống', note: s ? s.snippet : '', rec: s, btn: 'Tải' });
    }
    listSlots('Tải bản lưu', rows);
  });

  $('btnChapters').addEventListener('click', () => {
    const rows = chapters.map(c => ({
      label: c.title, sub: 'Đầu chương · ' + new Date(c.time).toLocaleString('vi-VN'),
      rec: { startState: c.startState, idx: 0, chapter: c.title, snippet: '', log: [], time: c.time }, btn: 'Chơi từ đây',
    }));
    listSlots('Chọn chương đã qua (giữ các lựa chọn lúc đó)', rows);
  });
})();
