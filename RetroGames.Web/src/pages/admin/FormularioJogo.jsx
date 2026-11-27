import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  alterarJogoApi,
  buscarJogoPorIdApi,
  inserirJogoApi,
  listarCondicoesApi,
  listarGenerosApi,
  listarPlataformasApi,
} from '../../api/services'
import { obterErrosValidacao, obterMensagemErro } from '../../api/errors'

const vazio = {
  nome: '', descricao: '', anoLancamento: '', preco: '', estoque: '',
  idPlataforma: '', idCondicaoJogo: '', idsGeneros: [], ativo: true,
}

export default function FormularioJogo() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [formulario, setFormulario] = useState(vazio)
  const [plataformas, setPlataformas] = useState([])
  const [condicoes, setCondicoes] = useState([])
  const [generos, setGeneros] = useState([])
  const [erros, setErros] = useState({})
  const [erro, setErro] = useState('')

  useEffect(() => {
    Promise.all([listarPlataformasApi(), listarCondicoesApi(), listarGenerosApi()])
      .then(([listaPlataformas, listaCondicoes, listaGeneros]) => {
        setPlataformas(listaPlataformas)
        setCondicoes(listaCondicoes)
        setGeneros(listaGeneros)
      })
      .catch((error) => setErro(obterMensagemErro(error)))

    if (id) {
      buscarJogoPorIdApi(id)
        .then((jogo) => setFormulario({
          nome: jogo.nome,
          descricao: jogo.descricao ?? '',
          anoLancamento: String(jogo.anoLancamento),
          preco: String(jogo.preco),
          estoque: String(jogo.estoque),
          idPlataforma: String(jogo.idPlataforma),
          idCondicaoJogo: String(jogo.idCondicaoJogo),
          idsGeneros: jogo.generos.map((genero) => genero.id),
          ativo: jogo.ativo,
        }))
        .catch((error) => setErro(obterMensagemErro(error)))
    }
  }, [id])

  function alterarCampo(event) {
    const { name, value } = event.target
    setFormulario((atual) => ({ ...atual, [name]: value }))
  }

  function alterarGeneros(event) {
    const selecionados = Array.from(event.target.selectedOptions, (opcao) => Number(opcao.value))
    setFormulario((atual) => ({ ...atual, idsGeneros: selecionados }))
  }

  async function salvar(event) {
    event.preventDefault()
    setErros({})
    setErro('')

    const dados = {
      nome: formulario.nome,
      descricao: formulario.descricao || null,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      ativo: formulario.ativo,
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
    }

    try {
      if (id) await alterarJogoApi(id, dados)
      else await inserirJogoApi(dados)
      navigate('/admin/jogos', { replace: true })
    } catch (error) {
      setErros(obterErrosValidacao(error))
      setErro(obterMensagemErro(error))
    }
  }

  return (
    <section className="admin-pagina">
      <div className="admin-cabecalho">
        <div><p className="eyebrow">Administração</p><h1>{id ? 'Alterar jogo' : 'Novo jogo'}</h1></div>
      </div>

      {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

      <form className="formulario-admin formulario-jogo" onSubmit={salvar} noValidate>
        <div><label htmlFor="nome">Nome</label><input id="nome" name="nome" value={formulario.nome} onChange={alterarCampo} />{erros.nome && <small className="erro-campo">{erros.nome}</small>}</div>
        <div><label htmlFor="anoLancamento">Ano</label><input id="anoLancamento" name="anoLancamento" type="number" value={formulario.anoLancamento} onChange={alterarCampo} />{erros.anoLancamento && <small className="erro-campo">{erros.anoLancamento}</small>}</div>
        <div><label htmlFor="preco">Preço</label><input id="preco" name="preco" type="number" step="0.01" value={formulario.preco} onChange={alterarCampo} />{erros.preco && <small className="erro-campo">{erros.preco}</small>}</div>
        <div><label htmlFor="estoque">Estoque</label><input id="estoque" name="estoque" type="number" value={formulario.estoque} onChange={alterarCampo} />{erros.estoque && <small className="erro-campo">{erros.estoque}</small>}</div>
        <div><label htmlFor="plataforma">Plataforma</label><select id="plataforma" name="idPlataforma" value={formulario.idPlataforma} onChange={alterarCampo}><option value="">Selecione</option>{plataformas.map((item) => <option key={item.id} value={item.id}>{item.nome}</option>)}</select>{erros.idPlataforma && <small className="erro-campo">{erros.idPlataforma}</small>}</div>
        <div><label htmlFor="condicao">Condição</label><select id="condicao" name="idCondicaoJogo" value={formulario.idCondicaoJogo} onChange={alterarCampo}><option value="">Selecione</option>{condicoes.map((item) => <option key={item.id} value={item.id}>{item.descricao}</option>)}</select>{erros.idCondicaoJogo && <small className="erro-campo">{erros.idCondicaoJogo}</small>}</div>
        <div className="campo-descricao"><label htmlFor="descricao">Descrição</label><textarea id="descricao" name="descricao" value={formulario.descricao} onChange={alterarCampo} />{erros.descricao && <small className="erro-campo">{erros.descricao}</small>}</div>
        <div className="campo-generos"><label htmlFor="generos">Gêneros</label><select id="generos" multiple size="6" value={formulario.idsGeneros.map(String)} onChange={alterarGeneros}>{generos.map((item) => <option key={item.id} value={item.id}>{item.nome}</option>)}</select>{erros.idsGeneros && <small className="erro-campo">{erros.idsGeneros}</small>}</div>

        <div className="acoes-formulario campo-acoes">
          <button className="botao-buscar" type="submit">Salvar</button>
          <button className="botao-limpar" type="button" onClick={() => navigate('/admin/jogos')}>Cancelar</button>
        </div>
      </form>
    </section>
  )
}
