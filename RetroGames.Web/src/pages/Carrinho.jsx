import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { criarPedidoApi } from '../api/services'
import { obterMensagemErro } from '../api/errors'
import { useCarrinho } from '../hooks/useCarrinho'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { formatarPreco } from '../lib/formato'

export default function Carrinho() {
  const { itens, valorTotal, alterarQuantidade, remover, limpar } = useCarrinho()
  const { estaLogado } = useAutenticacao()
  const navigate = useNavigate()
  const [erro, setErro] = useState('')
  const [finalizando, setFinalizando] = useState(false)

  async function finalizar() {
    if (!estaLogado) {
      navigate('/login?voltarPara=/carrinho')
      return
    }

    setErro('')
    setFinalizando(true)
    const pedidoItens = Object.fromEntries(itens.map((item) => [item.id, item.quantidade]))

    try {
      await criarPedidoApi(pedidoItens)
      limpar()
      navigate('/meus-pedidos', { replace: true })
    } catch (error) {
      setErro(obterMensagemErro(error))
    } finally {
      setFinalizando(false)
    }
  }

  if (itens.length === 0) {
    return (
      <section className="pagina-detalhe">
        <h1>Carrinho</h1>
        <div className="mensagem-estado">Seu carrinho está vazio.</div>
        <Link className="botao-voltar" to="/catalogo">Ir ao catálogo</Link>
      </section>
    )
  }

  return (
    <section className="admin-pagina">
      <div className="admin-cabecalho"><div><p className="eyebrow">Compra</p><h1>Carrinho</h1></div></div>
      {erro && <div className="mensagem-erro-inline" role="alert">{erro}</div>}

      <div className="tabela-admin">
        <table>
          <thead><tr><th>Jogo</th><th>Preço</th><th>Quantidade</th><th>Subtotal</th><th /></tr></thead>
          <tbody>
            {itens.map((item) => (
              <tr key={item.id}>
                <td>{item.nome}</td>
                <td>{formatarPreco(item.preco)}</td>
                <td>
                  <input
                    className="quantidade-carrinho"
                    type="number"
                    min="1"
                    max={item.estoque}
                    value={item.quantidade}
                    onChange={(event) => alterarQuantidade(item.id, Number(event.target.value))}
                  />
                </td>
                <td>{formatarPreco(item.preco * item.quantidade)}</td>
                <td><button type="button" onClick={() => remover(item.id)} aria-label={`Remover ${item.nome}`}><i className="bi bi-trash" /></button></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="carrinho-finalizar">
        <strong>Total: {formatarPreco(valorTotal)}</strong>
        <button className="botao-buscar" type="button" onClick={finalizar} disabled={finalizando}>{finalizando ? 'Finalizando...' : 'Finalizar compra'}</button>
      </div>
    </section>
  )
}
