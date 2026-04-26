import { API_BASE_URL } from './auth.js'

export async function apiRequest(path, options = {}) {
  const { token, method = 'GET', query, body } = options
  
  // Если API_BASE_URL пустой, используем текущий origin
  const baseUrl = "http://localhost:5219"
  // const baseUrl = "http://localhost:8081"

  const fullPath = path.startsWith('/') ? path : `/${path}`
  const urlString = baseUrl + fullPath
  
  const url = new URL(urlString)

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

  // Для 204 No Content возвращаем null
  if (response.status === 204) {
    return null
  }

  // Проверяем наличие JSON контента
  const contentType = response.headers.get('content-type')
  if (!contentType || !contentType.includes('application/json')) {
    // Если не JSON, проверяем есть ли вообще контент
    const text = await response.text()
    return text ? text : null
  }

  // Пытаемся распарсить JSON, если контент-тип JSON
  const text = await response.text()
  if (!text || text.trim() === '') {
    return null
  }

  return JSON.parse(text)
}
