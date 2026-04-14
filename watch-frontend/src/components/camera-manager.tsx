import { useEffect, useRef } from "react";
import { useThree } from "@react-three/fiber";
import { PerspectiveCamera, OrthographicCamera, Vector3 } from "three";
import { useViewSettings, type CameraMode } from "../store/view-settings";

const CameraManager = () => {
  const get = useThree((s) => s.get);
  const set = useThree((s) => s.set);
  const cameraMode = useViewSettings((s) => s.cameraMode);
  const prevMode = useRef<CameraMode>(cameraMode);

  useEffect(() => {
    if (cameraMode === prevMode.current) return;
    prevMode.current = cameraMode;

    const { camera, controls, size } = get();
    const pos = camera.position.clone();
    const target: Vector3 = controls
      ? // eslint-disable-next-line @typescript-eslint/no-explicit-any
        (controls as any).target.clone()
      : new Vector3(0, 0, 0);

    const aspect = size.width / size.height;

    let next: PerspectiveCamera | OrthographicCamera;

    if (cameraMode === "perspective") {
      next = new PerspectiveCamera(50, aspect, 0.01, 10000);
    } else {
      next = new OrthographicCamera(-100 * aspect, 100 * aspect, 100, -100, -1000, 1000);
    }

    next.position.copy(pos);
    next.lookAt(target);
    next.updateProjectionMatrix();

    set({ camera: next });

    if (controls) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const orbit = controls as any;
      orbit.object = next;
      orbit.target.copy(target);
      orbit.update();
    }
  }, [cameraMode, get, set]);

  return null;
};

export default CameraManager;
