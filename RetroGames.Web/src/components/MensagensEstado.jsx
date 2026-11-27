export function MensagemCarregando() {
  return (
    <div className="mensagem-estado" role="status">
      <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />
      Carregando...
    </div>
  )
}

export function MensagemErro({ mensagem }) {
  return (
    <div className="mensagem-estado mensagem-erro" role="alert">
      <i className="bi bi-exclamation-triangle me-2" aria-hidden="true" />
      {mensagem}
    </div>
  )
}
