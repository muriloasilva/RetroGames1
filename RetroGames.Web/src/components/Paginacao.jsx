export default function Paginacao({ pagina, totalPaginas, onMudarPagina }) {
  if (totalPaginas <= 1) return null

  return (
    <nav className="paginacao" aria-label="Paginação do catálogo">
      <button
        type="button"
        className="botao-pagina"
        disabled={pagina <= 1}
        onClick={() => onMudarPagina(pagina - 1)}
        aria-label="Página anterior"
      >
        <i className="bi bi-chevron-left" aria-hidden="true" />
      </button>

      {Array.from({ length: totalPaginas }, (_, index) => index + 1).map((numero) => (
        <button
          type="button"
          key={numero}
          className={`botao-pagina ${numero === pagina ? 'ativa' : ''}`}
          onClick={() => onMudarPagina(numero)}
          aria-current={numero === pagina ? 'page' : undefined}
        >
          {numero}
        </button>
      ))}

      <button
        type="button"
        className="botao-pagina"
        disabled={pagina >= totalPaginas}
        onClick={() => onMudarPagina(pagina + 1)}
        aria-label="Próxima página"
      >
        <i className="bi bi-chevron-right" aria-hidden="true" />
      </button>
    </nav>
  )
}
