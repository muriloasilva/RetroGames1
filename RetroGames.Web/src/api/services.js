import api from './api'

export async function listarPlataformasApi() {
  const response = await api.get('/Plataforma')
  return response.data.data
}

export async function listarGenerosApi() {
  const response = await api.get('/Genero')
  return response.data.data
}

export async function listarJogosApi(filtros) {
  const response = await api.get('/Jogo', { params: filtros })
  return response.data.data
}

export async function buscarJogoPorIdApi(id) {
  const response = await api.get(`/Jogo/${id}`)
  return response.data.data
}

export async function autenticarApi(email, senha) {
  const response = await api.post('/Autenticacao/login', { email, senha })
  return response.data.data
}

export async function sairApi() {
  const response = await api.post('/Autenticacao/logout')
  return response.data
}

export async function obterUsuarioAtualApi() {
  const response = await api.get('/Autenticacao/eu')
  return response.data.data
}

export async function inserirPlataformaApi(dados) {
  const response = await api.post('/Plataforma', dados)
  return response.data.data
}

export async function alterarPlataformaApi(id, dados) {
  const response = await api.put(`/Plataforma/${id}`, dados)
  return response.data.data
}

export async function excluirPlataformaApi(id) {
  const response = await api.delete(`/Plataforma/${id}`)
  return response.data
}

export async function listarCondicoesApi() {
  const response = await api.get('/CondicaoJogo')
  return response.data.data
}

export async function inserirJogoApi(dados) {
  const response = await api.post('/Jogo', dados)
  return response.data.data
}

export async function alterarJogoApi(id, dados) {
  const response = await api.put(`/Jogo/${id}`, dados)
  return response.data.data
}

export async function desativarJogoApi(id) {
  const response = await api.delete(`/Jogo/${id}`)
  return response.data
}
