import { Link, NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
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
