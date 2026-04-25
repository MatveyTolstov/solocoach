function DistributionCard({ title, data }) {
  const entries = Object.entries(data)

  return (
    <div className="distribution-card">
      <h3>{title}</h3>
      <ul>
        {entries.map(([label, count]) => (
          <li key={label}>
            {label}: {count}
          </li>
        ))}
      </ul>
    </div>
  )
}

export default DistributionCard
