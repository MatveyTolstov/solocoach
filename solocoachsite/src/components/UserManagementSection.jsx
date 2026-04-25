import { useEffect, useMemo, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { ensureArray, getValue, toNullableNumber } from '../utils/helpers.js'
import { DEFAULT_USER_ROLE_ID } from '../utils/auth.js'

function UserManagementSection({ token, roles }) {
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
    setSaving(true)
    try {
      // Валидация
      if (!createForm.name.trim()) {
        alert('Введите имя')
        setSaving(false)
        return
      }
      if (!createForm.login.trim()) {
        alert('Введите логин')
        setSaving(false)
        return
      }
      if (!createForm.email.trim()) {
        alert('Введите email')
        setSaving(false)
        return
      }
      if (!createForm.password.trim()) {
        alert('Введите пароль (минимум 6 символов)')
        setSaving(false)
        return
      }
      if (createForm.password.length < 6) {
        alert('Пароль должен быть минимум 6 символов')
        setSaving(false)
        return
      }

      const body = {
        name: createForm.name.trim(),
        login: createForm.login.trim(),
        email: createForm.email.trim(),
        password: createForm.password.trim(),
        roleId: toNullableNumber(createForm.roleId),
        metricsUserId: toNullableNumber(createForm.metricsUserId) || null,
      }

      await apiRequest('/api/User', { token, method: 'POST', body })

      setCreateForm({
        name: '',
        login: '',
        email: '',
        password: '',
        roleId: String(defaultRoleId),
        metricsUserId: '',
      })

      const result = await apiRequest('/api/User', { token })
      setUsers(ensureArray(result))
    } catch (error) {
      alert(`Ошибка создания пользователя: ${error.message}`)
    } finally {
      setSaving(false)
    }
  }

  const handleUpdate = async () => {
    if (!editing) {
      return
    }

    setSaving(true)
    try {
      const body = {
        idUser: editing.idUser,
        name: editing.name.trim(),
        login: editing.login.trim(),
        email: editing.email.trim(),
        roleId: toNullableNumber(editing.roleId),
        metricsUserId: toNullableNumber(editing.metricsUserId) || null,
      }

      await apiRequest(`/api/User/${editing.idUser}`, { token, method: 'PUT', body })

      setEditing(null)

      const result = await apiRequest('/api/User', { token })
      setUsers(ensureArray(result))
    } catch (error) {
      alert(`Ошибка обновления пользователя: ${error.message}`)
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (user) => {
    if (!confirm(`Удалить пользователя ${user.login}?`)) {
      return
    }

    try {
      await apiRequest(`/api/User/${user.idUser}`, { token, method: 'DELETE' })

      setUsers((previous) => previous.filter((item) => item.idUser !== user.idUser))
    } catch (error) {
      alert(`Ошибка удаления пользователя: ${error.message}`)
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
      <div className="panel-actions">
        <label className="field field-inline grow">
          <span>Поиск пользователей:</span>
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="name, login, email..."
          />
        </label>
      </div>

      {loading ? <p>Загрузка пользователей...</p> : null}
      {error ? <div className="error-banner">{error}</div> : null}

      {!loading && !error ? (
        <>
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Имя</th>
                  <th>Логин</th>
                  <th>Email</th>
                  <th>Роль</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                {filteredUsers.map((user) => {
                  const userId = getValue(user, 'idUser')
                  const isEditing = editing && editing.idUser === userId
                  return (
                    <tr key={userId ?? `user-${Math.random()}`}>
                      <td>{userId ?? '—'}</td>
                      <td>
                        {isEditing ? (
                          <input
                            type="text"
                            value={editing.name ?? ''}
                            onChange={(event) =>
                              setEditing((previous) => ({ ...previous, name: event.target.value }))
                            }
                            required
                          />
                        ) : (
                          String(getValue(user, 'name') ?? '—')
                        )}
                      </td>
                      <td>
                        {isEditing ? (
                          <input
                            type="text"
                            value={editing.login ?? ''}
                            onChange={(event) =>
                              setEditing((previous) => ({ ...previous, login: event.target.value }))
                            }
                            required
                          />
                        ) : (
                          String(getValue(user, 'login') ?? '—')
                        )}
                      </td>
                      <td>
                        {isEditing ? (
                          <input
                            type="email"
                            value={editing.email ?? ''}
                            onChange={(event) =>
                              setEditing((previous) => ({ ...previous, email: event.target.value }))
                            }
                            required
                          />
                        ) : (
                          String(getValue(user, 'email') ?? '—')
                        )}
                      </td>
                      <td>
                        {isEditing ? (
                          <select
                            value={editing.roleId ?? ''}
                            onChange={(event) =>
                              setEditing((previous) => ({ ...previous, roleId: event.target.value }))
                            }
                            required
                          >
                            {roleOptions.map((role) => (
                              <option key={role.id} value={role.id}>
                                {role.name}
                              </option>
                            ))}
                          </select>
                        ) : (
                          roleOptions.find(
                            (role) => role.id === toNullableNumber(getValue(user, 'roleId')),
                          )?.name ?? `Role ${getValue(user, 'roleId') ?? 'N/A'}`
                        )}
                      </td>
                      <td>
                        {isEditing ? (
                          <>
                            <button
                              type="button"
                              className="btn btn-primary btn-small"
                              onClick={handleUpdate}
                              disabled={saving}
                            >
                              Сохранить
                            </button>
                            <button
                              type="button"
                              className="btn btn-outline btn-small"
                              onClick={cancelEdit}
                            >
                              Отмена
                            </button>
                          </>
                        ) : (
                          <>
                            <button
                              type="button"
                              className="btn btn-outline btn-small"
                              onClick={() => startEdit(user)}
                            >
                              Редактировать
                            </button>
                            <button
                              type="button"
                              className="btn btn-danger btn-small"
                              onClick={() => handleDelete(user)}
                            >
                              Удалить
                            </button>
                          </>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>

          <div className="form-section">
            <h3>Создать нового пользователя</h3>
            <div className="form-grid">
              <label className="field">
                <span>Имя</span>
                <input
                  type="text"
                  value={createForm.name}
                  onChange={(event) =>
                    setCreateForm((previous) => ({ ...previous, name: event.target.value }))
                  }
                  required
                />
              </label>
              <label className="field">
                <span>Логин</span>
                <input
                  type="text"
                  value={createForm.login}
                  onChange={(event) =>
                    setCreateForm((previous) => ({ ...previous, login: event.target.value }))
                  }
                  required
                />
              </label>
              <label className="field">
                <span>Email</span>
                <input
                  type="email"
                  value={createForm.email}
                  onChange={(event) =>
                    setCreateForm((previous) => ({ ...previous, email: event.target.value }))
                  }
                  required
                />
              </label>
              <label className="field">
                <span>Пароль</span>
                <input
                  type="password"
                  value={createForm.password}
                  onChange={(event) =>
                    setCreateForm((previous) => ({ ...previous, password: event.target.value }))
                  }
                  required
                />
              </label>
              <label className="field">
                <span>Роль</span>
                <select
                  value={createForm.roleId}
                  onChange={(event) =>
                    setCreateForm((previous) => ({ ...previous, roleId: event.target.value }))
                  }
                  required
                >
                  {roleOptions.map((role) => (
                    <option key={role.id} value={role.id}>
                      {role.name}
                    </option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>MetricsUser ID (опционально)</span>
                <input
                  type="number"
                  value={createForm.metricsUserId}
                  onChange={(event) =>
                    setCreateForm((previous) => ({
                      ...previous,
                      metricsUserId: event.target.value,
                    }))
                  }
                />
              </label>
            </div>
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleCreate}
              disabled={saving}
            >
              {saving ? 'Создание...' : 'Создать пользователя'}
            </button>
          </div>
        </>
      ) : null}
    </section>
  )
}

export default UserManagementSection
