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
    settings: () => Object.assign({ bgm: 0.6, se: 0.8, cps: 45 }, get('aeth_settings') || {}),
    saveSettings: s => set('aeth_settings', s),
  };
})();
