import { useEffect, useMemo, useRef, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { API_BASE_URL } from '../utils/auth.js'
import { ensureArray, getValue, toNullableNumber, formatDateTime } from '../utils/helpers.js'
import { useToast } from './Toast.jsx'

function CrudSection({ token, config }) {
  const notify = useToast()

  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [editing, setEditing] = useState(null)
  const [createForm, setCreateForm] = useState({})
  const [refreshKey, setRefreshKey] = useState(0)
  const [selectOptions, setSelectOptions] = useState({})
  const [uploadingGifId, setUploadingGifId] = useState(null)
  const [pendingGifId, setPendingGifId] = useState(null)
  const fileInputRef = useRef(null)
  const gifPanelInputRef = useRef(null)
  const [gifPanelId, setGifPanelId] = useState('')
  const [gifPanelFile, setGifPanelFile] = useState(null)
  const [gifPanelUploading, setGifPanelUploading] = useState(false)

  const idField = config.idField

  const filteredItems = useMemo(() => {
    const query = search.trim().toLowerCase()
    if (!query) return items
    return items.filter((item) =>
      config.tableColumns.some((col) => {
        const raw = getValue(item, col.key)
        const field = config.formFields.find((f) => f.key === col.key)
        if (field?.type === 'select') {
          const opts = selectOptions[col.key] || []
          const opt = opts.find((o) => o.value === raw || o.value === Number(raw))
          return String(opt?.label ?? raw ?? '').toLowerCase().includes(query)
        }
        return String(raw ?? '').toLowerCase().includes(query)
      })
    )
  }, [items, search, config.tableColumns, config.formFields, selectOptions])

  useEffect(() => {
    let cancelled = false
    const loadSelectOptions = async () => {
      const options = {}
      for (const field of config.formFields.filter((f) => f.type === 'select')) {
        if (field.staticOptions) { options[field.key] = field.staticOptions; continue }
        if (field.selectOptions?.apiPath) {
          try {
            const data = await apiRequest(field.selectOptions.apiPath, { token })
            options[field.key] = ensureArray(data).map((item) => ({
              label: getValue(item, field.selectOptions.labelField),
              value: getValue(item, field.selectOptions.valueField),
            }))
          } catch { options[field.key] = [] }
        }
      }
      if (!cancelled) setSelectOptions(options)
    }
    loadSelectOptions()
    return () => { cancelled = true }
  }, [token, config.formFields])

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError('')
    apiRequest(config.apiPath, { token })
      .then((result) => { if (!cancelled) setItems(ensureArray(result)) })
      .catch((err)  => { if (!cancelled) { setError(err.message); setItems([]) } })
      .finally(()   => { if (!cancelled) setLoading(false) })
    return () => { cancelled = true }
  }, [token, config.apiPath, refreshKey])

  useEffect(() => {
    const initial = {}
    config.formFields.forEach((f) => { initial[f.key] = '' })
    setCreateForm(initial)
  }, [config.formFields])

  const buildBody = (form, fields) => {
    const body = {}
    for (const field of fields) {
      let value = form[field.key]
      if (field.emptyAsNull && !String(value ?? '').trim()) value = null
      else if (field.type === 'number') value = toNullableNumber(value)
      else if (field.type === 'select' && !field.staticOptions) value = toNullableNumber(value)
      body[field.key] = value
    }
    return body
  }

  const validate = (form, fields) => {
    const missing = []
    const invalid = []
    for (const field of fields) {
      const raw = String(form[field.key] ?? '').trim()
      if (field.required) {
        if (raw === '' || raw === 'null') { missing.push(field.label); continue }
      }
      if (field.type === 'number' && raw !== '') {
        if (!Number.isFinite(Number(raw))) invalid.push(field.label)
      }
    }
    if (missing.length) notify(`Обязательные поля: ${missing.join(', ')}`, 'error')
    if (invalid.length) notify(`Некорректное число в: ${invalid.join(', ')}`, 'error')
    return missing.length === 0 && invalid.length === 0
  }

  const handleCreate = async () => {
    if (!validate(createForm, config.formFields)) return
    setSaving(true)
    try {
      await apiRequest(config.apiPath, { token, method: 'POST', body: buildBody(createForm, config.formFields) })
      setCreateForm((prev) => { const next = { ...prev }; Object.keys(next).forEach((k) => { next[k] = '' }); return next })
      setRefreshKey((v) => v + 1)
      notify('Запись успешно создана', 'success')
    } catch (err) {
      notify(`Ошибка создания: ${err.message}`, 'error')
    } finally {
      setSaving(false)
    }
  }

  const handleUpdate = async () => {
    if (!editing) return
    if (!validate(editing, config.formFields)) return
    setSaving(true)
    try {
      const body = buildBody(editing, config.formFields)
      body[idField] = editing[idField]
      await apiRequest(`${config.apiPath}/${editing[idField]}`, {
        token, method: 'PUT', body,
      })
      setEditing(null)
      setRefreshKey((v) => v + 1)
      notify('Запись успешно обновлена', 'success')
    } catch (err) {
      notify(`Ошибка обновления: ${err.message}`, 'error')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (item) => {
    if (!confirm('Удалить запись?')) return
    try {
      await apiRequest(`${config.apiPath}/${item[idField]}`, { token, method: 'DELETE' })
      setRefreshKey((v) => v + 1)
      notify('Запись удалена', 'success')
    } catch (err) {
      notify(`Ошибка удаления: ${err.message}`, 'error')
    }
  }

  const handleGifUploadClick = (id) => {
    setPendingGifId(id)
    fileInputRef.current?.click()
  }

  const handleFileSelected = async (e) => {
    const file = e.target.files?.[0]
    e.target.value = ''
    if (!file || pendingGifId == null) return
    const id = pendingGifId
    setPendingGifId(null)
    setUploadingGifId(id)
    try {
      const form = new FormData()
      form.append('file', file)
      const response = await fetch(`${API_BASE_URL}${config.gifUploadApiPath}/${id}/video`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: form,
      })
      if (!response.ok) throw new Error(await response.text() || `HTTP ${response.status}`)
      setRefreshKey((v) => v + 1)
      notify('GIF успешно загружен', 'success')
    } catch (err) {
      notify(`Ошибка загрузки GIF: ${err.message}`, 'error')
    } finally {
      setUploadingGifId(null)
    }
  }

  const handleGifDelete = async (id) => {
    if (!confirm('Удалить GIF?')) return
    try {
      const response = await fetch(`${API_BASE_URL}${config.gifUploadApiPath}/${id}/video`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      })
      if (!response.ok) throw new Error(await response.text() || `HTTP ${response.status}`)
      setRefreshKey((v) => v + 1)
      notify('GIF удалён', 'success')
    } catch (err) {
      notify(`Ошибка удаления GIF: ${err.message}`, 'error')
    }
  }

  const handleGifPanelUpload = async () => {
    if (!gifPanelId || !gifPanelFile) return
    setGifPanelUploading(true)
    try {
      const form = new FormData()
      form.append('file', gifPanelFile)
      const response = await fetch(`${API_BASE_URL}${config.gifUploadApiPath}/${gifPanelId}/video`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: form,
      })
      if (!response.ok) throw new Error(await response.text() || `HTTP ${response.status}`)
      setGifPanelFile(null)
      setGifPanelId('')
      if (gifPanelInputRef.current) gifPanelInputRef.current.value = ''
      setRefreshKey((v) => v + 1)
      notify('GIF успешно загружен', 'success')
    } catch (err) {
      notify(`Ошибка загрузки GIF: ${err.message}`, 'error')
    } finally {
      setGifPanelUploading(false)
    }
  }

  const getSelectLabel = (fieldKey, value) => {
    const opts = selectOptions[fieldKey] || []
    const opt = opts.find((o) => o.value === value || o.value === Number(value))
    return opt?.label ?? value
  }

  const renderEditCell = (col) => {
    const field = config.formFields.find((f) => f.key === col.key)
    if (!field) return null
    const onChange = (e) => setEditing((prev) => ({ ...prev, [col.key]: e.target.value }))
    if (field.type === 'textarea')
      return <textarea value={editing[col.key] ?? ''} onChange={onChange} />
    if (field.type === 'select')
      return (
        <select value={editing[col.key] ?? ''} onChange={onChange}>
          {(selectOptions[col.key] || []).map((o) => (
            <option key={o.value} value={o.value}>{o.label}</option>
          ))}
        </select>
      )
    return <input type={field.type} value={editing[col.key] ?? ''} onChange={onChange} step={field.step} />
  }

  return (
    <section className="panel-block">
      {config.gifUploadApiPath && (
        <input
          ref={fileInputRef}
          type="file"
          accept=".gif,image/gif"
          style={{ display: 'none' }}
          onChange={handleFileSelected}
        />
      )}
      {/* Форма создания — над таблицей */}
      <div className="form-create-card">
        <p className="section-title">Новая запись</p>
        <div className="form-grid">
          {config.formFields.map((field) => (
            <label key={field.key} className="field">
              <span className="field-label">{field.label}</span>
              {field.type === 'textarea' ? (
                <textarea
                  value={createForm[field.key] ?? ''}
                  onChange={(e) => setCreateForm((p) => ({ ...p, [field.key]: e.target.value }))}
                  required={field.required}
                />
              ) : field.type === 'select' ? (
                <select
                  value={createForm[field.key] ?? ''}
                  onChange={(e) => setCreateForm((p) => ({ ...p, [field.key]: e.target.value }))}
                  required={field.required}
                >
                  {(selectOptions[field.key] || []).map((o) => (
                    <option key={o.value} value={o.value}>{o.label}</option>
                  ))}
                </select>
              ) : (
                <input
                  type={field.type}
                  value={createForm[field.key] ?? ''}
                  onChange={(e) => setCreateForm((p) => ({ ...p, [field.key]: e.target.value }))}
                  step={field.step}
                  required={field.required}
                />
              )}
            </label>
          ))}
        </div>
        <button className="btn btn-primary" onClick={handleCreate} disabled={saving}>
          {saving ? 'Создание...' : 'Создать'}
        </button>
      </div>

      {/* Панель загрузки GIF */}
      {config.gifUploadApiPath && (
        <div className="form-create-card">
          <p className="section-title">Загрузить GIF</p>
          <input
            ref={gifPanelInputRef}
            type="file"
            accept=".gif,image/gif"
            style={{ display: 'none' }}
            onChange={(e) => setGifPanelFile(e.target.files?.[0] ?? null)}
          />
          <div className="form-grid">
            <label className="field">
              <span className="field-label">Запись</span>
              <select value={gifPanelId} onChange={(e) => setGifPanelId(e.target.value)}>
                {items.map((item) => (
                  <option key={item[idField]} value={item[idField]}>
                    {item[config.tableColumns[0].key] ?? item[idField]}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span className="field-label">Файл</span>
              <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                <button
                  type="button"
                  className="btn btn-outline btn-sm"
                  onClick={() => gifPanelInputRef.current?.click()}
                >
                  {gifPanelFile ? gifPanelFile.name : 'Выбрать .gif'}
                </button>
                {gifPanelFile && (
                  <button
                    type="button"
                    className="btn btn-danger-ghost btn-sm"
                    onClick={() => { setGifPanelFile(null); if (gifPanelInputRef.current) gifPanelInputRef.current.value = '' }}
                  >
                    ✕
                  </button>
                )}
              </div>
            </label>
          </div>
          <button
            className="btn btn-primary"
            onClick={handleGifPanelUpload}
            disabled={!gifPanelId || !gifPanelFile || gifPanelUploading}
          >
            {gifPanelUploading ? 'Загрузка...' : 'Загрузить GIF'}
          </button>
        </div>
      )}

      {/* Тулбар поиска */}
      <div className="panel-toolbar">
        <div className="search-wrap">
          <label className="field">
            <input
              type="search"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Поиск по таблице..."
            />
          </label>
        </div>
        <button
          type="button"
          className="btn btn-ghost btn-sm"
          onClick={() => setRefreshKey((v) => v + 1)}
          disabled={loading}
        >
          Обновить
        </button>
      </div>

      {loading && <p className="loading">Загрузка...</p>}
      {error && <div className="error-banner">{error}</div>}

      {!loading && !error && (
        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                {config.tableColumns.map((col) => <th key={col.key}>{col.label}</th>)}
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredItems.map((item) => {
                const id = item[idField]
                const isEditing = editing && editing[idField] === id
                return (
                  <tr key={id ?? Math.random()}>
                    {config.tableColumns.map((col) => {
                      if (isEditing) {
                        const cell = renderEditCell(col)
                        if (cell) return <td key={col.key}>{cell}</td>
                      }
                      const value = getValue(item, col.key)
                      const field = config.formFields.find((f) => f.key === col.key)
                      if (col.type === 'gif') {
                        return (
                          <td key={col.key}>
                            {value
                              ? <a href={value} target="_blank" rel="noopener noreferrer">Открыть</a>
                              : '—'}
                          </td>
                        )
                      }
                      const display = field?.type === 'select' ? getSelectLabel(col.key, value) : value
                      return (
                        <td key={col.key}>
                          {col.type === 'datetime-local' ? formatDateTime(display) : String(display ?? '—')}
                        </td>
                      )
                    })}
                    <td>
                      <div className="td-actions">
                        {isEditing ? (
                          <>
                            <button className="btn btn-primary btn-sm" onClick={handleUpdate} disabled={saving}>Сохранить</button>
                            <button className="btn btn-ghost btn-sm" onClick={() => setEditing(null)}>Отмена</button>
                          </>
                        ) : (
                          <>
                            <button className="btn btn-outline btn-sm" onClick={() => setEditing({ ...item })}>Изменить</button>
                            <button className="btn btn-danger-ghost btn-sm" onClick={() => handleDelete(item)}>Удалить</button>
                            {config.gifUploadApiPath && (
                              <>
                                <button
                                  className="btn btn-outline btn-sm"
                                  onClick={() => handleGifUploadClick(id)}
                                  disabled={uploadingGifId === id}
                                >
                                  {uploadingGifId === id ? '...' : item.videoUrl ? 'GIF ↑' : 'GIF +'}
                                </button>
                                {item.videoUrl && (
                                  <button className="btn btn-danger-ghost btn-sm" onClick={() => handleGifDelete(id)}>
                                    GIF ✕
                                  </button>
                                )}
                              </>
                            )}
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

export default CrudSection
