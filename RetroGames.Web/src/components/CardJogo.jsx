import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

export default function CardJogo({ jogo }) {
  return (
    <article className="card-jogo">
      <div className="card-jogo-cabecalho">
        <span className="condicao">{jogo.descricaoCondicao}</span>
        <span className="ano">{jogo.anoLancamento}</span>
      </div>
      <h2>{jogo.nome}</h2>
      <p className="plataforma">{jogo.nomePlataforma}</p>
      <div className="generos" aria-label="Gêneros">
        {jogo.generos.map((genero) => (
          <span key={genero}>{genero}</span>
        ))}
      </div>
      <div className="card-jogo-rodape">
        <strong>{formatarPreco(jogo.preco)}</strong>
        <Link className="botao-detalhe" to={`/jogos/${jogo.id}`}>
          Ver detalhes
        </Link>
      </div>
    </article>
  )
}
