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
import LoginScreen from './components/LoginScreen.jsx'
import ManagerWorkspace from './pages/ManagerWorkspace.jsx'
import AdminWorkspace from './pages/AdminWorkspace.jsx'

function toNullableNumber(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : null
}

function App() {
  const [token, setToken] = useState(() => readStoredToken())
  const [roles, setRoles] = useState([])
  const [rolesLoading, setRolesLoading] = useState(false)
  const [rolesError, setRolesError] = useState('')
  const [authBusy, setAuthBusy] = useState(false)
  const [authError, setAuthError] = useState('')
  const [workspace, setWorkspace] = useState('manager')

  const session = useMemo(() => parseJwtSession(token), [token])

  useEffect(() => {
    if (!token || !session) {
      setRoles([])
      setRolesError('')
      return
    }

    let isCancelled = false
    const loadRoles = async () => {
      setRolesLoading(true)
      setRolesError('')
      try {
        const result = await apiRequest('/api/Role', { token })
        if (!isCancelled) {
          setRoles(ensureArray(result))
        }
      } catch (error) {
        if (!isCancelled) {
          setRoles([])
          setRolesError(error.message)
        }
      } finally {
        if (!isCancelled) {
          setRolesLoading(false)
        }
      }
    }

    loadRoles()

    return () => {
      isCancelled = true
    }
  }, [token, session])

  const currentRoleName = useMemo(() => {
    if (!session) {
      return ''
    }
    const role = roles.find(
      (item) => toNullableNumber(getValue(item, 'idRole')) === session.roleId,
    )
    if (!role) {
      return `Role ${session.roleId}`
    }
    return String(getValue(role, 'roleName') ?? '')
  }, [roles, session])

  const access = useMemo(() => {
    if (!session) {
      return { admin: false, manager: false }
    }
    return resolveRoleAccess(session.roleId, currentRoleName)
  }, [session, currentRoleName])

  const workspaceTabs = useMemo(() => {
    const tabs = []
    if (access.manager) {
      tabs.push({ key: 'manager', label: 'Кабинет менеджера' })
    }
    if (access.admin) {
      tabs.push({ key: 'admin', label: 'Кабинет администратора' })
    }
    return tabs
  }, [access])

  useEffect(() => {
    if (workspaceTabs.length === 0) {
      return
    }
    const exists = workspaceTabs.some((tab) => tab.key === workspace)
    if (!exists) {
      setWorkspace(workspaceTabs[0].key)
    }
  }, [workspaceTabs, workspace])

  const handleLogin = async (login, password) => {
    setAuthBusy(true)
    setAuthError('')
    try {
      const result = await apiRequest('/api/auth/login', {
        method: 'POST',
        query: { login, password },
      })

      console.log('Login response:', result)

      // Пытаемся получить токен несколькими способами
      let nextToken = String(result?.token ?? result?.data?.token ?? '')
      
      if (!nextToken) {
        // Может быть токен передан как строка?
        if (typeof result === 'string') {
          nextToken = result
        } else if (result) {
          console.error('Unexpected response structure:', result)
        }
      }

      if (!nextToken) {
        throw new Error('Сервер не вернул JWT токен.')
      }

      const parsed = parseJwtSession(nextToken)
      if (!parsed) {
        throw new Error('Получен некорректный или просроченный токен.')
      }

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
      <div className="shell">
        <header className="topbar">
          <div>
            <h1 className="topbar-title">SoloCoach Control Center</h1>
            <p className="topbar-subtitle">
              Недостаточно прав для доступа к менеджерскому или админскому разделу.
            </p>
          </div>
          <button
            type="button"
            className="btn btn-outline"
            onClick={() => handleLogout()}
          >
            Выйти
          </button>
        </header>
      </div>
    )
  }

  return (
    <div className="shell">
      <header className="topbar">
        <div>
          <h1 className="topbar-title">SoloCoach Control Center</h1>
          <p className="topbar-subtitle">
            {session.login} · {currentRoleName}
            {rolesLoading ? ' · загрузка ролей...' : ''}
            {rolesError ? ` · ${rolesError}` : ''}
          </p>
        </div>
        <div className="topbar-actions">
          <span className={`badge ${access.admin ? 'badge-admin' : 'badge-manager'}`}>
            {access.admin ? 'ADMIN' : 'MANAGER'}
          </span>
          <button
            type="button"
            className="btn btn-outline"
            onClick={() => handleLogout()}
          >
            Выйти
          </button>
        </div>
      </header>

      <nav className="workspace-tabs">
        {workspaceTabs.map((tab) => (
          <button
            key={tab.key}
            type="button"
            className={`tab-button ${workspace === tab.key ? 'tab-button--active' : ''}`}
            onClick={() => setWorkspace(tab.key)}
          >
            {tab.label}
          </button>
        ))}
      </nav>

      <main className="content">
        {workspace === 'manager' ? (
          <ManagerWorkspace token={token} />
        ) : (
          <AdminWorkspace token={token} roles={roles} />
        )}
      </main>
    </div>
  )
}

export default App
