import { useRef, useState } from 'react'
import LogsPanel from '../components/LogsPanel.jsx'
import UserManagementSection from '../components/UserManagementSection.jsx'
import CrudSection from '../components/CrudSection.jsx'
import { ENTITY_CONFIGS, ADMIN_DATABASE_KEYS } from '../constants/config.js'
import { useToast } from '../components/Toast.jsx'
import { API_BASE_URL } from '../utils/auth.js'

function AdminWorkspace({ token, roles, activeTab }) {
  const notify = useToast()
  const [selectedTable, setSelectedTable] = useState(ADMIN_DATABASE_KEYS[0])
  const [backupLoading, setBackupLoading] = useState(false)
  const [restoreFile, setRestoreFile] = useState(null)
  const [restoreLoading, setRestoreLoading] = useState(false)
  const restoreInputRef = useRef(null)

  const handleCreateBackup = async () => {
    setBackupLoading(true)
    try {
      const response = await fetch('http://localhost:5219/api/Backup/create', {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` },
      })
      if (!response.ok) throw new Error(`Backup failed: ${response.statusText}`)
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `backup_${new Date().toISOString().split('T')[0]}.sql`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
      notify('Бекап успешно создан и скачан', 'success')
    } catch (error) {
      notify(`Ошибка создания бекапа: ${error.message}`, 'error')
    } finally {
      setBackupLoading(false)
    }
  }

  const handleRestoreBackup = async () => {
    if (!restoreFile) return
    if (!confirm(`Восстановить БД из файла "${restoreFile.name}"? Текущие данные будут перезаписаны.`)) return
    setRestoreLoading(true)
    try {
      const form = new FormData()
      form.append('file', restoreFile)
      const response = await fetch(`${API_BASE_URL}/api/Backup/restore`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: form,
      })
      if (!response.ok) {
        const text = await response.text()
        throw new Error(text || `HTTP ${response.status}`)
      }
      setRestoreFile(null)
      notify('База данных успешно восстановлена', 'success')
    } catch (error) {
      notify(`Ошибка восстановления: ${error.message}`, 'error')
    } finally {
      setRestoreLoading(false)
    }
  }

  return (
    <div className="workspace">
      {activeTab === 'logs' && <LogsPanel token={token} />}
      {activeTab === 'users' && <UserManagementSection token={token} roles={roles} />}

      {activeTab === 'database' && (
        <>
          <div className="db-toolbar">
            <label className="field field-inline">
              <span className="field-label">Таблица:</span>
              <select
                value={selectedTable}
                onChange={(e) => setSelectedTable(e.target.value)}
              >
                {ADMIN_DATABASE_KEYS.map((key) => (
                  <option key={key} value={key}>
                    {ENTITY_CONFIGS[key].title}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <CrudSection token={token} config={ENTITY_CONFIGS[selectedTable]} />
        </>
      )}

      {activeTab === 'backup' && (
        <div className="backup-section">
          <h3>Резервные копии</h3>

          <div style={{ display: 'flex', gap: 12, marginBottom: 16, alignItems: 'center' }}>
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleCreateBackup}
              disabled={backupLoading}
            >
              {backupLoading ? 'Создание...' : 'Создать резервную копию'}
            </button>
          </div>
          <div className="info-banner" style={{ marginBottom: 24 }}>
            Нажмите кнопку выше чтобы создать резервную копию базы данных.
            Файл будет автоматически скачан на ваш компьютер.
          </div>

          <h3>Восстановление из файла</h3>
          <div style={{ display: 'flex', gap: 12, marginBottom: 16, alignItems: 'center' }}>
            <input
              ref={restoreInputRef}
              type="file"
              accept=".sql"
              style={{ display: 'none' }}
              onChange={(e) => setRestoreFile(e.target.files?.[0] ?? null)}
            />
            <button
              type="button"
              className="btn btn-outline"
              onClick={() => restoreInputRef.current?.click()}
            >
              {restoreFile ? restoreFile.name : 'Выбрать .sql файл'}
            </button>
            {restoreFile && (
              <button
                type="button"
                className="btn btn-danger-ghost"
                onClick={() => { setRestoreFile(null); restoreInputRef.current.value = '' }}
              >
                Очистить
              </button>
            )}
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleRestoreBackup}
              disabled={!restoreFile || restoreLoading}
            >
              {restoreLoading ? 'Восстановление...' : 'Восстановить'}
            </button>
          </div>
          <div className="info-banner">
            Загрузите .sql файл резервной копии. Внимание: текущие данные будут перезаписаны.
          </div>
        </div>
      )}
    </div>
  )
}

export default AdminWorkspace
