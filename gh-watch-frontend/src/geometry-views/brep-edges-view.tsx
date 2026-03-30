import { useEffect, useMemo } from "react";
import { BufferGeometry, Float32BufferAttribute } from "three";
import { useViewSettings } from "../store/view-settings";
import type { IBrepEdgesPayload } from "../props/payload-props/brep-edges-props";

interface Props {
  brepEdges: IBrepEdgesPayload[];
}

const BrepEdgesView = ({ brepEdges }: Props) => {
  const showEdges = useViewSettings((s) => s.showEdges);

  const geometry = useMemo<BufferGeometry | null>(() => {
    if (brepEdges.length === 0) return null;

    // Pre-allocate exact size to avoid intermediate JS array
    const totalLength = brepEdges.reduce((sum, e) => sum + e.buffer.length, 0);
    if (totalLength === 0) return null;

    const merged = new Float32Array(totalLength);
    let offset = 0;
    for (const e of brepEdges) {
      merged.set(e.buffer, offset);
      offset += e.buffer.length;
    }

    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(merged, 3));
    return geo;
  }, [brepEdges]);

  useEffect(() => {
    return () => {
      geometry?.dispose();
    };
  }, [geometry]);

  if (!showEdges || !geometry) return null;

  return (
    <lineSegments geometry={geometry}>
      <lineBasicMaterial color="#222222" />
    </lineSegments>
  );
};

export default BrepEdgesView;
