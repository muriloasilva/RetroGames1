import { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { obterMensagemErro } from '../api/errors'

export default function Login() {
  const { entrar, estaLogado, eAdministrador } = useAutenticacao()
  const navigate = useNavigate()
  const [params] = useSearchParams()
  const destino = params.get('voltarPara') || '/catalogo'

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    if (estaLogado) {
      navigate(eAdministrador ? destino : '/catalogo', { replace: true })
    }
  }, [estaLogado, eAdministrador, navigate, destino])

  async function enviar(event) {
    event.preventDefault()
    setErro('')

    if (!email.trim() || !senha) {
      setErro('E-mail ou senha inválidos.')
      return
    }

    setEnviando(true)
    try {
      await entrar(email.trim(), senha)
      navigate(destino, { replace: true })
    } catch (error) {
      setErro(obterMensagemErro(error))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="pagina-login">
      <form className="formulario-login" onSubmit={enviar} noValidate>
        <p className="eyebrow">Acesso administrativo</p>
        <h1>Entrar</h1>

        {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

        <label htmlFor="login-email">E-mail</label>
        <input
          id="login-email"
          type="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          autoComplete="email"
        />

        <label htmlFor="login-senha">Senha</label>
        <input
          id="login-senha"
          type="password"
          value={senha}
          onChange={(event) => setSenha(event.target.value)}
          autoComplete="current-password"
        />

        <button className="botao-buscar" type="submit" disabled={enviando}>
          {enviando ? 'Entrando...' : 'Entrar'}
        </button>
      </form>
    </section>
  )
}
