import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { listarMeusPedidosApi } from '../api/services'
import { obterMensagemErro } from '../api/errors'
import { formatarPreco } from '../lib/formato'

export default function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])
  const [erro, setErro] = useState('')

  useEffect(() => {
    listarMeusPedidosApi()
      .then(setPedidos)
      .catch((error) => setErro(obterMensagemErro(error)))
  }, [])

  return (
    <section className="admin-pagina">
      <div className="admin-cabecalho"><div><p className="eyebrow">Conta</p><h1>Meus pedidos</h1></div></div>
      {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

      {pedidos.length === 0 && !erro && <div className="mensagem-estado">Nenhum pedido encontrado.</div>}

      {pedidos.map((pedido) => (
        <article className="pedido-cliente" key={pedido.id}>
          <div className="pedido-cabecalho">
            <strong>Pedido #{pedido.id}</strong>
            <span>{pedido.situacao}</span>
          </div>
          <p>{new Date(pedido.dataPedido).toLocaleString('pt-BR')}</p>
          <ul>
            {pedido.itens.map((item) => (
              <li key={item.idJogo}>{item.nomeJogo} × {item.quantidade} — {formatarPreco(item.subtotal)}</li>
            ))}
          </ul>
          <strong>Total: {formatarPreco(pedido.total)}</strong>
        </article>
      ))}

      <Link className="botao-voltar" to="/catalogo">Continuar comprando</Link>
    </section>
  )
}
