import { useState } from 'react'
import LogsPanel from '../components/LogsPanel.jsx'
import UserManagementSection from '../components/UserManagementSection.jsx'
import CrudSection from '../components/CrudSection.jsx'
import { ENTITY_CONFIGS, ADMIN_DATABASE_KEYS } from '../constants/config.js'
import { apiRequest } from '../utils/api.js'

function AdminWorkspace({ token, roles }) {
  const [activeTab, setActiveTab] = useState('logs')
  const [selectedDatabaseTable, setSelectedDatabaseTable] = useState(ADMIN_DATABASE_KEYS[0])
  const [backupLoading, setBackupLoading] = useState(false)

  const tabs = [
    { key: 'logs', label: 'Логи приложения' },
    { key: 'users', label: 'Пользователи' },
    { key: 'database', label: 'База данных' },
    { key: 'backup', label: 'Бекап и восстановление' },
  ]

  const handleCreateBackup = async () => {
    setBackupLoading(true)
    try {
      const response = await fetch('http://localhost:5219/api/Backup/create', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      })

      if (!response.ok) {
        throw new Error(`Backup failed: ${response.statusText}`)
      }

      // Получаем blob и скачиваем файл
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `backup_${new Date().toISOString().split('T')[0]}.sql`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)

      alert('Бекап успешно создан и скачан!')
    } catch (error) {
      alert(`Ошибка создания бекапа: ${error.message}`)
    } finally {
      setBackupLoading(false)
    }
  }

  return (
    <section className="panel">
      <header className="panel-header">
        <h2>Раздел администратора</h2>
        <p>Логи, пользователи, полный CRUD по таблицам, бекапы базы данных</p>
      </header>

      <div className="subtabs">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            type="button"
            className={`tab-button tab-button--sub ${
              activeTab === tab.key ? 'tab-button--active' : ''
            }`}
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === 'logs' ? <LogsPanel token={token} /> : null}
      {activeTab === 'users' ? <UserManagementSection token={token} roles={roles} /> : null}
      {activeTab === 'database' ? (
        <>
          <div className="panel-actions">
            <label className="field field-inline">
              <span>Таблица:</span>
              <select
                value={selectedDatabaseTable}
                onChange={(event) => setSelectedDatabaseTable(event.target.value)}
              >
                {ADMIN_DATABASE_KEYS.map((key) => (
                  <option key={key} value={key}>
                    {ENTITY_CONFIGS[key].title}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <CrudSection token={token} config={ENTITY_CONFIGS[selectedDatabaseTable]} />
        </>
      ) : null}
      {activeTab === 'backup' ? (
        <section className="panel-block">
          <h3>Управление резервными копиями</h3>
          <div className="panel-actions">
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleCreateBackup}
              disabled={backupLoading}
            >
              {backupLoading ? 'Создание бекапа...' : 'Создать резервную копию'}
            </button>
          </div>
          <div className="info-banner">
            <p>
              Нажмите кнопку выше чтобы создать резервную копию базы данных.
              Файл будет автоматически скачан на ваш компьютер.
            </p>
          </div>
        </section>
      ) : null}
    </section>
  )
}

export default AdminWorkspace
