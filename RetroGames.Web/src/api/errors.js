export function obterErrosValidacao(error) {
  const dados = error.response?.data?.data
  if (dados && typeof dados === 'object') {
    return Object.fromEntries(
      Object.entries(dados).map(([campo, mensagens]) => [campo, mensagens[0] ?? 'Valor inválido.']),
    )
  }
  return {}
}

export function obterMensagemErro(error) {
  const envelope = error.response?.data
  if (typeof envelope?.errorMessage === 'string' && envelope.errorMessage) {
    return envelope.errorMessage
  }

  return 'Não foi possível conectar ao servidor.'
}
