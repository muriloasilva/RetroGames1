import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { desativarJogoApi, listarJogosApi } from '../../api/services'
import { obterMensagemErro } from '../../api/errors'
import { formatarPreco } from '../../lib/formato'

export default function Jogos() {
  const [resultado, setResultado] = useState(null)
  const [erro, setErro] = useState('')

  async function carregar() {
    setErro('')
    try {
      setResultado(await listarJogosApi({ pagina: 1, tamanhoPagina: 100 }))
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  useEffect(() => {
    listarJogosApi({ pagina: 1, tamanhoPagina: 100 })
      .then(setResultado)
      .catch((error) => setErro(obterMensagemErro(error)))
  }, [])

  async function desativar(jogo) {
    if (!window.confirm(`Desativar ${jogo.nome}?`)) return
    try {
      await desativarJogoApi(jogo.id)
      await carregar()
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  return (
    <section className="admin-pagina">
      <div className="admin-cabecalho">
        <div><p className="eyebrow">Administração</p><h1>Jogos</h1></div>
        <Link className="botao-buscar" to="/admin/jogos/novo">Novo jogo</Link>
      </div>

      {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

      <div className="tabela-admin">
        <table>
          <thead><tr><th>Nome</th><th>Plataforma</th><th>Preço</th><th>Estoque</th><th>Status</th><th>Ações</th></tr></thead>
          <tbody>
            {resultado?.itens.map((jogo) => (
              <tr key={jogo.id} className={!jogo.ativo ? 'linha-inativo' : ''}>
                <td>{jogo.nome}</td>
                <td>{jogo.nomePlataforma}</td>
                <td>{formatarPreco(jogo.preco)}</td>
                <td>{jogo.estoque}</td>
                <td>{jogo.ativo ? 'Ativo' : 'Inativo'}</td>
                <td>
                  <Link to={`/admin/jogos/${jogo.id}/editar`} aria-label={`Editar ${jogo.nome}`}><i className="bi bi-pencil" /></Link>
                  {jogo.ativo && (
                    <button type="button" onClick={() => desativar(jogo)} aria-label={`Desativar ${jogo.nome}`}><i className="bi bi-trash" /></button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  )
}
