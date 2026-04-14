import type { IPointPayload } from "../props/payload-props/IPointPayload";

interface PointViewProps {
  payload: IPointPayload;
}

const PointView: React.FC<PointViewProps> = ({ payload }) => {
  return (
    <mesh position={[payload.x, payload.y, payload.z]}>
      <sphereGeometry args={[0.05, 8, 8]} />
      <meshStandardMaterial color="#ff4444" />
    </mesh>
  );
};

export default PointView;
