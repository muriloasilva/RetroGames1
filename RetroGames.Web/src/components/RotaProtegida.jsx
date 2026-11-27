import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'

export default function RotaProtegida({ somenteAdministrador = false }) {
  const { estaLogado, eAdministrador, carregando } = useAutenticacao()
  const location = useLocation()

  if (carregando) return null

  if (!estaLogado) {
    return <Navigate to={`/login?voltarPara=${encodeURIComponent(location.pathname)}`} replace />
  }

  if (somenteAdministrador && !eAdministrador) {
    return <Navigate to="/catalogo" replace />
  }

  return <Outlet />
}
