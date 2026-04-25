import { useEffect, useMemo, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { ensureArray, getValue, toNullableNumber, formatDateTime } from '../utils/helpers.js'

function CrudSection({ token, config }) {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [editing, setEditing] = useState(null)
  const [createForm, setCreateForm] = useState({})
  const [refreshKey, setRefreshKey] = useState(0)
  const [selectOptions, setSelectOptions] = useState({})

  const idField = config.idField

  const filteredItems = useMemo(() => {
    const query = search.trim().toLowerCase()
    if (!query) {
      return items
    }

    return items.filter((item) => {
      const values = config.tableColumns.map((col) => getValue(item, col.key))
      return values.some((value) => String(value ?? '').toLowerCase().includes(query))
    })
  }, [items, search, config.tableColumns])

  // Load select options for all select fields
  useEffect(() => {
    let isCancelled = false
    const loadSelectOptions = async () => {
      const options = {}
      const selectFields = config.formFields.filter((f) => f.type === 'select')

      for (const field of selectFields) {
        // Если есть статические опции, используем их
        if (field.staticOptions) {
          options[field.key] = field.staticOptions
          continue
        }

        // Иначе загружаем с API
        if (field.selectOptions?.apiPath) {
          try {
            const data = await apiRequest(field.selectOptions.apiPath, { token })
            options[field.key] = ensureArray(data).map((item) => ({
              label: getValue(item, field.selectOptions.labelField),
              value: getValue(item, field.selectOptions.valueField),
            }))
          } catch (err) {
            console.error(`Failed to load options for ${field.key}:`, err)
            options[field.key] = []
          }
        }
      }

      if (!isCancelled) {
        setSelectOptions(options)
      }
    }

    loadSelectOptions()
    return () => {
      isCancelled = true
    }
  }, [token, config.formFields])

  // Load main items data
  useEffect(() => {
    let isCancelled = false
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const result = await apiRequest(config.apiPath, { token })
        if (!isCancelled) {
          setItems(ensureArray(result))
        }
      } catch (loadError) {
        if (!isCancelled) {
          setError(loadError.message)
          setItems([])
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
  }, [token, config.apiPath, refreshKey])

  useEffect(() => {
    const initial = {}
    for (const field of config.formFields) {
      initial[field.key] = ''
    }
    setCreateForm(initial)
  }, [config.formFields])

  const handleCreate = async () => {
    setSaving(true)
    try {
      const body = {}
      for (const field of config.formFields) {
        let value = createForm[field.key]
        if (field.emptyAsNull && !value.toString().trim()) {
          value = null
        } else if (field.type === 'number') {
          value = toNullableNumber(value)
        } else if (field.type === 'select' && !field.staticOptions) {
          // Only convert to number for API-based select fields, not string enums
          value = toNullableNumber(value)
        }
        body[field.key] = value
      }

      await apiRequest(config.apiPath, { token, method: 'POST', body })

      setCreateForm((previous) => {
        const next = { ...previous }
        for (const key of Object.keys(next)) {
          next[key] = ''
        }
        return next
      })

      setRefreshKey((value) => value + 1)
    } catch (error) {
      alert(`Ошибка создания: ${error.message}`)
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
      const body = { ...editing }
      for (const field of config.formFields) {
        let value = editing[field.key]
        if (field.emptyAsNull && !String(value ?? '').trim()) {
          value = null
        } else if (field.type === 'number') {
          value = toNullableNumber(value)
        } else if (field.type === 'select' && !field.staticOptions) {
          // Only convert to number for API-based select fields, not string enums
          value = toNullableNumber(value)
        }
        body[field.key] = value
      }

      await apiRequest(`${config.apiPath}/${editing[idField]}`, {
        token,
        method: 'PUT',
        body,
      })

      setEditing(null)

      setRefreshKey((value) => value + 1)
    } catch (error) {
      alert(`Ошибка обновления: ${error.message}`)
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (item) => {
    if (!confirm(`Удалить запись?`)) {
      return
    }

    try {
      await apiRequest(`${config.apiPath}/${item[idField]}`, {
        token,
        method: 'DELETE',
      })

      setRefreshKey((value) => value + 1)
    } catch (error) {
      alert(`Ошибка удаления: ${error.message}`)
    }
  }

  const startEdit = (item) => {
    setEditing({ ...item })
  }

  const cancelEdit = () => {
    setEditing(null)
  }

  const getSelectLabel = (fieldKey, value) => {
    const options = selectOptions[fieldKey] || []
    const option = options.find((opt) => opt.value === value || opt.value === Number(value))
    return option?.label ?? value
  }

  return (
    <section className="panel-block">
      <div className="panel-actions">
        <label className="field field-inline grow">
          <span>Поиск:</span>
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Поиск по таблице..."
          />
        </label>
        <button
          type="button"
          className="btn btn-outline"
          onClick={() => setRefreshKey((value) => value + 1)}
          disabled={loading}
        >
          Обновить
        </button>
      </div>

      {loading ? <p>Загрузка...</p> : null}
      {error ? <div className="error-banner">{error}</div> : null}

      {!loading && !error ? (
        <>
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  {config.tableColumns.map((col) => (
                    <th key={col.key}>{col.label}</th>
                  ))}
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                {filteredItems.map((item) => {
                  const id = item[idField]
                  const isEditing = editing && editing[idField] === id
                  return (
                    <tr key={id ?? `item-${Math.random()}`}>
                      {config.tableColumns.map((col) => {
                        const value = getValue(item, col.key)
                        if (isEditing) {
                          const field = config.formFields.find((f) => f.key === col.key)
                          if (field) {
                            return (
                              <td key={col.key}>
                                {field.type === 'textarea' ? (
                                  <textarea
                                    value={editing[col.key] ?? ''}
                                    onChange={(event) =>
                                      setEditing((previous) => ({
                                        ...previous,
                                        [col.key]: event.target.value,
                                      }))
                                    }
                                    required={field.required}
                                  />
                                ) : field.type === 'select' ? (
                                  <select
                                    value={editing[col.key] ?? ''}
                                    onChange={(event) =>
                                      setEditing((previous) => ({
                                        ...previous,
                                        [col.key]: event.target.value,
                                      }))
                                    }
                                    required={field.required}
                                  >
                                    <option value="">-- Выберите --</option>
                                    {(selectOptions[col.key] || []).map((opt) => (
                                      <option key={opt.value} value={opt.value}>
                                        {opt.label}
                                      </option>
                                    ))}
                                  </select>
                                ) : (
                                  <input
                                    type={field.type}
                                    value={editing[col.key] ?? ''}
                                    onChange={(event) =>
                                      setEditing((previous) => ({
                                        ...previous,
                                        [col.key]: event.target.value,
                                      }))
                                    }
                                    step={field.step}
                                    required={field.required}
                                  />
                                )}
                              </td>
                            )
                          }
                        }
                        const field = config.formFields.find((f) => f.key === col.key)
                        const displayValue =
                          field?.type === 'select' ? getSelectLabel(col.key, value) : value
                        return (
                          <td key={col.key}>
                            {col.type === 'datetime-local'
                              ? formatDateTime(displayValue)
                              : String(displayValue ?? '—')}
                          </td>
                        )
                      })}
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
                              onClick={() => startEdit(item)}
                            >
                              Редактировать
                            </button>
                            <button
                              type="button"
                              className="btn btn-danger btn-small"
                              onClick={() => handleDelete(item)}
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
            <h3>Создать новую запись</h3>
            <div className="form-grid">
              {config.formFields.map((field) => (
                <label key={field.key} className="field">
                  <span>{field.label}</span>
                  {field.type === 'textarea' ? (
                    <textarea
                      value={createForm[field.key] ?? ''}
                      onChange={(event) =>
                        setCreateForm((previous) => ({
                          ...previous,
                          [field.key]: event.target.value,
                        }))
                      }
                      required={field.required}
                    />
                  ) : field.type === 'select' ? (
                    <select
                      value={createForm[field.key] ?? ''}
                      onChange={(event) =>
                        setCreateForm((previous) => ({
                          ...previous,
                          [field.key]: event.target.value,
                        }))
                      }
                      required={field.required}
                    >
                      <option value="">-- Выберите --</option>
                      {(selectOptions[field.key] || []).map((opt) => (
                        <option key={opt.value} value={opt.value}>
                          {opt.label}
                        </option>
                      ))}
                    </select>
                  ) : (
                    <input
                      type={field.type}
                      value={createForm[field.key] ?? ''}
                      onChange={(event) =>
                        setCreateForm((previous) => ({
                          ...previous,
                          [field.key]: event.target.value,
                        }))
                      }
                      step={field.step}
                      required={field.required}
                    />
                  )}
                </label>
              ))}
            </div>
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleCreate}
              disabled={saving}
            >
              {saving ? 'Создание...' : 'Создать'}
            </button>
          </div>
        </>
      ) : null}
    </section>
  )
}

export default CrudSection
