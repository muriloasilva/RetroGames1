import { useEffect, useState } from 'react'
import {
  alterarPlataformaApi,
  excluirPlataformaApi,
  inserirPlataformaApi,
  listarPlataformasApi,
} from '../../api/services'
import { obterErrosValidacao, obterMensagemErro } from '../../api/errors'

const vazio = { nome: '', fabricante: '', anoLancamento: '' }

export default function Plataformas() {
  const [plataformas, setPlataformas] = useState([])
  const [formulario, setFormulario] = useState(vazio)
  const [editandoId, setEditandoId] = useState(null)
  const [erros, setErros] = useState({})
  const [erro, setErro] = useState('')

  async function carregar() {
    setErro('')
    try {
      setPlataformas(await listarPlataformasApi())
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  useEffect(() => {
    listarPlataformasApi()
      .then(setPlataformas)
      .catch((error) => setErro(obterMensagemErro(error)))
  }, [])

  function alterarCampo(event) {
    const { name, value } = event.target
    setFormulario((atual) => ({ ...atual, [name]: value }))
  }

  async function salvar(event) {
    event.preventDefault()
    setErros({})
    setErro('')

    const dados = {
      nome: formulario.nome,
      fabricante: formulario.fabricante,
      anoLancamento: Number(formulario.anoLancamento),
    }

    try {
      if (editandoId) await alterarPlataformaApi(editandoId, dados)
      else await inserirPlataformaApi(dados)
      setFormulario(vazio)
      setEditandoId(null)
      await carregar()
    } catch (error) {
      setErros(obterErrosValidacao(error))
      setErro(obterMensagemErro(error))
    }
  }

  function editar(plataforma) {
    setEditandoId(plataforma.id)
    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    })
  }

  async function excluir(id) {
    if (!window.confirm('Excluir esta plataforma?')) return
    try {
      await excluirPlataformaApi(id)
      await carregar()
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  function cancelar() {
    setEditandoId(null)
    setFormulario(vazio)
    setErros({})
  }

  return (
    <section className="admin-pagina">
      <div className="admin-cabecalho">
        <div>
          <p className="eyebrow">Administração</p>
          <h1>Plataformas</h1>
        </div>
      </div>

      {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

      <form className="formulario-admin" onSubmit={salvar} noValidate>
        <div>
          <label htmlFor="nome">Nome</label>
          <input id="nome" name="nome" value={formulario.nome} onChange={alterarCampo} />
          {erros.nome && <small className="erro-campo">{erros.nome}</small>}
        </div>
        <div>
          <label htmlFor="fabricante">Fabricante</label>
          <input id="fabricante" name="fabricante" value={formulario.fabricante} onChange={alterarCampo} />
          {erros.fabricante && <small className="erro-campo">{erros.fabricante}</small>}
        </div>
        <div>
          <label htmlFor="anoLancamento">Ano de lançamento</label>
          <input id="anoLancamento" name="anoLancamento" type="number" value={formulario.anoLancamento} onChange={alterarCampo} />
          {erros.anoLancamento && <small className="erro-campo">{erros.anoLancamento}</small>}
        </div>
        <div className="acoes-formulario">
          <button className="botao-buscar" type="submit">{editandoId ? 'Alterar' : 'Inserir'}</button>
          {editandoId && <button className="botao-limpar" type="button" onClick={cancelar}>Cancelar</button>}
        </div>
      </form>

      <div className="tabela-admin">
        <table>
          <thead>
            <tr><th>Nome</th><th>Fabricante</th><th>Ano</th><th>Ações</th></tr>
          </thead>
          <tbody>
            {plataformas.map((plataforma) => (
              <tr key={plataforma.id}>
                <td>{plataforma.nome}</td>
                <td>{plataforma.fabricante}</td>
                <td>{plataforma.anoLancamento}</td>
                <td>
                  <button type="button" onClick={() => editar(plataforma)} aria-label={`Editar ${plataforma.nome}`}><i className="bi bi-pencil" /></button>
                  <button type="button" onClick={() => excluir(plataforma.id)} aria-label={`Excluir ${plataforma.nome}`}><i className="bi bi-trash" /></button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  )
}
