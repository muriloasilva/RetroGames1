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
