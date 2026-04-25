import { useEffect, useRef } from 'react'
import Chart from 'chart.js/auto'

function GenderDistributionChart({ data }) {
  const chartRef = useRef(null)
  const chartInstance = useRef(null)

  useEffect(() => {
    if (!chartRef.current || !data) {
      return
    }

    const labels = Object.keys(data)
    const values = Object.values(data)

    if (chartInstance.current) {
      chartInstance.current.destroy()
    }

    const ctx = chartRef.current.getContext('2d')
    chartInstance.current = new Chart(ctx, {
      type: 'pie',
      data: {
        labels,
        datasets: [
          {
            data: values,
            backgroundColor: ['#ec4899', '#3b82f6', '#6b7280'],
            borderColor: '#ffffff',
            borderWidth: 2,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              usePointStyle: true,
              padding: 15,
              font: { size: 12 },
            },
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                const total = context.dataset.data.reduce((a, b) => a + b, 0)
                const percentage = ((context.parsed / total) * 100).toFixed(1)
                return `${context.label}: ${context.parsed} (${percentage}%)`
              },
            },
          },
        },
      },
    })

    return () => {
      if (chartInstance.current) {
        chartInstance.current.destroy()
      }
    }
  }, [data])

  return (
    <div className="distribution-card">
      <h3>Распределение по полу</h3>
      <div className="chart-canvas">
        <canvas ref={chartRef} />
      </div>
    </div>
  )
}

export default GenderDistributionChart
