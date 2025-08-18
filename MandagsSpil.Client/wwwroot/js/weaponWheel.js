// wwwroot/js/weaponWheel.js (ES module)
let canvas, ctx;
let slices = [];              // [{ name, imageUrl, isSpecial, specialType, color }]
let images = [];              // [HTMLImageElement|null]
let anglePerSlice = 0;
let rotation = 0;
let spinning = false;
let lastTickIndex = -1;
let clickAudio = null;
let dingAudio = null;

export function init(canvasEl, initSlices, options = {}) {
  canvas = canvasEl;
  ctx = canvas.getContext('2d');
  setSlices(initSlices);
  if (options.clickSoundUrl) clickAudio = new Audio(options.clickSoundUrl);
  if (options.dingSoundUrl)  dingAudio  = new Audio(options.dingSoundUrl);
  drawWheel(0);
}

export function setSlices(newSlices) {
  slices = newSlices || [];
  anglePerSlice = (2 * Math.PI) / slices.length;
  // Preload images
  images = slices.map(s => {
    if (!s.imageUrl) return null;
    const img = new Image();
    img.src = s.imageUrl;
    return img;
  });
}

export function spinRandom(opts = {}) {
  const targetIndex = Math.floor(Math.random() * slices.length);
  return spinToIndex(targetIndex, opts);
}

export function spinToIndex(targetIndex, opts = {}) {
  if (spinning || !slices.length) return Promise.resolve(-1);
  spinning = true;
  lastTickIndex = -1;

  const spins = opts.spins ?? 6;               // full rotations
  const duration = opts.duration ?? 3000;      // ms
  const startRotation = rotation;

  // Make the target slice center align with the top pointer (-PI/2)
  const targetAngleCenter = -Math.PI / 2;
  const targetSliceCenterAngle = (targetIndex + 0.5) * anglePerSlice;
  // delta to move from 0 to target center, then add whole spins, and start from current rotation
  const finalRotation = startRotation + (Math.PI * 2 * spins) + (targetAngleCenter - targetSliceCenterAngle);

  let startTime = null;

  return new Promise(resolve => {
    function animate(t) {
      if (!startTime) startTime = t;
      const elapsed = t - startTime;
      const progress = Math.min(elapsed / duration, 1);
      // cubic ease-out
      const eased = 1 - Math.pow(1 - progress, 3);
      rotation = startRotation + (finalRotation - startRotation) * eased;

      // Tick sound when pointer crosses a new slice
      const pointerIndex = getPointerIndex(rotation);
      if (pointerIndex !== lastTickIndex) {
        if (clickAudio) {
          try { clickAudio.currentTime = 0; clickAudio.play(); } catch { /* ignore */ }
        }
        lastTickIndex = pointerIndex;
      }

      drawWheel(rotation);

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        // normalize rotation
        rotation = ((rotation % (2 * Math.PI)) + 2 * Math.PI) % (2 * Math.PI);
        drawWheel(rotation);
        if (dingAudio) { try { dingAudio.play(); } catch { /* ignore */ } }
        spinning = false;
        resolve(pointerIndex);
      }
    }
    requestAnimationFrame(animate);
  });
}

function getPointerIndex(rot) {
  // Pointer is at -PI/2 (top). Which slice center is under the pointer?
  // Compute the slice index whose center (i+0.5)*anglePerSlice aligns with pointer angle after rotation.
  // Solve: (i + 0.5) * a + rot ~= -PI/2  => i = ( -PI/2 - rot ) / a - 0.5
  const i = Math.floor((((-Math.PI / 2) - rot) / anglePerSlice) - 0.5);
  const n = slices.length;
  // Proper modulo for negatives
  return ((i % n) + n) % n;
}

