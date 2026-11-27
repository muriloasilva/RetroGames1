import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { useCarrinho } from '../hooks/useCarrinho'

export default function Layout() {
  const { usuario, estaLogado, eAdministrador, sair } = useAutenticacao()
  const { quantidadeTotal } = useCarrinho()
  const navigate = useNavigate()

  async function fazerLogout() {
    await sair()
    navigate('/catalogo', { replace: true })
  }

  return (
    <>
      <header className="site-header">
        <nav className="navbar navbar-expand-md" aria-label="Navegação principal">
          <div className="container-fluid px-3 px-lg-4">
            <Link className="navbar-brand" to="/">
              <i className="bi bi-disc-fill me-2" aria-hidden="true" />
              RetroGames
            </Link>
            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#menuPrincipal"
              aria-controls="menuPrincipal"
              aria-expanded="false"
              aria-label="Abrir menu"
            >
              <i className="bi bi-list" aria-hidden="true" />
            </button>
            <div className="collapse navbar-collapse" id="menuPrincipal">
              <ul className="navbar-nav ms-auto">
                <li className="nav-item">
                  <NavLink className="nav-link" to="/catalogo">Catálogo</NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/carrinho">
                    Carrinho <span className="badge rounded-pill text-bg-light">{quantidadeTotal}</span>
                  </NavLink>
                </li>
                {estaLogado && !eAdministrador && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/meus-pedidos">Meus pedidos</NavLink>
                  </li>
                )}
                {eAdministrador && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/admin/jogos">Jogos</NavLink>
                  </li>
                )}
                {eAdministrador && (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/admin/plataformas">Plataformas</NavLink>
                  </li>
                )}
                {estaLogado ? (
                  <li className="nav-item menu-usuario">
                    <span>{usuario.nome}</span>
                    <button type="button" onClick={fazerLogout}>Sair</button>
                  </li>
                ) : (
                  <>
                    <li className="nav-item">
                      <Link className="nav-link" to="/cadastro">Cadastrar</Link>
                    </li>
                    <li className="nav-item">
                      <Link className="nav-link" to="/login">Entrar</Link>
                    </li>
                  </>
                )}
              </ul>
            </div>
          </div>
        </nav>
      </header>

      <main>
        <Outlet />
      </main>
    </>
  )
}
