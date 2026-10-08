/* Phần chạy trong trình duyệt: phát từng dòng, nhận lựa chọn, sổ tay cổ tay, lưu/tải.
   Mọi luật truyện (điều kiện, điểm, kết cục) nằm ở máy chủ (GameController + Models/Game/GameEngine.cs). */
(() => {
  'use strict';
  const $ = id => document.getElementById(id);
  const E = {
    stage: $('stage'), bgA: $('bgA'), bgB: $('bgB'), elev: $('elev'), sprite: $('sprite'), spriteL: $('spriteL'),
    bars: $('bars'), fillL: $('fillL'), fillD: $('fillD'), lblL: $('lblL'),
    toasts: $('toasts'), wrist: $('wrist'), dot: $('wristDot'),
    dialog: $('dialog'), name: $('name'), text: $('text'), more: $('more'),
    choices: $('choices'), card: $('card'), cardK: $('cardK'), cardT: $('cardT'), ending: $('ending'),
    notebook: $('notebook'), nbBody: $('nbBody'), shardCount: $('shardCount'),
    shardView: $('shardView'), shardTitle: $('shardTitle'), shardBody: $('shardBody'),
    logPanel: $('logPanel'), logBody: $('logBody'),
    savePanel: $('savePanel'), saveBody: $('saveBody'), saveTitle: $('saveTitle'),
    setPanel: $('setPanel'), busy: $('busy'),
  };

  let manifest = { bg: [], sprites: [], bgm: [], se: [] };
  const cfg = Saves.settings();
  const dev = new URLSearchParams(location.search).has('dev');

  const S = {
    state: null, startState: null, steps: [], idx: 0, pause: null,
    nb: { items: [], notes: [], shards: [] },
    log: [], typing: null, waiting: false, auto: false, skip: false, autoTimer: 0,
    cardOpen: false, locked: false, lastSay: null, chapter: '',
    scene: { bg: null, sprite: null, elev: false, bars: false, lmax: 0, dmax: 0, l: 0, d: 0, who: 'Helena' },
    nbTab: 'items', bgFront: 'A', curBg: null,
  };

  // ───────────────────────────────────────── gọi máy chủ

  async function api(action, body) {
    E.busy.hidden = false;
    try {
      const r = await fetch('/Game/' + action, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body || {}),
      });
      if (!r.ok) throw new Error(await r.text());
      return await r.json();
    } finally { E.busy.hidden = true; }
  }

  // ───────────────────────────────────────── cảnh: nền, sprite, thanh

  const fileFor = (list, name) => list.find(f => f.replace(/\.[^.]+$/, '') === name);

  function setBg(name, instant) {
    if (name === S.curBg) return;
    S.curBg = name;
    castStale = true;
    const next = S.bgFront === 'A' ? E.bgB : E.bgA, prev = S.bgFront === 'A' ? E.bgA : E.bgB;
    S.bgFront = S.bgFront === 'A' ? 'B' : 'A';
    next.className = 'bg';
    next.style.backgroundImage = '';
    next.removeAttribute('data-name');
    if (name) {
      const f = fileFor(manifest.bg, name);
      if (f) next.style.backgroundImage = `url("/assets/bg/${f}")`;
      else { next.classList.add('fb', 'fb-' + name.slice(0, 4).toLowerCase()); if (dev) next.setAttribute('data-name', name + ' (nền tạm)'); }
    }
    next.style.transition = instant ? 'none' : '';
    prev.style.transition = instant ? 'none' : '';
    setTimeout(() => { next.classList.add('on'); prev.classList.remove('on'); }, 20);
  }

  // Hai nhân vật cùng hiện: Kael bên trái, người đối thoại bên phải; ai đang nói thì sáng, người kia tối đi.
  const cast = { L: null, R: null };
  let castStale = false;
  const charOf = n => (n || '').split('_')[0];

  function paintSlot(k, name) {
    const el = k === 'L' ? E.spriteL : E.sprite;
    const f = name && fileFor(manifest.sprites, name);
    if (!f) { el.dataset.base = ''; return; }
    const img = new Image();
    img.onload = () => {
      if (cast[k] !== name) return;
      el.src = img.src;
      el.dataset.base = 'sprite ' + (name.startsWith('Veritas') ? 'holo ' : '') + (name === 'Veritas_Glitch' ? 'glitch ' : '');
      updateFocus();
    };
    img.src = '/assets/sprites/' + f;
  }

  function setSprite(name) {
    S.scene.sprite = name;
    if (castStale) { cast.L = cast.R = null; castStale = false; }   // đổi nền = đổi cảnh: bỏ người cũ
    if (!name) cast.L = cast.R = null;
    else cast[charOf(name) === 'Kael' ? 'L' : 'R'] = name;
    paintSlot('L', cast.L); paintSlot('R', cast.R);
    updateFocus();
  }

  function updateFocus() {
    const nm = S.speaker || '';
    const both = !!(cast.L && cast.R);
    const match = k => { const c = charOf(cast[k]); return !!c && (nm === c || (nm === 'A.L.I.C.E' && c === 'ALICE')); };
    const voice = !!S.hideVoice && !!cast.R;   // chỉ có tiếng, không hiện người
    const anyMatch = !voice && (match('L') || match('R'));
    [['L', E.spriteL], ['R', E.sprite]].forEach(([k, el]) => {
      const on = !!cast[k] && !!el.dataset.base;
      el.className = (el.dataset.base || 'sprite ') + (on ? 'show ' : '')
        + (both ? (k === 'L' ? 'pos-l ' : 'pos-r ') : '')
        + (both && anyMatch && !match(k) ? 'dim ' : '')
        + (voice && k === 'R' ? 'voice ' : '');
    });
  }

  function setBars(on, lmax, dmax, who) {
    S.scene.bars = on;
    if (lmax) { S.scene.lmax = lmax; S.scene.dmax = dmax; }
    if (who) S.scene.who = who;
    E.bars.hidden = !on;
    E.lblL.textContent = 'LUNG LAY · ' + (S.scene.who || 'Helena').toUpperCase();
  }
  function setBarValues(l, d) {
    S.scene.l = l; S.scene.d = d;
    E.fillL.style.width = (S.scene.lmax ? Math.round(l / S.scene.lmax * 100) : 0) + '%';
    E.fillD.style.width = (S.scene.dmax ? Math.round(d / S.scene.dmax * 100) : 0) + '%';
  }

  function applyScene(sc) {
    setBg(sc.bg, true);
    setSprite(sc.sprite);
    E.elev.classList.toggle('on', !!sc.elev);
    setBars(!!sc.bars, sc.lmax, sc.dmax, sc.who);
    setBarValues(sc.l, sc.d);
    Sound.bgm(sc.bgm || null);
    S.chapter = sc.chapter || S.chapter;
  }

  // ───────────────────────────────────────── thông báo & sổ tay

  function toast(msg, teal) {
    const t = document.createElement('div');
    t.className = 'toast' + (teal ? ' teal' : '');
    t.textContent = msg;
    E.toasts.appendChild(t);
    setTimeout(() => t.classList.add('out'), 3000);
    setTimeout(() => t.remove(), 3600);
  }
  function blinkWrist(teal) {
    E.wrist.classList.remove('blink', 'tealpulse');
    void E.wrist.offsetWidth;
    E.wrist.classList.add(teal ? 'tealpulse' : 'blink');
    if (!teal) E.dot.classList.add('on');
  }

  function renderNotebook() {
    E.shardCount.textContent = S.nb.shards.length + '/…';
    document.querySelectorAll('#notebook .tabs button').forEach(b => b.classList.toggle('on', b.dataset.tab === S.nbTab));
    const body = E.nbBody; body.innerHTML = '';
    const empty = t => { const p = document.createElement('p'); p.className = 'empty'; p.textContent = t; body.appendChild(p); };
    if (S.nbTab === 'items') {
      if (!S.nb.items.length) return empty('Chưa có vật phẩm nào.');
      S.nb.items.forEach(it => {
        const d = document.createElement('div'); d.className = 'entry';
        const h = document.createElement('h4'); h.textContent = it.name; d.appendChild(h);
        (it.desc || '').split('\n\n').filter(Boolean).forEach(p => { const e = document.createElement('p'); e.textContent = p; d.appendChild(e); });
        body.appendChild(d);
      });
    } else if (S.nbTab === 'notes') {
      if (!S.nb.notes.length) return empty('Chưa có ghi chú nào.');
      const ul = document.createElement('ul'); ul.className = 'notes';
      S.nb.notes.forEach(n => { const li = document.createElement('li'); li.textContent = n; ul.appendChild(li); });
      body.appendChild(ul);
    } else {
      if (!S.nb.shards.length) return empty('Chưa có mảnh lưu trữ nào.');
      S.nb.shards.slice().sort((a, b) => a.n - b.n).forEach(sh => {
        const b = document.createElement('button'); b.className = 'shard-row';
        b.innerHTML = '<span class="n"></span><span class="t"></span>';
        b.querySelector('.n').textContent = String(sh.n).padStart(2, '0');
        b.querySelector('.t').textContent = sh.title;
        b.onclick = () => openShard(sh.n);
        body.appendChild(b);
      });
    }
  }

  // Mảnh mở thành một khung giữa màn hình; đóng khung thì thanh bên hiện lại ở danh sách mảnh.
  const shardOpen = () => !E.shardView.hidden;
  function openShard(n) {
    const sh = S.nb.shards.find(s => s.n === n);
    if (!sh) return;
    E.shardTitle.textContent = 'Mảnh ' + String(sh.n).padStart(2, '0') + ': “' + sh.title + '”';
    const body = E.shardBody; body.innerHTML = ''; body.scrollTop = 0;
    sh.paras.forEach(p => {
      const e = document.createElement('p');
      e.textContent = p.text;
      if (p.kind === 'head') e.className = 'stance-head' + (p.hi ? ' hi' : '');
      else if (p.kind === 'foot') e.className = 'foot';
      else if (p.hi === true) e.className = 'hi-body';
      else if (p.hi === false) e.className = 'lo-body';
      body.appendChild(e);
    });
    closePanels();
    E.shardView.hidden = false;
  }
  function closeShard(noReopen) {
    if (!shardOpen()) return;
    E.shardView.hidden = true;
    if (!noReopen) { S.nbTab = 'shards'; openPanel('notebook'); }
  }

  // ───────────────────────────────────────── phát dòng

  const nameClass = n => ({ Kael: 'kael', Helena: 'helena', Veritas: 'veritas', Vane: 'vane', Rian: 'rian', 'A.L.I.C.E': 'alice', Doran: 'doran', 'Mẹ': 'me' }[n] || '');

  function logAdd(name, text) {
    S.log.push({ name, text });
    if (S.log.length > 300) S.log.shift();
  }

  function board(text) {
    const frag = document.createDocumentFragment();
    text.split('~~').forEach((part, i) => {
      const n = i % 2 ? document.createElement('s') : document.createElement('span');
      n.textContent = part; frag.appendChild(n);
    });
    return frag;
  }

  function showSay(st, instant) {
    clearTimeout(S.autoTimer);
    E.choices.hidden = true;
    E.dialog.hidden = false;
    S.lastSay = st;
    const kind = st.kind;
    E.dialog.className = 'dialog k-' + kind + (st.whisper ? ' whisper' : '') + (st.loud ? ' loud' : '');
    let nm = st.name || '';
    if (kind === 'loa') nm = 'A.L.I.C.E';
    E.name.textContent = nm;
    E.name.className = 'name ' + nameClass(nm);
    E.name.style.visibility = nm ? 'visible' : 'hidden';
    S.hideVoice = !!st.hs;
    S.speaker = nm;
    updateFocus();
    if (st.teal) blinkWrist(true);
    if (st.loud) { E.stage.classList.remove('shake'); void E.stage.offsetWidth; E.stage.classList.add('shake'); }

    const shown = kind === 'board' ? st.text.replace(/~~/g, '') : st.text;
    if (S.suppressLog) S.suppressLog = false; else logAdd(nm, shown);

    E.text.innerHTML = '';
    E.more.style.visibility = 'hidden';
    S.waiting = false;
    if (kind === 'board') { E.text.appendChild(board(st.text)); finishTyping(); return; }
    if (instant || S.skip) { E.text.textContent = st.text; finishTyping(); return; }
    // chữ hiện dần
    const full = st.text; let pos = 0, last = performance.now();
    const span = document.createElement('span'), rest = document.createElement('span');
    rest.className = 'rest'; rest.textContent = full;
    E.text.append(span, rest);
    S.typing = { full, finish() { span.textContent = full; rest.textContent = ''; } };
    (function tick(t) {
      if (!S.typing || S.typing.full !== full) return;
      pos = Math.min(full.length, pos + (t - last) / 1000 * cfg.cps); last = t;
      const p = Math.floor(pos); span.textContent = full.slice(0, p); rest.textContent = full.slice(p);
      if (p >= full.length) { S.typing = null; finishTyping(); } else setTimeout(() => tick(performance.now()), 16);
    })(last);
  }

  function finishTyping() {
    S.typing = null; S.waiting = true;
    E.more.style.visibility = 'visible';
    if (S.skip) S.autoTimer = setTimeout(advance, 22);
    else if (S.auto) S.autoTimer = setTimeout(advance, 900 + (S.lastSay ? S.lastSay.text.length : 0) * 38);
  }

  function skipTyping() {
    if (!S.typing) return false;
    S.typing.finish(); S.typing = null; finishTyping(); return true;
  }

  // ───────────────────────────────────────── chạy các bước

  function applyStep(st, silent) {
    switch (st.t) {
      case 'bg': setBg(st.v, silent); break;
      case 'bgm': Sound.bgm(st.v || null); break;
      case 'se': if (!silent) Sound.se(st.v); break;
      case 'spr': setSprite(st.v); break;
      case 'elev': E.elev.classList.toggle('on', !!st.on); break;
      case 'bars': setBars(!!st.on, st.lmax, st.dmax, st.who); break;
      case 'bar': setBarValues(st.l, st.d); break;
      case 'item':
        if (!S.nb.items.some(i => i.name === st.name)) S.nb.items.push({ name: st.name, desc: st.desc });
        if (!silent) { toast('Vật phẩm mới: ' + st.name); blinkWrist(); }
        break;
      case 'note':
        if (!S.nb.notes.includes(st.name)) S.nb.notes.push(st.name);
        if (!silent) { toast('Ghi chú mới: ' + st.name); blinkWrist(); }
        break;
      case 'shard':
        st.shards.forEach(sh => { if (!S.nb.shards.some(s => s.n === sh.n)) S.nb.shards.push(sh); });
        if (!silent) { toast('Mảnh lưu trữ mới'); blinkWrist(); }   // chỉ một thông báo dù mở nhiều mảnh cùng lúc
        break;
    }
    if (!silent && S.nbOpen) renderNotebook();
  }

  function pump() {
    while (true) {
      if (S.idx >= S.steps.length) return handlePause();
      const st = S.steps[S.idx++];
      if (st.t === 'say') { showSay(st); autosave(); return; }
      if (st.t === 'card') { showCard(st); return; }
      applyStep(st, false);
    }
  }

  function advance() {
    if (S.locked || S.cardOpen || shardOpen()) return;
    if (!E.choices.hidden || !E.ending.hidden) return;
    if (skipTyping()) return;
    if (!S.waiting) return;
    clearTimeout(S.autoTimer);
    S.waiting = false;
    pump();
  }

  function showCard(st) {
    S.cardOpen = true;
    const parts = st.text.split(': ');
    E.cardK.textContent = parts.length > 1 ? parts[0] : '';
    E.cardT.textContent = parts.length > 1 ? parts.slice(1).join(': ') : st.text;
    E.card.hidden = false; E.dialog.hidden = true;
    setTimeout(() => E.card.classList.add('on'), 20);
    if (S.idx === 1) Saves.saveChapter({ title: st.text, startState: S.startState, time: Date.now() });
    const done = () => {
      if (!S.cardOpen) return;
      S.cardOpen = false; E.card.classList.remove('on'); setTimeout(() => E.card.hidden = true, 500); pump();
    };
    S.cardDone = done;
    clearTimeout(S.cardTimer); S.cardTimer = setTimeout(done, S.skip ? 100 : 4200);
  }

  // ───────────────────────────────────────── điểm dừng: lựa chọn, kết cục, hết chương

  function handlePause() {
    const p = S.pause;
    autosave(true);
    if (p.type === 'choice' || p.type === 'battle') return showChoices(p);
    if (p.type === 'end') return showEnding(p);
    if (p.type === 'chapterEnd') return showChapterEnd(p);
    showChapterEnd({ title: S.chapter, hasNext: false });
  }

  function showChoices(p) {
    S.skip = false; $('btnSkip').classList.remove('on');
    clearTimeout(S.autoTimer);
    let opts = p.options.slice();
    if (p.type === 'battle') for (let i = opts.length - 1; i > 0; i--) { const j = Math.random() * (i + 1) | 0;[opts[i], opts[j]] = [opts[j], opts[i]]; }
    E.choices.innerHTML = '';
    opts.forEach(o => {
      const b = document.createElement('button');
      b.className = 'choice'; b.textContent = o.text;
      b.onclick = async () => {
        if (S.locked) return; S.locked = true;
        E.choices.hidden = true;
        if (p.type === 'choice') logAdd('Lựa chọn', o.text);
        try { const batch = await api('Choose', { state: S.state, index: o.i }); loadBatch(batch, 0); }
        catch (e) { alert('Lỗi: ' + e.message); E.choices.hidden = false; }
        finally { S.locked = false; }
      };
      E.choices.appendChild(b);
    });
    E.choices.hidden = false;
    E.more.style.visibility = 'hidden';
  }

  function showEnding(p) {
    S.auto = false; S.skip = false; $('btnAuto').classList.remove('on'); $('btnSkip').classList.remove('on');
    E.dialog.hidden = true; E.choices.hidden = true;
    const el = E.ending; el.innerHTML = '';
    const h = document.createElement('h2'); h.textContent = '━━ ' + p.title + ' ━━'; el.appendChild(h);
    const box = document.createElement('div'); box.className = 'lesson';
    p.lesson.forEach((t, i) => { const e = document.createElement('p'); e.textContent = t; if (i === 0) e.className = 'lesson-h'; box.appendChild(e); });
    el.appendChild(box);
    const row = document.createElement('div'); row.className = 'end-btns';
    const cap = document.createElement('span'); cap.textContent = 'Quay lại điểm chọn:'; row.appendChild(cap);
    p.retry.forEach(r => {
      const b = document.createElement('button'); b.className = 'btn'; b.textContent = r.label;
      b.onclick = async () => {
        try { const batch = await api('Retry', { state: S.state, id: r.id }); el.hidden = true; S.nbOpen = false; loadBatch(batch, 0); }
        catch (e) { alert('Lỗi: ' + e.message); }
      };
      row.appendChild(b);
    });
    const m = document.createElement('a'); m.className = 'btn ghost'; m.href = '/'; m.textContent = 'Về menu'; row.appendChild(m);
    el.appendChild(row);
    el.hidden = false; setTimeout(() => el.classList.add('on'), 20);
  }

  function showChapterEnd(p) {
    E.dialog.hidden = true;
    const el = E.ending; el.innerHTML = '';
    const h = document.createElement('h2');
    h.textContent = 'Hết ' + (p.title || '');
    el.appendChild(h);
    const row = document.createElement('div'); row.className = 'end-btns';
    if (p.hasNext) {
      const b = document.createElement('button'); b.className = 'btn primary'; b.textContent = 'Sang ' + (p.nextTitle || 'chương tiếp');
      b.onclick = async () => {
        try { const batch = await api('Continue', { state: S.state }); el.hidden = true; el.classList.remove('on'); loadBatch(batch, 0); }
        catch (e) { alert('Lỗi: ' + e.message); }
      };
      row.appendChild(b);
    } else {
      const note = document.createElement('p'); note.className = 'lesson-h';
      note.textContent = 'Đây là hết phần kịch bản hiện có. Chương tiếp theo sẽ có khi người viết giao kịch bản.';
      el.appendChild(note);
    }
    const m = document.createElement('a'); m.className = 'btn ghost'; m.href = '/'; m.textContent = 'Về menu'; row.appendChild(m);
    el.appendChild(row);
    el.hidden = false; setTimeout(() => el.classList.add('on'), 20);
  }

  // ───────────────────────────────────────── nạp một loạt dòng từ máy chủ

  function loadBatch(b, startIdx, fromSave) {
    S.state = b.state; S.startState = b.startState; S.steps = b.steps; S.pause = b.pause;
    S.nb = JSON.parse(JSON.stringify(b.notebook));
    S.idx = 0; S.waiting = false; S.typing = null; S.cardOpen = false;
    E.ending.hidden = true; E.ending.classList.remove('on'); E.choices.hidden = true;
    closeShard(true);
    applyScene(b.scene);
    renderNotebook();
    const n = Math.min(startIdx || 0, S.steps.length);
    // phát nhanh các bước đã qua (khi tải ván), không hiện thông báo, không ghi nhật ký (nhật ký lấy từ bản lưu)
    for (let k = 0; k < n; k++) {
      const st = S.steps[k];
      if (st.t === 'say') S.lastSay = st;
      else if (st.t !== 'card') applyStep(st, true);
    }
    S.idx = n;
    S.suppressLog = !!fromSave;   // dòng đang hiện đã nằm sẵn trong nhật ký đã lưu
    if (n > 0 && n >= S.steps.length && S.lastSay) showSay(S.lastSay, true);
    pump();
    S.suppressLog = false;
  }

  // ───────────────────────────────────────── lưu / tải

  function record() {
    return {
      startState: S.startState, idx: S.idx > 0 && !(S.idx >= S.steps.length) ? S.idx - 1 : S.steps.length,
      chapter: S.chapter, snippet: S.lastSay ? S.lastSay.text.slice(0, 70) : '', log: S.log.slice(-120), time: Date.now(),
    };
  }
  function autosave(atPause) {
    if (!S.startState) return;
    const r = record(); if (atPause) r.idx = S.steps.length;
    Saves.saveAuto(r);
  }

  async function loadRecord(r) {
    S.locked = true;
    try {
      const batch = await api('Resume', { state: r.startState });
      S.log = (r.log || []).slice();
      loadBatch(batch, r.idx, true);
    } catch (e) { alert('Không tải được: ' + e.message); }
    finally { S.locked = false; }
  }

  function openSavePanel(mode) {
    E.saveTitle.textContent = mode === 'save' ? 'Lưu ván' : 'Tải ván';
    E.saveBody.innerHTML = '';
    const mk = (label, rec, n) => {
      const row = document.createElement('div'); row.className = 'slot';
      const info = document.createElement('div'); info.className = 'info';
      info.innerHTML = '<b></b><span></span><em></em>';
      info.querySelector('b').textContent = label;
      info.querySelector('span').textContent = rec ? (rec.chapter || '') + (rec.time ? ' · ' + new Date(rec.time).toLocaleString('vi-VN') : '') : 'Trống';
      info.querySelector('em').textContent = rec ? rec.snippet : '';
      row.appendChild(info);
      const b = document.createElement('button'); b.className = 'btn';
      if (mode === 'save') {
        if (n == null) return;   // không lưu đè ô tự động
        b.textContent = 'Lưu vào đây';
        b.onclick = () => { if (Saves.saveSlot(n, record())) { openSavePanel('save'); toast('Đã lưu'); } else toast('Không lưu được'); };
      } else {
        b.textContent = 'Tải'; b.disabled = !rec;
        b.onclick = () => { closePanels(); loadRecord(rec); };
      }
      row.appendChild(b); E.saveBody.appendChild(row);
    };
    if (mode === 'load') mk('Tự động', Saves.auto(), null);
    for (let n = 1; n <= Saves.SLOTS; n++) mk('Ô ' + n, Saves.slot(n), n);
    openPanel('savePanel');
  }

  // ───────────────────────────────────────── bảng

  const PANELS = ['notebook', 'logPanel', 'savePanel', 'setPanel'];
  function openPanel(id) {
    PANELS.forEach(p => { if (p !== id) $(p).hidden = true; });
    $(id).hidden = false; S.nbOpen = id === 'notebook';
    if (id === 'notebook') { E.wrist.classList.remove('blink', 'tealpulse'); E.dot.classList.remove('on'); renderNotebook(); }
    if (id === 'logPanel') {
      E.logBody.innerHTML = '';
      S.log.forEach(l => {
        const p = document.createElement('p'); const nm = document.createElement('b');
        nm.textContent = l.name ? l.name + ': ' : ''; p.append(nm, document.createTextNode(l.text)); E.logBody.appendChild(p);
      });
      E.logBody.scrollTop = E.logBody.scrollHeight;
    }
  }
  function closePanels() { PANELS.forEach(p => $(p).hidden = true); S.nbOpen = false; }
  const anyPanel = () => PANELS.some(p => !$(p).hidden);

  function toggleAuto() {
    S.auto = !S.auto; $('btnAuto').classList.toggle('on', S.auto);
    if (S.auto) { S.skip = false; $('btnSkip').classList.remove('on'); if (S.waiting) finishTyping(); }
    else clearTimeout(S.autoTimer);
  }
  function setSkip(on) {
    S.skip = on; $('btnSkip').classList.toggle('on', on);
    if (on) { S.auto = false; $('btnAuto').classList.remove('on'); if (S.cardOpen && S.cardDone) S.cardDone(); else if (S.waiting) finishTyping(); else skipTyping(); }
    else clearTimeout(S.autoTimer);
  }

  // ───────────────────────────────────────── sự kiện

  document.addEventListener('pointerdown', () => Sound.unlock(), { once: false });

  E.stage.addEventListener('click', ev => {
    // xét theo đường đi của cú bấm lúc nó xảy ra: nút vừa bấm có thể đã bị vẽ lại (gỡ khỏi trang) trước khi tới đây
    const inUi = ev.composedPath().some(el => el.matches && el.matches('.toolbar, .panel, .choices, .wrist, .ending, .toasts, .shard-box'));
    if (inUi) return;
    if (shardOpen()) { closeShard(); return; }
    if (anyPanel()) { closePanels(); return; }
    if (S.cardOpen) { S.cardDone && S.cardDone(); return; }
    advance();
  });

  document.querySelectorAll('.toolbar button').forEach(b => b.addEventListener('click', () => {
    const a = b.dataset.act;
    if (a === 'log') openPanel('logPanel');
    else if (a === 'auto') toggleAuto();
    else if (a === 'skip') setSkip(!S.skip);
    else if (a === 'save') openSavePanel('save');
    else if (a === 'load') openSavePanel('load');
    else if (a === 'settings') openPanel('setPanel');
    else if (a === 'menu') { if (confirm('Về menu? Ván hiện tại đã được tự động lưu.')) { autosave(!E.choices.hidden); location.href = '/'; } }
  }));
  E.wrist.addEventListener('click', () => (E.notebook.hidden ? openPanel('notebook') : closePanels()));
  document.querySelectorAll('[data-close]').forEach(b => b.addEventListener('click', closePanels));
  document.querySelectorAll('#notebook .tabs button').forEach(b => b.addEventListener('click', () => { S.nbTab = b.dataset.tab; renderNotebook(); }));
  $('shardClose').addEventListener('click', () => closeShard());

  document.addEventListener('keydown', ev => {
    if (ev.target.closest && ev.target.closest('input, textarea')) return;
    if (ev.key === 'Control') { if (!S.skip) { S.holdSkip = true; setSkip(true); } return; }
    if (ev.key === ' ' || ev.key === 'Enter') { ev.preventDefault(); if (S.cardOpen) S.cardDone && S.cardDone(); else if (shardOpen()) closeShard(); else if (!anyPanel()) advance(); }
    else if (ev.key === 'Escape') { if (shardOpen()) closeShard(); else closePanels(); }
    else if (shardOpen()) return;
    else if (ev.key === 'l' || ev.key === 'L') openPanel('logPanel');
    else if (ev.key === 'n' || ev.key === 'N') (E.notebook.hidden ? openPanel('notebook') : closePanels());
    else if (ev.key === 'a' || ev.key === 'A') toggleAuto();
  });
  document.addEventListener('keyup', ev => { if (ev.key === 'Control' && S.holdSkip) { S.holdSkip = false; setSkip(false); } });

  // cài đặt
  const sb = $('setBgm'), ss = $('setSe'), sc = $('setCps');
  sb.value = cfg.bgm; ss.value = cfg.se; sc.value = cfg.cps;
  const applySettings = () => { cfg.bgm = +sb.value; cfg.se = +ss.value; cfg.cps = +sc.value; Saves.saveSettings(cfg); Sound.setSettings(cfg); };
  [sb, ss, sc].forEach(i => i.addEventListener('input', applySettings));

  // ───────────────────────────────────────── khởi động

  async function boot() {
    try { manifest = await (await fetch('/Game/Manifest')).json(); } catch { }
    Sound.init(manifest, cfg);
    E.dot.classList.remove('on');
    const params = new URLSearchParams(location.search);
    const pending = Saves.takePending();
    try {
      if (pending) { await loadRecord(pending); return; }
      const auto = Saves.auto();
      if (!params.has('new') && auto) { await loadRecord(auto); return; }
      Saves.clearAuto();
      const batch = await api('Start');
      loadBatch(batch, 0);
    } catch (e) {
      alert('Không bắt đầu được: ' + e.message);
    }
  }
  if (dev) window.__AETH = { S, advance, pump, setSkip, handlePause };   // chỉ để kiểm thử bằng ?dev=1
  boot();
})();
