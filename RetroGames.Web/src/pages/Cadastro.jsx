import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { cadastrarUsuarioApi } from '../api/services'
import { obterErrosValidacao, obterMensagemErro } from '../api/errors'

export default function Cadastro() {
  const navigate = useNavigate()
  const [formulario, setFormulario] = useState({ nome: '', email: '', senha: '' })
  const [erros, setErros] = useState({})
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  function alterar(event) {
    const { name, value } = event.target
    setFormulario((atual) => ({ ...atual, [name]: value }))
  }

  async function enviar(event) {
    event.preventDefault()
    setErros({})
    setErro('')
    setEnviando(true)

    try {
      await cadastrarUsuarioApi(formulario)
      navigate('/login?cadastrado=1', { replace: true })
    } catch (error) {
      setErros(obterErrosValidacao(error))
      setErro(obterMensagemErro(error))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="pagina-login">
      <form className="formulario-login" onSubmit={enviar} noValidate>
        <p className="eyebrow">Nova conta</p>
        <h1>Cadastrar</h1>
        {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

        <label htmlFor="cadastro-nome">Nome</label>
        <input id="cadastro-nome" name="nome" value={formulario.nome} onChange={alterar} />
        {erros.nome && <small className="erro-campo">{erros.nome}</small>}

        <label htmlFor="cadastro-email">E-mail</label>
        <input id="cadastro-email" name="email" type="email" value={formulario.email} onChange={alterar} autoComplete="email" />
        {erros.email && <small className="erro-campo">{erros.email}</small>}

        <label htmlFor="cadastro-senha">Senha</label>
        <input id="cadastro-senha" name="senha" type="password" value={formulario.senha} onChange={alterar} autoComplete="new-password" />
        {erros.senha && <small className="erro-campo">{erros.senha}</small>}

        <button className="botao-buscar" type="submit" disabled={enviando}>{enviando ? 'Cadastrando...' : 'Cadastrar'}</button>
      </form>
    </section>
  )
}
