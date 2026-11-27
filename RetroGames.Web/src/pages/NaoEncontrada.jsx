import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <section className="pagina-nao-encontrada">
      <span>404</span>
      <h1>Página não encontrada</h1>
      <Link className="botao-detalhe" to="/catalogo">Ir para o catálogo</Link>
    </section>
  )
}
