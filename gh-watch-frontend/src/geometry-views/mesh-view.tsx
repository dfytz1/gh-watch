import { useEffect, useMemo } from "react";
import {
  BufferGeometry,
  Float32BufferAttribute,
  Uint32BufferAttribute,
} from "three";
import { mergeGeometries } from "three/examples/jsm/utils/BufferGeometryUtils.js";
import type { IMeshPayload } from "../props/payload-props/mesh-props";

interface Props {
  meshes: IMeshPayload[];
}

function buildGeometry({
  vertices,
  normals,
  faces,
}: IMeshPayload): BufferGeometry {
  const geo = new BufferGeometry();
  geo.setAttribute(
    "position",
    new Float32BufferAttribute(new Float32Array(vertices), 3),
  );
  geo.setAttribute(
    "normal",
    new Float32BufferAttribute(new Float32Array(normals), 3),
  );
  geo.setIndex(new Uint32BufferAttribute(new Uint32Array(faces), 1));
  return geo;
}

const MeshView = ({ meshes }: Props) => {
  const geometry = useMemo<BufferGeometry | null>(() => {
    if (meshes.length === 0) return null;
    const individual = meshes.map(buildGeometry);
    const merged = mergeGeometries(individual);
    individual.forEach((g) => g.dispose());
    return merged;
  }, [meshes]);

  useEffect(() => {
    return () => {
      geometry?.dispose();
    };
  }, [geometry]);

  if (!geometry) return null;

  return (
    <mesh geometry={geometry}>
      <meshStandardMaterial color="#888888" side={2} transparent opacity={0.5} />
    </mesh>
  );
};

export default MeshView;
