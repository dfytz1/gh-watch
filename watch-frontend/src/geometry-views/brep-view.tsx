import React, { useEffect } from "react";
import { BufferGeometry, MeshStandardMaterial } from "three";

export interface IBrepViewProps {
  geometry: BufferGeometry;
}

const material = new MeshStandardMaterial({
  color: "#888888",
  transparent: true,
  opacity: 0.5,
  side: 2,
});

const BrepView: React.FC<IBrepViewProps> = ({ geometry }) => {
  useEffect(() => {
    return () => {
      geometry.dispose();
    };
  }, [geometry]);

  return <mesh geometry={geometry} material={material} />;
};

export default BrepView;
