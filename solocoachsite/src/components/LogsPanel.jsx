import { useEffect, useMemo, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { ensureArray, getValue, formatDateTime } from '../utils/helpers.js'

function LogsPanel({ token }) {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [logs, setLogs] = useState([])
  const [refreshKey, setRefreshKey] = useState(0)

  useEffect(() => {
    let isCancelled = false
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const logsRaw = await apiRequest('/api/ApplicationLog/admin', { token })

        if (!isCancelled) {
          setLogs(ensureArray(logsRaw))
        }
      } catch (loadError) {
        if (!isCancelled) {
          setError(loadError.message)
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
  }, [token, refreshKey])

  const filteredLogs = useMemo(() => {
    const query = search.trim().toLowerCase()
    if (!query) {
      return logs
    }

    return logs.filter((log) => {
      const values = [
        getValue(log, 'idApplicationLog'),
        getValue(log, 'action'),
        getValue(log, 'entityType'),
        getValue(log, 'entityId'),
        getValue(log, 'userId'),
        getValue(log, 'status'),
        getValue(log, 'errorMessage'),
        getValue(log, 'details'),
      ]
      return values.some((value) => String(value ?? '').toLowerCase().includes(query))
    })
  }, [logs, search])

  return (
    <section className="panel-block">
      <div className="panel-actions">
        <label className="field field-inline grow">
          <span>Поиск по логам:</span>
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="login, workout, статус..."
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

      {loading ? <p>Загрузка логов...</p> : null}
      {error ? <div className="error-banner">{error}</div> : null}

      {!loading && !error ? (
        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Время</th>
                <th>Действие</th>
                <th>Тип сущности</th>
                <th>ID сущности</th>
                <th>ID пользователя</th>
                <th>Статус</th>
                <th>Ошибка</th>
              </tr>
            </thead>
            <tbody>
              {filteredLogs.map((log) => {
                const logId = getValue(log, 'idApplicationLog')
                return (
                  <tr key={logId ?? `log-${Math.random()}`}>
                    <td>{logId ?? '—'}</td>
                    <td>{formatDateTime(getValue(log, 'createdAt'))}</td>
                    <td>{String(getValue(log, 'action') ?? '—')}</td>
                    <td>{String(getValue(log, 'entityType') ?? '—')}</td>
                    <td>{String(getValue(log, 'entityId') ?? '—')}</td>
                    <td>{String(getValue(log, 'userId') ?? '—')}</td>
                    <td>{String(getValue(log, 'status') ?? '—')}</td>
                    <td>{String(getValue(log, 'errorMessage') ?? '—')}</td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  )
}

export default LogsPanel
