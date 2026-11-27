export function obterMensagemErro(error) {
  const envelope = error.response?.data
  if (typeof envelope?.errorMessage === 'string' && envelope.errorMessage) {
    return envelope.errorMessage
  }

  return 'Não foi possível conectar ao servidor.'
}
