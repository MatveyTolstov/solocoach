import { useEffect, useMemo, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { ensureArray, getValue, toNullableNumber } from '../utils/helpers.js'
import { DEFAULT_USER_ROLE_ID } from '../utils/auth.js'
import { useToast } from './Toast.jsx'

function UserManagementSection({ token, roles }) {
  const notify = useToast()
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [editing, setEditing] = useState(null)
  const [createForm, setCreateForm] = useState({
    name: '',
    login: '',
    email: '',
    password: '',
    roleId: '',
    metricsUserId: '',
  })

  const roleOptions = useMemo(
    () =>
      ensureArray(roles)
        .map((role) => ({
          id: toNullableNumber(getValue(role, 'idRole')),
          name: String(getValue(role, 'roleName') ?? ''),
        }))
        .filter((role) => role.id != null),
    [roles],
  )

  const defaultRoleId = useMemo(() => {
    const userRole = roleOptions.find((role) => {
      const normalized = role.name.toLowerCase()
      return normalized.includes('user') || normalized.includes('пользоват')
    })
    return userRole?.id ?? DEFAULT_USER_ROLE_ID
  }, [roleOptions])

  useEffect(() => {
    setCreateForm((previous) => ({
      ...previous,
      roleId: previous.roleId || String(defaultRoleId),
    }))
  }, [defaultRoleId])

  useEffect(() => {
    let isCancelled = false
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const result = await apiRequest('/api/User', { token })
        if (!isCancelled) {
          setUsers(ensureArray(result))
        }
      } catch (loadError) {
        if (!isCancelled) {
          setError(loadError.message)
          setUsers([])
        }
      } finally {
        if (!isCancelled) {
          setLoading(false)
        }
      }
    }

    load()
    return () => {
      isCancelled = true
    }
  }, [token])

  const filteredUsers = useMemo(() => {
    const query = search.trim().toLowerCase()
    if (!query) {
      return users
    }

    return users.filter((user) => {
      const values = [
        getValue(user, 'idUser'),
        getValue(user, 'name'),
        getValue(user, 'login'),
        getValue(user, 'email'),
        getValue(user, 'roleId'),
      ]
      return values.some((value) => String(value ?? '').toLowerCase().includes(query))
    })
  }, [users, search])

  const handleCreate = async () => {
    if (!createForm.name.trim())           return notify('Введите имя', 'error')
    if (!createForm.login.trim())          return notify('Введите логин', 'error')
    if (!createForm.email.trim())          return notify('Введите email', 'error')
    if (!createForm.password.trim())       return notify('Введите пароль', 'error')
    if (createForm.password.length < 6)   return notify('Пароль должен быть минимум 6 символов', 'error')

    setSaving(true)
    try {
      await apiRequest('/api/User', {
        token, method: 'POST',
        body: {
          name: createForm.name.trim(),
          login: createForm.login.trim(),
          email: createForm.email.trim(),
          password: createForm.password.trim(),
          roleId: toNullableNumber(createForm.roleId),
          metricsUserId: toNullableNumber(createForm.metricsUserId) || null,
        },
      })
      setCreateForm({ name: '', login: '', email: '', password: '', roleId: String(defaultRoleId), metricsUserId: '' })
      const result = await apiRequest('/api/User', { token })
      setUsers(ensureArray(result))
      notify('Пользователь успешно создан', 'success')
    } catch (error) {
      notify(`Ошибка создания: ${error.message}`, 'error')
    } finally {
      setSaving(false)
    }
  }

  const handleUpdate = async () => {
    if (!editing) return

    const name  = editing.name?.trim()  ?? ''
    const login = editing.login?.trim() ?? ''
    const email = editing.email?.trim() ?? ''
    if (!name)  return notify('Введите имя', 'error')
    if (!login) return notify('Введите логин', 'error')
    if (!email) return notify('Введите email', 'error')
    if (!editing.roleId) return notify('Выберите роль', 'error')

    setSaving(true)
    try {
      const body = {
        idUser: editing.idUser,
        name,
        login,
        email,
        roleId: toNullableNumber(editing.roleId),
        metricsUserId: toNullableNumber(editing.metricsUserId) || null,
      }

      await apiRequest(`/api/User/${editing.idUser}`, { token, method: 'PUT', body })

      setEditing(null)

      const result = await apiRequest('/api/User', { token })
      setUsers(ensureArray(result))
      notify('Пользователь обновлён', 'success')
    } catch (error) {
      notify(`Ошибка обновления: ${error.message}`, 'error')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (user) => {
    if (!confirm(`Удалить пользователя ${user.login}?`)) return
    try {
      await apiRequest(`/api/User/${user.idUser}`, { token, method: 'DELETE' })
      setUsers((previous) => previous.filter((item) => item.idUser !== user.idUser))
      notify('Пользователь удалён', 'success')
    } catch (error) {
      notify(`Ошибка удаления: ${error.message}`, 'error')
    }
  }

  const startEdit = (user) => {
    setEditing({ ...user })
  }

  const cancelEdit = () => {
    setEditing(null)
  }

  return (
    <section className="panel-block">
      {/* Форма создания — над таблицей */}
      <div className="form-create-card">
        <p className="section-title">Новый пользователь</p>
        <div className="form-grid">
          <label className="field">
            <span className="field-label">Имя</span>
            <input type="text" value={createForm.name} onChange={(e) => setCreateForm((p) => ({ ...p, name: e.target.value }))} required />
          </label>
          <label className="field">
            <span className="field-label">Логин</span>
            <input type="text" value={createForm.login} onChange={(e) => setCreateForm((p) => ({ ...p, login: e.target.value }))} required />
          </label>
          <label className="field">
            <span className="field-label">Email</span>
            <input type="email" value={createForm.email} onChange={(e) => setCreateForm((p) => ({ ...p, email: e.target.value }))} required />
          </label>
          <label className="field">
            <span className="field-label">Пароль</span>
            <input type="password" value={createForm.password} onChange={(e) => setCreateForm((p) => ({ ...p, password: e.target.value }))} required />
          </label>
          <label className="field">
            <span className="field-label">Роль</span>
            <select value={createForm.roleId} onChange={(e) => setCreateForm((p) => ({ ...p, roleId: e.target.value }))} required>
              {roleOptions.map((role) => <option key={role.id} value={role.id}>{role.name}</option>)}
            </select>
          </label>
          <label className="field">
            <span className="field-label">MetricsUser ID (опционально)</span>
            <input type="number" value={createForm.metricsUserId} onChange={(e) => setCreateForm((p) => ({ ...p, metricsUserId: e.target.value }))} />
          </label>
        </div>
        <button type="button" className="btn btn-primary" onClick={handleCreate} disabled={saving}>
          {saving ? 'Создание...' : 'Создать пользователя'}
        </button>
      </div>

      {/* Тулбар поиска */}
      <div className="panel-toolbar">
        <div className="search-wrap">
          <label className="field">
            <input type="search" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Поиск по имени, логину, email..." />
          </label>
        </div>
      </div>

      {loading && <p className="loading">Загрузка пользователей...</p>}
      {error && <div className="error-banner">{error}</div>}

      {!loading && !error && (
        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr><th>ID</th><th>Имя</th><th>Логин</th><th>Email</th><th>Роль</th><th>Действия</th></tr>
            </thead>
            <tbody>
              {filteredUsers.map((user) => {
                const userId = getValue(user, 'idUser')
                const isEditing = editing && editing.idUser === userId
                return (
                  <tr key={userId ?? Math.random()}>
                    <td>{userId ?? '—'}</td>
                    <td>{isEditing
                      ? <input type="text" value={editing.name ?? ''} onChange={(e) => setEditing((p) => ({ ...p, name: e.target.value }))} />
                      : String(getValue(user, 'name') ?? '—')}
                    </td>
                    <td>{isEditing
                      ? <input type="text" value={editing.login ?? ''} onChange={(e) => setEditing((p) => ({ ...p, login: e.target.value }))} />
                      : String(getValue(user, 'login') ?? '—')}
                    </td>
                    <td>{isEditing
                      ? <input type="email" value={editing.email ?? ''} onChange={(e) => setEditing((p) => ({ ...p, email: e.target.value }))} />
                      : String(getValue(user, 'email') ?? '—')}
                    </td>
                    <td>{isEditing
                      ? (
                        <select value={editing.roleId ?? ''} onChange={(e) => setEditing((p) => ({ ...p, roleId: e.target.value }))}>
                          {roleOptions.map((role) => <option key={role.id} value={role.id}>{role.name}</option>)}
                        </select>
                      )
                      : roleOptions.find((r) => r.id === toNullableNumber(getValue(user, 'roleId')))?.name ?? `Role ${getValue(user, 'roleId') ?? 'N/A'}`}
                    </td>
                    <td>
                      <div className="td-actions">
                        {isEditing ? (
                          <>
                            <button type="button" className="btn btn-primary btn-sm" onClick={handleUpdate} disabled={saving}>Сохранить</button>
                            <button type="button" className="btn btn-ghost btn-sm" onClick={cancelEdit}>Отмена</button>
                          </>
                        ) : (
                          <>
                            <button type="button" className="btn btn-outline btn-sm" onClick={() => startEdit(user)}>Изменить</button>
                            <button type="button" className="btn btn-danger-ghost btn-sm" onClick={() => handleDelete(user)}>Удалить</button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default UserManagementSection
