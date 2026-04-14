import { useEffect, useRef } from "react";
import { BufferGeometry, Float32BufferAttribute, LineBasicMaterial, LineSegments } from "three";
import type { IMeshEdgesPayload } from "../props/payload-props/IMeshEdgesPayload";

interface MeshEdgesViewProps {
  payload: IMeshEdgesPayload;
}

// Renders mesh topology edges as LineSegments.
// buffer format: [x0s,y0s,z0s, x0e,y0e,z0e,  x1s,y1s,z1s, x1e,y1e,z1e, ...]
const MeshEdgesView: React.FC<MeshEdgesViewProps> = ({ payload }) => {
  const linesRef = useRef<LineSegments>(null);

  useEffect(() => {
    const lines = linesRef.current;
    if (!lines) return;

    const prev = lines.geometry;

    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(new Float32Array(payload.buffer), 3));
    lines.geometry = geo;

    prev.dispose();
  }, [payload]);

  return (
    <primitive
      object={new LineSegments(new BufferGeometry(), new LineBasicMaterial({ color: "#333333" }))}
      ref={linesRef}
    />
  );
};

export default MeshEdgesView;
