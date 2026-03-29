import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import * as THREE from 'three'

THREE.Object3D.DEFAULT_UP.set(0, 0, 1)
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
