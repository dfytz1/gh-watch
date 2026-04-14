import { useMemo } from "react";
import { BufferGeometry, Float32BufferAttribute, LineSegments, LineBasicMaterial } from "three";
import type { ILinePayload } from "../props/payload-props/ILinePayload";

interface LineViewProps {
  payload: ILinePayload;
}

const LineView: React.FC<LineViewProps> = ({ payload }) => {
  const object = useMemo(() => {
    const positions = new Float32Array([
      payload.start[0], payload.start[1], payload.start[2],
      payload.end[0],   payload.end[1],   payload.end[2],
    ]);
    const geo = new BufferGeometry();
    geo.setAttribute("position", new Float32BufferAttribute(positions, 3));
    return new LineSegments(geo, new LineBasicMaterial({ color: "#444444" }));
  }, [payload]);

  return <primitive object={object} />;
};

export default LineView;
