/* Nhạc nền (mp3) và hiệu ứng (mp3 nếu có file, nếu chưa có thì tự tạo âm thay thế bằng WebAudio). */
const Sound = (() => {
  let manifest = { bgm: [], se: [] };
  let settings = { bgm: 0.6, se: 0.8 };
  let cur = null, curName = null, wanted = null, unlocked = false;
  let ctx = null;

  const find = (list, name) => list.find(f => f.replace(/\.[^.]+$/, '') === name);

  function ac() {
    if (!ctx) { try { ctx = new (window.AudioContext || window.webkitAudioContext)(); } catch { } }
    if (ctx && ctx.state === 'suspended') ctx.resume();
    return ctx;
  }

  function fade(el, to, ms, done) {
    const from = el.volume, t0 = performance.now();
    (function step(t) {
      const p = Math.min(1, (t - t0) / ms);
      el.volume = Math.max(0, Math.min(1, from + (to - from) * p));
      if (p < 1) requestAnimationFrame(step); else done && done();
    })(t0);
  }

  function startBgm(name) {
    const f = find(manifest.bgm, name);
    if (!f) return;
    const el = new Audio('/assets/bgm/' + f);
    el.loop = true; el.volume = 0;
    const p = el.play();
    const old = cur;
    cur = el; curName = name;
    const go = () => { fade(el, settings.bgm, 1200); if (old) fade(old, 0, 1200, () => old.pause()); };
    if (p && p.then) p.then(go).catch(() => { unlocked = false; cur = old; curName = null; });
    else go();
  }

  // ───── hiệu ứng thay thế
  function noise(c, dur) {
    const b = c.createBuffer(1, Math.max(1, c.sampleRate * dur | 0), c.sampleRate), d = b.getChannelData(0);
    for (let i = 0; i < d.length; i++) d[i] = Math.random() * 2 - 1;
    const s = c.createBufferSource(); s.buffer = b; return s;
  }
  function env(c, g, t0, a, peak, d) {
    g.gain.setValueAtTime(0.0001, t0);
    g.gain.exponentialRampToValueAtTime(Math.max(0.0002, peak), t0 + a);
    g.gain.exponentialRampToValueAtTime(0.0001, t0 + a + d);
  }
  const synth = {
    SE01(c, v) { // còi báo động trạm
      for (let i = 0; i < 3; i++) {
        const o = c.createOscillator(), g = c.createGain(), t = c.currentTime + i * 0.55;
        o.type = 'square'; o.frequency.setValueAtTime(520, t); o.frequency.linearRampToValueAtTime(760, t + 0.4);
        env(c, g, t, 0.03, 0.12 * v, 0.45); o.connect(g).connect(c.destination); o.start(t); o.stop(t + 0.5);
      }
    },
    SE02(c, v) { // gõ bàn phím
      for (let i = 0; i < 14; i++) {
        const t = c.currentTime + i * 0.085 + Math.random() * 0.03, s = noise(c, 0.03), g = c.createGain(), f = c.createBiquadFilter();
        f.type = 'highpass'; f.frequency.value = 2200; env(c, g, t, 0.002, 0.25 * v, 0.03);
        s.connect(f).connect(g).connect(c.destination); s.start(t);
      }
    },
    SE03(c, v) { // glitch hệ thống
      for (let i = 0; i < 9; i++) {
        const t = c.currentTime + i * 0.06, o = c.createOscillator(), g = c.createGain();
        o.type = i % 2 ? 'sawtooth' : 'square'; o.frequency.value = 80 + Math.random() * 1800;
        env(c, g, t, 0.004, 0.14 * v, 0.05); o.connect(g).connect(c.destination); o.start(t); o.stop(t + 0.07);
      }
      const s = noise(c, 0.5), g = c.createGain(); env(c, g, c.currentTime, 0.01, 0.1 * v, 0.45); s.connect(g).connect(c.destination); s.start();
    },
    SE04(c, v) { // nổ tháp năng lượng
      const t = c.currentTime;
      const s = noise(c, 1.8), f = c.createBiquadFilter(), g = c.createGain();
      f.type = 'lowpass'; f.frequency.setValueAtTime(1800, t); f.frequency.exponentialRampToValueAtTime(90, t + 1.6);
      env(c, g, t, 0.01, 0.9 * v, 1.7); s.connect(f).connect(g).connect(c.destination); s.start(t);
      const o = c.createOscillator(), g2 = c.createGain(); o.frequency.setValueAtTime(110, t); o.frequency.exponentialRampToValueAtTime(30, t + 1.2);
      env(c, g2, t, 0.01, 0.8 * v, 1.2); o.connect(g2).connect(c.destination); o.start(t); o.stop(t + 1.3);
    },
    SE05(c, v) { // xả hơi kim loại
      const t = c.currentTime, s = noise(c, 2.2), f = c.createBiquadFilter(), g = c.createGain();
      f.type = 'bandpass'; f.frequency.value = 3200; f.Q.value = 0.7;
      env(c, g, t, 0.25, 0.35 * v, 1.8); s.connect(f).connect(g).connect(c.destination); s.start(t);
      const o = c.createOscillator(), g2 = c.createGain(); o.type = 'triangle'; o.frequency.setValueAtTime(70, t); o.frequency.linearRampToValueAtTime(52, t + 2);
      env(c, g2, t, 0.3, 0.18 * v, 1.9); o.connect(g2).connect(c.destination); o.start(t); o.stop(t + 2.2);
    },
    SE06(c, v) { // cửa thép mở
      const t = c.currentTime;
      const o = c.createOscillator(), g = c.createGain(); o.type = 'sawtooth'; o.frequency.setValueAtTime(190, t); o.frequency.exponentialRampToValueAtTime(60, t + 0.9);
      env(c, g, t, 0.05, 0.16 * v, 0.9); o.connect(g).connect(c.destination); o.start(t); o.stop(t + 1);
      const s = noise(c, 0.25), g2 = c.createGain(), f = c.createBiquadFilter(); f.type = 'lowpass'; f.frequency.value = 600;
      env(c, g2, t + 0.95, 0.005, 0.5 * v, 0.2); s.connect(f).connect(g2).connect(c.destination); s.start(t + 0.95);
    },
  };

  return {
    init(m, s) { manifest = m; settings = Object.assign(settings, s); },
    setSettings(s) { settings = Object.assign(settings, s); if (cur) cur.volume = settings.bgm; },
    /** gọi trong lần bấm đầu tiên của người chơi (trình duyệt chặn tự phát nhạc trước đó) */
    unlock() {
      unlocked = true; ac();
      if (wanted && (!cur || curName !== wanted)) startBgm(wanted);
    },
    bgm(name) {
      wanted = name;
      if (!name) { if (cur) { const o = cur; fade(o, 0, 900, () => o.pause()); cur = null; curName = null; } return; }
      if (name === curName) return;
      if (unlocked) startBgm(name);
    },
    se(name) {
      const f = find(manifest.se, name);
      if (f) { const a = new Audio('/assets/se/' + f); a.volume = settings.se; a.play().catch(() => { }); return; }
      const c = ac(); const fn = synth[(name || '').slice(0, 4)];
      if (c && fn) fn(c, settings.se);
    },
  };
})();
