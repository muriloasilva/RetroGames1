import { useEffect, useState } from 'react'
import { autenticarApi, obterUsuarioAtualApi, sairApi } from '../api/services'

import { AutenticacaoContext } from './autenticacaoContext'

export function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let ativo = true

    obterUsuarioAtualApi()
      .then((dados) => {
        if (ativo) setUsuario(dados)
      })
      .catch(() => {
        if (ativo) setUsuario(null)
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => { ativo = false }
  }, [])

  async function entrar(email, senha) {
    const dados = await autenticarApi(email, senha)
    setUsuario(dados)
    return dados
  }

  useEffect(() => {
    function limparSessaoExpirada() {
      setUsuario(null)
    }

    window.addEventListener('sessao:expirada', limparSessaoExpirada)
    return () => window.removeEventListener('sessao:expirada', limparSessaoExpirada)
  }, [])

  async function sair() {
    await sairApi()
    setUsuario(null)
  }

  async function recarregarUsuario() {
    try {
      const dados = await obterUsuarioAtualApi()
      setUsuario(dados)
    } catch {
      setUsuario(null)
    }
  }

  return (
    <AutenticacaoContext.Provider
      value={{
        usuario,
        carregando,
        entrar,
        sair,
        recarregarUsuario,
        estaLogado: usuario !== null,
        eAdministrador: usuario?.perfil === 'Administrador',
      }}
    >
      {children}
    </AutenticacaoContext.Provider>
  )
}

