import { useEffect, useMemo, useState } from 'react'
import './App.css'
import {
  readStoredToken,
  writeStoredToken,
  clearStoredToken,
  parseJwtSession,
  resolveRoleAccess,
} from './utils/auth.js'
import { apiRequest } from './utils/api.js'
import { ensureArray, getValue } from './utils/helpers.js'
import { ENTITY_CONFIGS, MANAGER_RESOURCE_KEYS } from './constants/config.js'
import LoginScreen from './components/LoginScreen.jsx'
import ManagerWorkspace from './pages/ManagerWorkspace.jsx'
import AdminWorkspace from './pages/AdminWorkspace.jsx'

function toNullableNumber(value) {
  if (value === null || value === undefined || value === '') return null
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : null
}

const MANAGER_TABS = [
  { key: 'analytics', label: 'Аналитика' },
  ...MANAGER_RESOURCE_KEYS.map((key) => ({ key, label: ENTITY_CONFIGS[key].title })),
]

const ADMIN_TABS = [
  { key: 'logs',     label: 'Логи' },
  { key: 'users',    label: 'Пользователи' },
  { key: 'database', label: 'База данных' },
  { key: 'backup',   label: 'Бекап' },
]

function App() {
  const [token, setToken] = useState(() => readStoredToken())
  const [roles, setRoles] = useState([])
  const [rolesLoading, setRolesLoading] = useState(false)
  const [authBusy, setAuthBusy] = useState(false)
  const [authError, setAuthError] = useState('')
  const [workspace, setWorkspace] = useState('manager')
  const [activeTab, setActiveTab] = useState(MANAGER_TABS[0].key)

  const session = useMemo(() => parseJwtSession(token), [token])

  useEffect(() => {
    if (!token || !session) { setRoles([]); return }
    let cancelled = false
    setRolesLoading(true)
    apiRequest('/api/Role', { token })
      .then((result) => { if (!cancelled) setRoles(ensureArray(result)) })
      .catch(() => { if (!cancelled) setRoles([]) })
      .finally(() => { if (!cancelled) setRolesLoading(false) })
    return () => { cancelled = true }
  }, [token, session])

  const currentRoleName = useMemo(() => {
    if (!session) return ''
    const role = roles.find(
      (item) => toNullableNumber(getValue(item, 'idRole')) === session.roleId,
    )
    return role ? String(getValue(role, 'roleName') ?? '') : `Role ${session.roleId}`
  }, [roles, session])

  const access = useMemo(() => {
    if (!session) return { admin: false, manager: false }
    return resolveRoleAccess(session.roleId, currentRoleName)
  }, [session, currentRoleName])

  const workspaceTabs = useMemo(() => {
    const tabs = []
    if (access.manager) tabs.push({ key: 'manager', label: 'Менеджер' })
    if (access.admin)   tabs.push({ key: 'admin',   label: 'Администратор' })
    return tabs
  }, [access])

  // Correct active workspace when access changes
  useEffect(() => {
    if (!workspaceTabs.length) return
    if (!workspaceTabs.some((t) => t.key === workspace)) {
      setWorkspace(workspaceTabs[0].key)
    }
  }, [workspaceTabs, workspace])

  // Reset active tab when workspace switches
  useEffect(() => {
    setActiveTab(workspace === 'manager' ? MANAGER_TABS[0].key : ADMIN_TABS[0].key)
  }, [workspace])

  const currentTabs = workspace === 'manager' ? MANAGER_TABS : ADMIN_TABS

  const handleLogin = async (login, password) => {
    setAuthBusy(true)
    setAuthError('')
    try {
      const result = await apiRequest('/api/auth/login', {
        method: 'POST',
        query: { login, password },
      })
      let nextToken = String(result?.token ?? result?.data?.token ?? '')
      if (!nextToken && typeof result === 'string') nextToken = result
      if (!nextToken) throw new Error('Сервер не вернул JWT токен.')
      const parsed = parseJwtSession(nextToken)
      if (!parsed) throw new Error('Получен некорректный или просроченный токен.')
      writeStoredToken(nextToken)
      setToken(nextToken)
    } catch (error) {
      setAuthError(error.message)
    } finally {
      setAuthBusy(false)
    }
  }

  const handleLogout = (message = '') => {
    clearStoredToken()
    setToken('')
    setRoles([])
    setWorkspace('manager')
    setActiveTab(MANAGER_TABS[0].key)
    setAuthError(message)
  }

  if (!token || !session) {
    return (
      <LoginScreen
        onLogin={handleLogin}
        isBusy={authBusy}
        error={authError || (token && !session ? 'Сессия истекла. Войдите снова.' : '')}
      />
    )
  }

  if (!access.manager && !access.admin) {
    return (
      <div className="login-page">
        <div className="login-card" style={{ textAlign: 'center' }}>
          <div className="login-logo">
            <div className="login-logo-mark">SC</div>
            <span className="login-title">SoloCoach</span>
          </div>
          <p className="login-subtitle" style={{ marginBottom: 8 }}>Панель управления</p>
          <div className="error-banner" style={{ textAlign: 'left', marginBottom: 20 }}>
            У вас нет доступа к панели управления.<br />
            Эта панель предназначена только для менеджеров и администраторов.
          </div>
          <button className="btn btn-outline" style={{ width: '100%', justifyContent: 'center' }} onClick={() => handleLogout()}>
            Выйти
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="app">
      <aside className="sidebar">
        <div className="sidebar-logo">
          <div className="logo-mark">SC</div>
          <span className="logo-text">SoloCoach</span>
        </div>

        {/* Workspace switcher — только если есть оба доступа */}
        {workspaceTabs.length > 1 && (
          <div className="sidebar-section">
            {workspaceTabs.map((tab) => (
              <button
                key={tab.key}
                type="button"
                className={`nav-item nav-item--workspace ${workspace === tab.key ? 'nav-item--active' : ''}`}
                onClick={() => setWorkspace(tab.key)}
              >
                <span className="nav-dot" />
                {tab.label}
              </button>
            ))}
          </div>
        )}

        {/* Sub-navigation */}
        <div className="sidebar-section sidebar-section--nav">
          {currentTabs.map((tab) => (
            <button
              key={tab.key}
              type="button"
              className={`nav-item ${activeTab === tab.key ? 'nav-item--active' : ''}`}
              onClick={() => setActiveTab(tab.key)}
            >
              <span className="nav-dot" />
              {tab.label}
            </button>
          ))}
        </div>

        <div className="sidebar-spacer" />

        <div className="sidebar-footer">
          <div className="user-card">
            <div className="user-avatar">
              {session.login.charAt(0).toUpperCase()}
            </div>
            <div>
              <div className="user-name">{session.login}</div>
              <div className="user-role">
                {rolesLoading ? 'загрузка...' : currentRoleName}
              </div>
            </div>
          </div>
          <button className="btn-logout" onClick={() => handleLogout()}>
            Выйти
          </button>
        </div>
      </aside>

      <div className="main-area">
        <header className="topbar">
          <span className="topbar-title">
            {currentTabs.find((t) => t.key === activeTab)?.label ?? ''}
          </span>
          <div className="topbar-actions">
            <span className={`badge ${access.admin ? 'badge-admin' : 'badge-manager'}`}>
              {access.admin ? 'Администратор' : 'Менеджер'}
            </span>
          </div>
        </header>

        <main className="content">
          {workspace === 'manager' ? (
            <ManagerWorkspace token={token} activeTab={activeTab} />
          ) : (
            <AdminWorkspace token={token} roles={roles} activeTab={activeTab} />
          )}
        </main>
      </div>
    </div>
  )
}

export default App
