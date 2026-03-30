import { useEffect, useMemo } from "react";
import { useViewSettings } from "../store/view-settings";
import {
  BufferGeometry,
  EdgesGeometry,
  Float32BufferAttribute,
  Uint32BufferAttribute,
} from "three";
import { mergeGeometries } from "three/examples/jsm/utils/BufferGeometryUtils.js";
import type { IMeshPayload } from "../props/payload-props/mesh-props";

interface Props {
  meshes: IMeshPayload[];
}

interface Geos {
  mesh: BufferGeometry;
  edges: BufferGeometry;
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
  const showEdges = useViewSettings((s) => s.showEdges);
  const geos = useMemo<Geos | null>(() => {
    if (meshes.length === 0) return null;
    const individual = meshes.map(buildGeometry);
    const merged = mergeGeometries(individual);
    individual.forEach((g) => g.dispose());
    const edges = new EdgesGeometry(merged);
    return { mesh: merged, edges };
  }, [meshes]);

  // Dispose GPU resources whenever geos is replaced or the component unmounts
  useEffect(() => {
    return () => {
      geos?.mesh.dispose();
      geos?.edges.dispose();
    };
  }, [geos]);

  if (!geos) return null;

  return (
    <group>
      <mesh geometry={geos.mesh}>
        <meshStandardMaterial
          color="#888888"
          side={2}
          transparent
          opacity={0.5}
        />
      </mesh>
      {showEdges && (
        <lineSegments geometry={geos.edges}>
          <lineBasicMaterial color="#222222" />
        </lineSegments>
      )}
    </group>
  );
};

export default MeshView;
