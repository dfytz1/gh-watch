import { Canvas } from "@react-three/fiber";
import { OrbitControls, Grid } from "@react-three/drei";
import { DoubleSide } from "three";
import GeometryView from "./geometry-views/geometry-view";
import Toolbar from "./components/toolbar";

function Scene() {
  return (
    <>
      <ambientLight intensity={3.5} />
      {/* <directionalLight position={[10, 10, 10]} intensity={1.75} /> */}

      <Grid
        rotation={[-Math.PI / 2, 0, 0]}
        args={[20, 20]}
        cellSize={1}
        cellThickness={0.5}
        cellColor="#6b7280"
        sectionSize={5}
        sectionThickness={1}
        sectionColor="#9ca3af"
        fadeDistance={100}
        fadeStrength={0}
        followCamera={false}
        infiniteGrid={false}
        side={DoubleSide}
      />
      <axesHelper args={[5]} />

      <GeometryView />

      <OrbitControls makeDefault />
    </>
  );
}

export default function App() {
  return (
    <div style={{ width: "100vw", height: "100vh", background: "#1a1a1a", position: "relative" }}>
      <Canvas camera={{ position: [8, -8, 6], fov: 50 }}>
        <Scene />
      </Canvas>
      <Toolbar />
    </div>
  );
}
