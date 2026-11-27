import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { buscarJogoPorIdApi } from '../api/services'
import { obterMensagemErro } from '../api/errors'
import { MensagemCarregando, MensagemErro } from '../components/MensagensEstado'
import { formatarPreco } from '../lib/formato'
import { useCarrinho } from '../hooks/useCarrinho'

export default function DetalheJogo() {
  const { id } = useParams()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const { adicionar } = useCarrinho()

  useEffect(() => {
    let ativo = true

    buscarJogoPorIdApi(id)
      .then((dados) => {
        if (ativo) setJogo(dados)
      })
      .catch((error) => {
        if (ativo) setErro(obterMensagemErro(error))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => { ativo = false }
  }, [id])

  if (carregando) return <MensagemCarregando />

  if (erro) {
    return (
      <section className="pagina-detalhe">
        <MensagemErro mensagem={erro} />
        <Link className="botao-voltar" to="/catalogo">
          <i className="bi bi-arrow-left me-2" aria-hidden="true" />
          Voltar ao catálogo
        </Link>
      </section>
    )
  }

  return (
    <section className="pagina-detalhe">
      <Link className="botao-voltar" to="/catalogo">
        <i className="bi bi-arrow-left me-2" aria-hidden="true" />
        Voltar ao catálogo
      </Link>

      <article className="detalhe-jogo">
        <div className="detalhe-identidade">
          <p className="eyebrow">{jogo.nomePlataforma}</p>
          <h1>{jogo.nome}</h1>
          <div className="detalhe-meta">
            <span>{jogo.anoLancamento}</span>
            <span>{jogo.descricaoCondicao}</span>
          </div>
        </div>

        <div className="detalhe-conteudo">
          <p className="detalhe-descricao">
            {jogo.descricao || 'Descrição não informada.'}
          </p>

          <dl className="detalhe-ficha">
            <div>
              <dt>Plataforma</dt>
              <dd>{jogo.nomePlataforma}</dd>
            </div>
            <div>
              <dt>Condição</dt>
              <dd>{jogo.descricaoCondicao}</dd>
            </div>
            <div>
              <dt>Gêneros</dt>
              <dd>{jogo.generos.map((genero) => genero.nome).join(', ')}</dd>
            </div>
            <div>
              <dt>Estoque</dt>
              <dd>{jogo.estoque}</dd>
            </div>
          </dl>

          <div className="detalhe-preco">
            <span>Preço</span>
            <strong>{formatarPreco(jogo.preco)}</strong>
          </div>
          <button
            type="button"
            className="botao-buscar mt-3"
            onClick={() => adicionar(jogo)}
            disabled={jogo.estoque <= 0}
          >
            Adicionar ao carrinho
          </button>
        </div>
      </article>
    </section>
  )
}
