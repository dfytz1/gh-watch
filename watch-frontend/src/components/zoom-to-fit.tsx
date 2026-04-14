import { useEffect, useRef } from "react";
import { useThree } from "@react-three/fiber";
import { Box3, Vector3, PerspectiveCamera, OrthographicCamera } from "three";
import { useViewSettings } from "../store/view-settings";
import { useGeometryStore } from "../store/geometry-store";

function computeBoundingBox(): Box3 {
  const { meshes, points, curves, lines } = useGeometryStore.getState();
  const box = new Box3();

  for (const mesh of meshes)
    for (let i = 0; i < mesh.vertices.length; i += 3)
      box.expandByPoint(
        new Vector3(
          mesh.vertices[i],
          mesh.vertices[i + 1],
          mesh.vertices[i + 2],
        ),
      );

  for (const pt of points) box.expandByPoint(new Vector3(pt.x, pt.y, pt.z));

  for (const curve of curves)
    for (let i = 0; i < curve.buffer.length; i += 3)
      box.expandByPoint(
        new Vector3(curve.buffer[i], curve.buffer[i + 1], curve.buffer[i + 2]),
      );

  for (const line of lines) {
    box.expandByPoint(new Vector3(line.start[0], line.start[1], line.start[2]));
    box.expandByPoint(new Vector3(line.end[0], line.end[1], line.end[2]));
  }

  return box;
}

const ZoomToFit = () => {
  const get = useThree((state) => state.get);
  const request = useViewSettings((s) => s.zoomToFitRequest);
  const prevRequest = useRef(0);

  useEffect(() => {
    if (request === 0 || request === prevRequest.current) return;
    prevRequest.current = request;

    const { camera, controls, size } = get();

    const box = computeBoundingBox();
    if (box.isEmpty()) return;

    const center = box.getCenter(new Vector3());
    const sizeVec = box.getSize(new Vector3());
    const maxDim = Math.max(sizeVec.x, sizeVec.y, sizeVec.z);
    const lookDir = new Vector3(1, -1, 0.75).normalize();

    if (camera instanceof PerspectiveCamera) {
      const fov = camera.fov * (Math.PI / 180);
      const distance = (maxDim / 2 / Math.tan(fov / 2)) * 2;
      camera.position.copy(center).addScaledVector(lookDir, distance);
      camera.lookAt(center);
      camera.updateProjectionMatrix();
    } else if (camera instanceof OrthographicCamera) {
      const aspect = size.width / size.height;
      const padding = 1.2;
      const halfHeight = (maxDim / 2) * padding;
      const halfWidth = halfHeight * aspect;

      camera.left = -halfWidth;
      camera.right = halfWidth;
      camera.top = halfHeight;
      camera.bottom = -halfHeight;

      const orthoDistance = 500;
      camera.position.copy(center).addScaledVector(lookDir, orthoDistance);
      camera.lookAt(center);
      camera.updateProjectionMatrix();
    }

    if (controls) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      (controls as any).target.copy(center);
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      (controls as any).update();
    }
  }, [request, get]);

  return null;
};

export default ZoomToFit;
