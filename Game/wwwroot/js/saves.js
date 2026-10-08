/* Lưu/tải ván bằng localStorage. Mỗi bản lưu = trạng thái đầu loạt dòng + vị trí trong loạt đó (máy chủ chạy lại loạt là ra đúng màn hình). */
const Saves = (() => {
  const SLOTS = 8;
  const k = n => 'aeth_save_' + n;
  function get(key) { try { const v = localStorage.getItem(key); return v ? JSON.parse(v) : null; } catch { return null; } }
  function set(key, val) { try { localStorage.setItem(key, JSON.stringify(val)); return true; } catch { return false; } }
  return {
    SLOTS,
    slot: n => get(k(n)),
    saveSlot: (n, rec) => set(k(n), rec),
    auto: () => get('aeth_auto'),
    saveAuto: rec => set('aeth_auto', rec),
    clearAuto() { try { localStorage.removeItem('aeth_auto'); } catch { } },
    // đầu mỗi chương (để chọn lại chương đã qua)
    chapters: () => get('aeth_chapters') || [],
    saveChapter(rec) {
      const list = (get('aeth_chapters') || []).filter(c => c.title !== rec.title);
      list.push(rec);
      set('aeth_chapters', list);
    },
    // một ván cần mở ngay khi vào trang chơi (từ menu)
    setPending(rec) { try { sessionStorage.setItem('aeth_pending', JSON.stringify(rec)); } catch { } },
    takePending() {
      try { const v = sessionStorage.getItem('aeth_pending'); sessionStorage.removeItem('aeth_pending'); return v ? JSON.parse(v) : null; }
      catch { return null; }
    },
    // nhân vật đã gặp (tiền tố sprite), để màn menu hiện dần theo chặng đã chơi
    met: () => get('aeth_met') || [],
    meet(names) {
      const cur = get('aeth_met') || [];
      const add = names.filter(n => n && !cur.includes(n));
      if (add.length) set('aeth_met', cur.concat(add));
    },
    // kết cục đã xem: { "5": [{ title, lesson: [...] }, ...] }; một kết cục có hai lối thì giữ cả hai bài học đã thấy
    endings: () => get('aeth_endings') || {},
    seeEnding(title, lesson) {
      const m = /KẾT CỤC (\d)\/7/.exec(title || ''); if (!m) return;
      const all = get('aeth_endings') || {}, list = all[m[1]] || [];
      const key = (lesson || []).join('|');
      if (list.some(x => (x.lesson || []).join('|') === key)) return;
      list.push({ title, lesson }); all[m[1]] = list; set('aeth_endings', all);
    },
    // mảnh lưu trữ đã mở qua mọi lượt chơi: { "7": { n, title, paras } }; mở lại một mảnh thì giữ bản mới nhất (Mảnh 06 đổi theo lượt chơi)
    shards: () => get('aeth_shards') || {},
    seeShards(list) {
      if (!list || !list.length) return;
      const all = get('aeth_shards') || {};
      list.forEach(sh => { all[sh.n] = { n: sh.n, title: sh.title, paras: sh.paras }; });
      set('aeth_shards', all);
    },
    // màn hướng dẫn cách chơi đã xem (mỗi kiểu màn chỉ tự hiện một lần; nút "?" mở lại được)
    guideSeen: k => (get('aeth_guides') || []).includes(k),
    seeGuide(k) { const cur = get('aeth_guides') || []; if (!cur.includes(k)) set('aeth_guides', cur.concat([k])); },
    settings: () => Object.assign({ bgm: 0.6, se: 0.8, cps: 45 }, get('aeth_settings') || {}),
    saveSettings: s => set('aeth_settings', s),
  };
})();
