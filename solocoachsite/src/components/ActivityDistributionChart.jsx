import { useEffect, useRef } from 'react'
import Chart from 'chart.js/auto'

function ActivityDistributionChart({ data }) {
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
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: 'Количество пользователей',
            data: values,
            backgroundColor: [
              'rgba(16, 185, 129, 0.6)',
              'rgba(34, 197, 94, 0.6)',
              'rgba(245, 158, 11, 0.6)',
              'rgba(239, 68, 68, 0.6)',
            ],
            borderColor: ['#10b981', '#22c55e', '#f59e0b', '#ef4444'],
            borderWidth: 1,
            borderRadius: 4,
          },
        ],
      },
      options: {
        indexAxis: 'x',
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false,
          },
          tooltip: {
            callbacks: {
              label: (context) => `Пользователей: ${context.parsed.y}`,
            },
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 1,
            },
            grid: {
              color: 'rgba(0, 0, 0, 0.05)',
            },
          },
          x: {
            grid: {
              display: false,
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
      <h3>Уровень активности</h3>
      <div className="chart-canvas">
        <canvas ref={chartRef} />
      </div>
    </div>
  )
}

export default ActivityDistributionChart
