import { useState } from 'react'

function LoginScreen({ onLogin, isBusy, error }) {
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')

  const submit = async (e) => {
    e.preventDefault()
    await onLogin(login.trim(), password)
  }

  return (
    <div className="login-page">
      <form className="login-card" onSubmit={submit}>
        <div className="login-logo">
          <div className="login-logo-mark">SC</div>
          <span className="login-title">SoloCoach</span>
        </div>
        <p className="login-subtitle">Панель управления</p>

        {error && <div className="error-banner">{error}</div>}

        <label className="field">
          <span className="field-label">Логин</span>
          <input
            type="text"
            value={login}
            onChange={(e) => setLogin(e.target.value)}
            required
            autoComplete="username"
            placeholder="Введите логин"
          />
        </label>

        <label className="field">
          <span className="field-label">Пароль</span>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="current-password"
            placeholder="Введите пароль"
          />
        </label>

        <button type="submit" className="btn btn-primary" disabled={isBusy}>
          {isBusy ? 'Вход...' : 'Войти'}
        </button>
      </form>
    </div>
  )
}

export default LoginScreen
