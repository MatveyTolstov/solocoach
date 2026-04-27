import { API_BASE_URL } from './auth.js'

export async function apiRequest(path, options = {}) {
  const { token, method = 'GET', query, body } = options
  
  const fullPath = path.startsWith('/') ? path : `/${path}`
  const url = new URL(API_BASE_URL + fullPath)
  
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        url.searchParams.set(key, String(value))
      }
    }
  }
  const headers = {}
  if (token) {
    headers.Authorization = `Bearer ${token}`
  }
  if (body) {
    headers['Content-Type'] = 'application/json'
  }
  const response = await fetch(url, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  })
  if (!response.ok) {
    const text = await response.text()
    throw new Error(text || `HTTP ${response.status}`)
  }
  if (response.status === 204) return null
  
  const contentType = response.headers.get('content-type')
  if (!contentType || !contentType.includes('application/json')) {
    const text = await response.text()
    return text ? text : null
  }
  const text = await response.text()
  if (!text || text.trim() === '') return null
  return JSON.parse(text)
}