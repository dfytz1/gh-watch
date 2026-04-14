import { useEffect } from "react";
import { LineBasicMaterial } from "three";
import type { BufferGeometry } from "three";

interface CurveViewProps {
  geometry: BufferGeometry;
}

const lineMaterial = new LineBasicMaterial({ color: "#444444" });

const CurveView: React.FC<CurveViewProps> = ({ geometry }) => {
  useEffect(() => {
    return () => {
      geometry.dispose();
    };
  }, [geometry]);

  return <lineSegments geometry={geometry} material={lineMaterial} />;
};

export default CurveView;
