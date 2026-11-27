import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route index element={<Navigate to="/catalogo" replace />} />
          <Route path="/catalogo" element={<Catalogo />} />
          <Route path="/jogos/:id" element={<DetalheJogo />} />
          <Route path="*" element={<NaoEncontrada />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
