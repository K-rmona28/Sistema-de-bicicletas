/** 'Trail X' → 'BT' (B + inicial del modelo, sin tildes) */
export function codePrefix(model: string): string {
  const letter = model.trim().normalize('NFD').replace(/[\u0300-\u036f]/g, '').charAt(0).toUpperCase();
  return /[A-Z]/.test(letter) ? `B${letter}` : '';
}

/** Siguiente código libre para un prefijo: BT-001, BT-002... */
export function nextCode(prefix: string, codes: string[]): string {
  const re = new RegExp(`^${prefix}-(\\d+)$`, 'i');
  const max = codes.reduce((m, c) => {
    const r = re.exec(c);
    return r ? Math.max(m, Number(r[1])) : m;
  }, 0);
  return `${prefix}-${String(max + 1).padStart(3, '0')}`;
}

/** Suma `step` al número final: bumpCode('BT-004', 2) → 'BT-006' */
export function bumpCode(code: string, step: number): string {
  const m = /^(.*?)(\d+)$/.exec(code);
  return m ? m[1] + String(Number(m[2]) + step).padStart(m[2].length, '0') : code;
}