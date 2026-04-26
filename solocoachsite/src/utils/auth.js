export const TOKEN_STORAGE_KEY = 'solocoach_manager_portal_token'
export const DEFAULT_USER_ROLE_ID = 2

const USER_ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
const LOGIN_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

// Получаем базовый URL API из переменной окружения или используем fallback
const getApiBaseUrl = () => {
  const fromEnv = import.meta.env.VITE_API_BASE_URL
  if (fromEnv && fromEnv.trim()) {
    return fromEnv.trim()
  }
  
  // Fallback для локальной разработки
  if (import.meta.env.DEV) {
    // Если API на том же хосте, но на другом порту (5219 для .NET)
    const protocol = window.location.protocol
    const hostname = window.location.hostname
    return `${protocol}//${hostname}:5219`
  }
  
  // Для production - API на том же хосте
  return window.location.origin
}

export const API_BASE_URL = getApiBaseUrl()

console.log('API_BASE_URL:', API_BASE_URL)

export function readStoredToken() {
  try {
    return localStorage.getItem(TOKEN_STORAGE_KEY) || ''
  } catch {
    return ''
  }
}

export function writeStoredToken(token) {
  try {
    localStorage.setItem(TOKEN_STORAGE_KEY, token)
  } catch {
    // ignore
  }
}

export function clearStoredToken() {
  try {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
  } catch {
    // ignore
  }
}

export function parseJwtSession(token) {
  if (!token) {
    return null
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]))

    const userId = parseInt(payload[USER_ID_CLAIM], 10)
    const login = payload[LOGIN_CLAIM]
    const roleId = parseInt(payload[ROLE_CLAIM], 10)

    if (!Number.isFinite(userId) || !login || !Number.isFinite(roleId)) {
      return null
    }

    return { userId, login, roleId }
  } catch {
    return null
  }
}

export function resolveRoleAccess(roleId, roleName) {
  const normalized = (roleName || '').toLowerCase()

  console.log(roleId)
  
  // Role 1 (Admin) - только админка
  const isAdmin = roleId === 1 || normalized.includes('admin') || normalized.includes('админ')
  
  // Role 3 (Manager) - только менеджер кабинет
  const isManager = roleId === 3 || normalized.includes('manager') || normalized.includes('менеджер') || normalized.includes('менедж')
  
  // Исключаем админов из менеджерского кабинета
  return { 
    admin: isAdmin, 
    manager: isManager && !isAdmin  // Админ НЕ может в менеджер!
  }
}
