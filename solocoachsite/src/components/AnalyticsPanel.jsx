import { useEffect, useState } from 'react'
import { apiRequest } from '../utils/api.js'
import { ensureArray, getValue, toNullableNumber, countBy } from '../utils/helpers.js'
import StatCard from './StatCard.jsx'
import RoleDistributionChart from './RoleDistributionChart.jsx'
import AgeDistributionChart from './AgeDistributionChart.jsx'
import GenderDistributionChart from './GenderDistributionChart.jsx'
import ActivityDistributionChart from './ActivityDistributionChart.jsx'

function AnalyticsPanel({ token }) {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [snapshot, setSnapshot] = useState(null)
  const [refreshKey, setRefreshKey] = useState(0)

  useEffect(() => {
    let isCancelled = false

    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const [usersRaw, metricsRaw, rolesRaw, goalsRaw, workoutsRaw, exercisesRaw] =
          await Promise.all([
            apiRequest('/api/User', { token }),
            apiRequest('/api/MetricsUser', { token }),
            apiRequest('/api/Role', { token }),
            apiRequest('/api/Goal', { token }),
            apiRequest('/api/Workout', { token }),
            apiRequest('/api/Exercise', { token }),
          ])

        const users = ensureArray(usersRaw)
        const metrics = ensureArray(metricsRaw)
        const roles = ensureArray(rolesRaw)
        const goals = ensureArray(goalsRaw)
        const workouts = ensureArray(workoutsRaw)
        const exercises = ensureArray(exercisesRaw)

        const roleNameById = new Map()
        const roleIdToName = {}
        roles.forEach((role) => {
          const idRole = toNullableNumber(getValue(role, 'idRole'))
          const roleName = String(getValue(role, 'roleName') ?? '')
          roleNameById.set(idRole, roleName)
          roleIdToName[idRole] = roleName
        })

        const roleDistribution = {}
        users.forEach((user) => {
          const roleId = toNullableNumber(getValue(user, 'roleId'))
          const roleName = roleNameById.get(roleId) ?? `Role ${roleId ?? 'N/A'}`
          roleDistribution[roleName] = (roleDistribution[roleName] || 0) + 1
        })

        const metricsById = new Map(
          metrics.map((item) => [toNullableNumber(getValue(item, 'idMetricsUser')), item]),
        )

        const usersWithMetrics = users.filter((user) => {
          const metricsId = toNullableNumber(getValue(user, 'metricsUserId'))
          return metricsById.has(metricsId)
        }).length

        const ages = metrics
          .map((item) => toNullableNumber(getValue(item, 'age')))
          .filter((age) => Number.isFinite(age))

        const avgAge = ages.length ? ages.reduce((sum, value) => sum + value, 0) / ages.length : null

        const ageBuckets = {
          '< 18': 0,
          '18 - 25': 0,
          '26 - 35': 0,
          '36 - 50': 0,
          '50+': 0,
        }

        ages.forEach((age) => {
          if (age < 18) {
            ageBuckets['< 18'] += 1
          } else if (age <= 25) {
            ageBuckets['18 - 25'] += 1
          } else if (age <= 35) {
            ageBuckets['26 - 35'] += 1
          } else if (age <= 50) {
            ageBuckets['36 - 50'] += 1
          } else {
            ageBuckets['50+'] += 1
          }
        })

        const genderDistribution = {}
        metrics.forEach((item) => {
          const gender = String(getValue(item, 'gender') ?? '').trim()
          const key = gender || 'Не указан'
          genderDistribution[key] = (genderDistribution[key] || 0) + 1
        })

        const activityDistribution = {}
        metrics.forEach((item) => {
          const activity = String(getValue(item, 'activityLevel') ?? '').trim()
          const key = activity || 'Не указан'
          activityDistribution[key] = (activityDistribution[key] || 0) + 1
        })

        if (!isCancelled) {
          setSnapshot({
            usersTotal: users.length,
            usersWithMetrics,
            avgAge,
            goalsTotal: goals.length,
            workoutsTotal: workouts.length,
            exercisesTotal: exercises.length,
            roleDistribution,
            roleIdToName,
            ageBuckets,
            genderDistribution,
            activityDistribution,
          })
        }
      } catch (loadError) {
        if (!isCancelled) {
          setError(loadError.message)
          setSnapshot(null)
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

  return (
    <section className="panel-block">
      <div className="panel-actions">
        <button
          type="button"
          className="btn btn-outline"
          onClick={() => setRefreshKey((value) => value + 1)}
          disabled={loading}
        >
          Обновить аналитику
        </button>
      </div>

      {loading ? <p>Загрузка аналитики...</p> : null}
      {error ? <div className="error-banner">{error}</div> : null}

      {!loading && !error && snapshot ? (
        <>
          <div className="stats-grid">
            <StatCard label="Пользователей" value={snapshot.usersTotal} />
            <StatCard label="С метриками" value={snapshot.usersWithMetrics} />
            <StatCard
              label="Средний возраст"
              value={snapshot.avgAge ? snapshot.avgAge.toFixed(1) : '—'}
            />
            <StatCard label="Целей" value={snapshot.goalsTotal} />
            <StatCard label="Тренировок" value={snapshot.workoutsTotal} />
            <StatCard label="Упражнений" value={snapshot.exercisesTotal} />
          </div>

          <div className="distribution-grid">
            <RoleDistributionChart
              data={snapshot.roleDistribution}
              roleNames={snapshot.roleIdToName}
            />
            <AgeDistributionChart data={snapshot.ageBuckets} />
            <GenderDistributionChart data={snapshot.genderDistribution} />
            <ActivityDistributionChart data={snapshot.activityDistribution} />
          </div>
        </>
      ) : null}
    </section>
  )
}

export default AnalyticsPanel
