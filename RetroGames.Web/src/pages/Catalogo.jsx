import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { listarGenerosApi, listarJogosApi, listarPlataformasApi } from '../api/services'
import { obterMensagemErro } from '../api/errors'
import CardJogo from '../components/CardJogo'
import Paginacao from '../components/Paginacao'
import { MensagemCarregando, MensagemErro } from '../components/MensagensEstado'

export default function Catalogo() {
  const [params, setParams] = useSearchParams()
  const busca = params.get('busca') ?? ''
  const idPlataforma = params.get('idPlataforma') ?? ''
  const idGenero = params.get('idGenero') ?? ''
  const pagina = Math.max(1, Number(params.get('pagina') ?? 1) || 1)

  const [buscaAtual, setBuscaAtual] = useState(busca)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  useEffect(() => {
    Promise.all([listarPlataformasApi(), listarGenerosApi()])
      .then(([listaPlataformas, listaGeneros]) => {
        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
      })
      .catch((error) => setErro(obterMensagemErro(error)))
  }, [])

  useEffect(() => {
    let ativo = true

    listarJogosApi({
      busca: busca || undefined,
      idPlataforma: idPlataforma || undefined,
      idGenero: idGenero || undefined,
      pagina,
      tamanhoPagina: 12,
    })
      .then((dados) => {
        if (ativo) setResultado(dados)
      })
      .catch((error) => {
        if (ativo) setErro(obterMensagemErro(error))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => { ativo = false }
  }, [busca, idPlataforma, idGenero, pagina])

  function alterarParametro(nome, valor) {
    setCarregando(true)
    setErro('')
    const novos = new URLSearchParams(params)
    if (valor) novos.set(nome, valor)
    else novos.delete(nome)
    novos.set('pagina', '1')
    setParams(novos)
  }

  function buscar(event) {
    event.preventDefault()
    alterarParametro('busca', buscaAtual.trim())
  }

  function limparFiltros() {
    setCarregando(true)
    setErro('')
    setBuscaAtual('')
    setParams(new URLSearchParams())
  }

  function mudarPagina(novaPagina) {
    setCarregando(true)
    setErro('')
    const novos = new URLSearchParams(params)
    novos.set('pagina', String(novaPagina))
    setParams(novos)
  }

  return (
    <section className="catalogo-pagina">
      <div className="catalogo-cabecalho">
        <div>
          <p className="eyebrow">Coleção disponível</p>
          <h1>Catálogo</h1>
        </div>
        <span className="total-jogos">
          {resultado ? `${resultado.totalItens} jogos` : '...' }
        </span>
      </div>

      <form className="filtros" onSubmit={buscar}>
        <div className="campo-busca">
          <label htmlFor="busca">Buscar</label>
          <div className="entrada-com-icone">
            <i className="bi bi-search" aria-hidden="true" />
            <input
              id="busca"
              type="search"
              value={buscaAtual}
              onChange={(event) => setBuscaAtual(event.target.value)}
              placeholder="Nome do jogo"
            />
          </div>
        </div>

        <div className="campo-filtro">
          <label htmlFor="plataforma">Plataforma</label>
          <select
            id="plataforma"
            value={idPlataforma}
            onChange={(event) => alterarParametro('idPlataforma', event.target.value)}
          >
            <option value="">Todas</option>
            {plataformas.map((item) => (
              <option key={item.id} value={item.id}>{item.nome}</option>
            ))}
          </select>
        </div>

        <div className="campo-filtro">
          <label htmlFor="genero">Gênero</label>
          <select
            id="genero"
            value={idGenero}
            onChange={(event) => alterarParametro('idGenero', event.target.value)}
          >
            <option value="">Todos</option>
            {generos.map((item) => (
              <option key={item.id} value={item.id}>{item.nome}</option>
            ))}
          </select>
        </div>

        <button className="botao-buscar" type="submit">
          <i className="bi bi-search me-2" aria-hidden="true" />
          Buscar
        </button>
        <button className="botao-limpar" type="button" onClick={limparFiltros}>
          Limpar
        </button>
      </form>

      {carregando && <MensagemCarregando />}
      {!carregando && erro && <MensagemErro mensagem={erro} />}
      {!carregando && !erro && resultado?.itens.length === 0 && (
        <MensagemErro mensagem="Nenhum jogo encontrado para os filtros selecionados." />
      )}

      {!carregando && !erro && resultado?.itens.length > 0 && (
        <>
          <div className="grade-jogos">
            {resultado.itens.map((jogo) => <CardJogo key={jogo.id} jogo={jogo} />)}
          </div>
          <Paginacao
            pagina={resultado.pagina}
            totalPaginas={resultado.totalPaginas}
            onMudarPagina={mudarPagina}
          />
        </>
      )}
    </section>
  )
}
