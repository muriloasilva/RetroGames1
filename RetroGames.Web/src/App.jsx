import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import { AutenticacaoProvider } from './contexts/AutenticacaoContext'
import Login from './pages/Login'
import RotaProtegida from './components/RotaProtegida'
import Plataformas from './pages/admin/Plataformas'
import Jogos from './pages/admin/Jogos'
import FormularioJogo from './pages/admin/FormularioJogo'

function App() {
  return (
    <BrowserRouter>
      <AutenticacaoProvider>
        <Routes>
        <Route element={<Layout />}>
          <Route index element={<Navigate to="/catalogo" replace />} />
          <Route path="/catalogo" element={<Catalogo />} />
          <Route path="/jogos/:id" element={<DetalheJogo />} />
          <Route path="/login" element={<Login />} />
          <Route element={<RotaProtegida somenteAdministrador />}>
            <Route path="/admin/jogos" element={<Jogos />} />
            <Route path="/admin/jogos/novo" element={<FormularioJogo />} />
            <Route path="/admin/jogos/:id/editar" element={<FormularioJogo />} />
            <Route path="/admin/plataformas" element={<Plataformas />} />
          </Route>
          <Route path="*" element={<NaoEncontrada />} />
        </Route>
        </Routes>
      </AutenticacaoProvider>
    </BrowserRouter>
  )
}

export default App
