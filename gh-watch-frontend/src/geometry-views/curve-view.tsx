import { useEffect, useMemo } from "react";
import { BufferGeometry, Float32BufferAttribute } from "three";
import type { ICurvePayload } from "../props/payload-props/curve-props";

interface Props {
  curves: ICurvePayload[];
}

const CurveView = ({ curves }: Props) => {
  const geometry = useMemo<BufferGeometry | null>(() => {
    if (curves.length === 0) return null;
    const positions: number[] = [];
    for (const curve of curves) {
      const buf = curve.buffer;
      for (let i = 0; i < buf.length - 3; i += 3) {
        positions.push(buf[i], buf[i + 1], buf[i + 2], buf[i + 3], buf[i + 4], buf[i + 5]);
      }
    }
    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(new Float32Array(positions), 3));
    return geo;
  }, [curves]);

  useEffect(() => {
    return () => { geometry?.dispose(); };
  }, [geometry]);

  if (!geometry) return null;

  return (
    <lineSegments geometry={geometry}>
      <lineBasicMaterial color="#44aaff" />
    </lineSegments>
  );
};

export default CurveView;
