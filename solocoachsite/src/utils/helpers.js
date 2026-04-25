export function ensureArray(value) {
  return Array.isArray(value) ? value : []
}

export function getValue(obj, key) {
  return obj?.[key]
}

export function toNullableNumber(value) {
  const num = parseFloat(value)
  return Number.isFinite(num) ? num : null
}

export function formatDateTime(value) {
  if (!value) {
    return '—'
  }

  try {
    return new Date(value).toLocaleString('ru-RU')
  } catch {
    return String(value)
  }
}

export function countBy(items, keyFn) {
  const counts = new Map()
  for (const item of items) {
    const key = keyFn(item)
    counts.set(key, (counts.get(key) || 0) + 1)
  }
  return Object.fromEntries(counts)
}