function drawWheel(rot) {
  if (!ctx) return;
  const w = canvas.width, h = canvas.height;
  const cx = w / 2, cy = h / 2;
  const radius = Math.min(cx, cy) - 8;

  ctx.clearRect(0, 0, w, h);

  // Draw slices
  for (let i = 0; i < slices.length; i++) {
    const start = rot + i * anglePerSlice;
    const end = start + anglePerSlice;

    // wedge
    ctx.beginPath();
    ctx.moveTo(cx, cy);
    ctx.arc(cx, cy, radius, start, end);
    ctx.closePath();

    const col = slices[i].color || `hsl(${(i * 360) / slices.length},70%,50%)`;
    ctx.fillStyle = col;
    ctx.fill();

    // subtle stroke
    ctx.strokeStyle = "rgba(0,0,0,0.2)";
    ctx.lineWidth = 1;
    ctx.stroke();

    // content (icon or text)
    const mid = start + anglePerSlice / 2;
    const iconR = radius * 0.62;
    const ix = cx + Math.cos(mid) * iconR;
    const iy = cy + Math.sin(mid) * iconR;

    const img = images[i];
    if (img && img.complete) {
      const size = Math.min(48, radius * 0.28);
      ctx.save();
      ctx.translate(ix, iy);
      ctx.rotate(mid + Math.PI / 2); // upright appearance
      ctx.drawImage(img, -size / 2, -size / 2, size, size);
      ctx.restore();
    } else {
      // fallback: a small disk + initial
      const size = Math.min(16, radius * 0.08);
      ctx.beginPath();
      ctx.arc(ix, iy, size, 0, Math.PI * 2);
      ctx.fillStyle = "rgba(0,0,0,0.55)";
      ctx.fill();

      ctx.save();
      ctx.translate(ix, iy);
      ctx.rotate(mid + Math.PI / 2);
      ctx.fillStyle = "white";
      ctx.font = "bold 12px system-ui, Arial";
      ctx.textAlign = "center";
      ctx.textBaseline = "middle";
      const ch = (slices[i].name || "?").charAt(0).toUpperCase();
      ctx.fillText(ch, 0, 0);
      ctx.restore();
    }

    // label (optional small text nearer center)
    const labelR = radius * 0.40;
    const lx = cx + Math.cos(mid) * labelR;
    const ly = cy + Math.sin(mid) * labelR;
    ctx.save();
    ctx.translate(lx, ly);
    ctx.rotate(mid + Math.PI / 2);
    ctx.fillStyle = "white";
    ctx.font = "11px system-ui, Arial";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    const txt = slices[i].name ?? "";
    const clipped = txt.length > 16 ? (txt.slice(0, 14) + "…") : txt;
    // text shadow
    ctx.strokeStyle = "rgba(0,0,0,0.35)";
    ctx.lineWidth = 3;
    ctx.strokeText(clipped, 0, 0);
    ctx.fillText(clipped, 0, 0);
    ctx.restore();
  }

  // center hub
  ctx.beginPath();
  ctx.arc(cx, cy, Math.max(26, radius * 0.12), 0, Math.PI * 2);
  ctx.fillStyle = "#fff";
  ctx.fill();
  ctx.lineWidth = 2;
  ctx.strokeStyle = "rgba(0,0,0,0.25)";
  ctx.stroke();

  // top pointer
  const ptW = 18, ptH = 22;
  ctx.save();
  ctx.translate(cx, cy);
  ctx.beginPath();
  ctx.moveTo(0, -(radius + 6));
  ctx.lineTo(ptW / 2, -(radius - ptH));
  ctx.lineTo(-ptW / 2, -(radius - ptH));
  ctx.closePath();
  ctx.fillStyle = "#e74c3c";
  ctx.fill();
  ctx.lineWidth = 2;
  ctx.strokeStyle = "rgba(0,0,0,0.35)";
  ctx.stroke();
  ctx.restore();

  // outer rim
  ctx.beginPath();
  ctx.arc(cx, cy, radius, 0, Math.PI * 2);
  ctx.lineWidth = 4;
  ctx.strokeStyle = "rgba(255,255,255,0.65)";
  ctx.stroke();
}
