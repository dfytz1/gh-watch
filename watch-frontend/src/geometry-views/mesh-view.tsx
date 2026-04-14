import { useEffect, useRef } from "react";
import { BufferGeometry, Float32BufferAttribute, Int32BufferAttribute, Mesh, MeshStandardMaterial } from "three";
import type { IMeshPayload } from "../props/payload-props/IMeshPayload";

interface MeshViewProps {
  payload: IMeshPayload;
}

const MeshView: React.FC<MeshViewProps> = ({ payload }) => {
  const meshRef = useRef<Mesh>(null);

  useEffect(() => {
    const mesh = meshRef.current;
    if (!mesh) return;

    const prev = mesh.geometry;

    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(new Float32Array(payload.vertices), 3));
    geo.setAttribute("normal", new Float32BufferAttribute(new Float32Array(payload.normals), 3));
    geo.setIndex(new Int32BufferAttribute(new Int32Array(payload.faces), 1));

    mesh.geometry = geo;

    prev.dispose();
  }, [payload]);

  return (
    <mesh ref={meshRef}>
      <bufferGeometry />
      <meshStandardMaterial color="#888888" transparent opacity={0.5} side={2} />
    </mesh>
  );
};

export default MeshView;
