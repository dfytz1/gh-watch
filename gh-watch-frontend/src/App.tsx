import { Canvas } from "@react-three/fiber";
import { OrbitControls, Grid, GizmoHelper, GizmoViewport } from "@react-three/drei";
import { DoubleSide } from "three";
import GeometryView from "./geometry-views/geometry-view";
import Toolbar from "./components/toolbar";
import ZoomToFit from "./components/zoom-to-fit";
import CameraManager from "./components/camera-manager";

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
      <ZoomToFit />
      <CameraManager />
      <GizmoHelper alignment="bottom-right" margin={[60, 60]}>
        <GizmoViewport axisColors={["#ff4060", "#80ff80", "#4080ff"]} labelColor="white" />
      </GizmoHelper>
    </>
  );
}

export default function App() {
  return (
    <div
      style={{
        width: "100vw",
        height: "100vh",
        background: "#ffffff",
        position: "relative",
      }}
    >
      <Canvas camera={{ position: [8, -8, 6], fov: 50 }}>
        <Scene />
      </Canvas>
      <Toolbar />
    </div>
  );
}
