import { useEffect, useState } from 'react'
import { CarrinhoContext } from './carrinhoContext'

const chave = 'retrogames.carrinho'

export function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(() => {
    try {
      return JSON.parse(localStorage.getItem(chave) ?? '{}')
    } catch {
      return {}
    }
  })

  useEffect(() => {
    localStorage.setItem(chave, JSON.stringify(itens))
  }, [itens])

  function adicionar(jogo, quantidade = 1) {
    setItens((atual) => {
      const atualQuantidade = atual[jogo.id]?.quantidade ?? 0
      return {
        ...atual,
        [jogo.id]: {
          id: jogo.id,
          nome: jogo.nome,
          preco: jogo.preco,
          estoque: jogo.estoque,
          quantidade: Math.min(atualQuantidade + quantidade, jogo.estoque),
        },
      }
    })
  }

  function alterarQuantidade(id, quantidade) {
    setItens((atual) => {
      const item = atual[id]
      if (!item) return atual
      if (quantidade <= 0) {
        const novo = { ...atual }
        delete novo[id]
        return novo
      }
      return { ...atual, [id]: { ...item, quantidade: Math.min(quantidade, item.estoque) } }
    })
  }

  function remover(id) {
    setItens((atual) => {
      const novo = { ...atual }
      delete novo[id]
      return novo
    })
  }

  function limpar() {
    setItens({})
  }

  const lista = Object.values(itens)
  const quantidadeTotal = lista.reduce((total, item) => total + item.quantidade, 0)
  const valorTotal = lista.reduce((total, item) => total + item.preco * item.quantidade, 0)

  return (
    <CarrinhoContext.Provider
      value={{ itens: lista, quantidadeTotal, valorTotal, adicionar, alterarQuantidade, remover, limpar }}
    >
      {children}
    </CarrinhoContext.Provider>
  )
}
