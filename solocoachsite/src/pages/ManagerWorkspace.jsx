import AnalyticsPanel from '../components/AnalyticsPanel.jsx'
import CrudSection from '../components/CrudSection.jsx'
import { ENTITY_CONFIGS } from '../constants/config.js'

function ManagerWorkspace({ token, activeTab }) {
  return (
    <div className="workspace">
      {activeTab === 'analytics' ? (
        <AnalyticsPanel token={token} />
      ) : (
        <CrudSection token={token} config={ENTITY_CONFIGS[activeTab]} />
      )}
    </div>
  )
}

export default ManagerWorkspace
