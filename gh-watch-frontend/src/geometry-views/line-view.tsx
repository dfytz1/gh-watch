import { useEffect, useMemo } from "react";
import { BufferGeometry, Float32BufferAttribute } from "three";
import type { ILinePayload } from "../props/payload-props/line-props";

interface Props {
  lines: ILinePayload[];
}

const LineView = ({ lines }: Props) => {
  const geometry = useMemo<BufferGeometry | null>(() => {
    if (lines.length === 0) return null;
    const positions = new Float32Array(lines.length * 6);
    for (let i = 0; i < lines.length; i++) {
      const o = i * 6;
      positions[o]     = lines[i].start[0];
      positions[o + 1] = lines[i].start[1];
      positions[o + 2] = lines[i].start[2];
      positions[o + 3] = lines[i].end[0];
      positions[o + 4] = lines[i].end[1];
      positions[o + 5] = lines[i].end[2];
    }
    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(positions, 3));
    return geo;
  }, [lines]);

  useEffect(() => {
    return () => { geometry?.dispose(); };
  }, [geometry]);

  if (!geometry) return null;

  return (
    <lineSegments geometry={geometry}>
      <lineBasicMaterial color="#ffaa00" />
    </lineSegments>
  );
};

export default LineView;
