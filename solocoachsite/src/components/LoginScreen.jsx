import { useState } from 'react'

function LoginScreen({ onLogin, isBusy, error }) {
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')

  const submit = async (event) => {
    event.preventDefault()
    await onLogin(login.trim(), password)
  }

  return (
    <div className="login-page">
      <form className="login-card" onSubmit={submit}>
        <h1 className="login-title">SoloCoach</h1>
        <p className="login-subtitle">Панель менеджеров и администраторов</p>
        {error ? <div className="error-banner">{error}</div> : null}

        <label className="field">
          <span>Логин</span>
          <input
            type="text"
            value={login}
            onChange={(event) => setLogin(event.target.value)}
            required
            autoComplete="username"
          />
        </label>

        <label className="field">
          <span>Пароль</span>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
            autoComplete="current-password"
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
