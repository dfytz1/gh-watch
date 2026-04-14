import { useEffect, useMemo } from "react";
import { BufferGeometry, Float32BufferAttribute } from "three";
import type { IPointPayload } from "../props/payload-props/point-props";

interface Props {
  points: IPointPayload[];
}

const PointView = ({ points }: Props) => {
  const geometry = useMemo<BufferGeometry | null>(() => {
    if (points.length === 0) return null;
    const positions = new Float32Array(points.length * 3);
    for (let i = 0; i < points.length; i++) {
      positions[i * 3]     = points[i].x;
      positions[i * 3 + 1] = points[i].y;
      positions[i * 3 + 2] = points[i].z;
    }
    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(positions, 3));
    return geo;
  }, [points]);

  useEffect(() => {
    return () => { geometry?.dispose(); };
  }, [geometry]);

  if (!geometry) return null;

  return (
    <points geometry={geometry}>
      <pointsMaterial color="#ff4444" size={0.1} />
    </points>
  );
};

export default PointView;
