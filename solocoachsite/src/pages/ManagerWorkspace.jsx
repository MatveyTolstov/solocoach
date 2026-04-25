import { useEffect, useState } from 'react'
import AnalyticsPanel from '../components/AnalyticsPanel.jsx'
import CrudSection from '../components/CrudSection.jsx'
import { ENTITY_CONFIGS, MANAGER_RESOURCE_KEYS } from '../constants/config.js'

function ManagerWorkspace({ token }) {
  const [activeTab, setActiveTab] = useState('analytics')

  const tabs = [
    { key: 'analytics', label: 'Аналитика' },
    ...MANAGER_RESOURCE_KEYS.map((key) => ({
      key,
      label: ENTITY_CONFIGS[key].title,
    })),
  ]

  useEffect(() => {
    const exists = tabs.some((tab) => tab.key === activeTab)
    if (!exists) {
      setActiveTab(tabs[0].key)
    }
  }, [tabs, activeTab])

  return (
    <section className="panel">
      <header className="panel-header">
        <h2>Раздел менеджера</h2>
        <p>Контент тренировок и аналитика пользователей</p>
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

      {activeTab === 'analytics' ? (
        <AnalyticsPanel token={token} />
      ) : (
        <CrudSection token={token} config={ENTITY_CONFIGS[activeTab]} />
      )}
    </section>
  )
}

export default ManagerWorkspace
