import { useEffect } from "react";
import { registerWebViewMessageHandlers } from "../webview-communication/wv";
import type { IMeshPayload } from "../props/payload-props/mesh-props";
import type { IPointPayload } from "../props/payload-props/point-props";
import type { ICurvePayload } from "../props/payload-props/curve-props";
import type { ILinePayload } from "../props/payload-props/line-props";
import type { IType } from "../props/payload-props/type";
import MeshView from "./mesh-view";
import PointView from "./point-view";
import CurveView from "./curve-view";
import LineView from "./line-view";
import { useGeometryStore } from "../store/geometry-store";

const GeometryView = () => {
  const { meshes, points, curves, lines, setGeometry } = useGeometryStore();

  useEffect(() => {
    const unregister = registerWebViewMessageHandlers({
      geometry: (payload: unknown) => {
        const items = payload as IType[];
        const next = { meshes: [] as IMeshPayload[], points: [] as IPointPayload[], curves: [] as ICurvePayload[], lines: [] as ILinePayload[] };

        for (const item of items) {
          switch (item.type) {
            case "mesh":   next.meshes.push(item as IMeshPayload);  break;
            case "point":  next.points.push(item as IPointPayload); break;
            case "curve":  next.curves.push(item as ICurvePayload); break;
            case "line":   next.lines.push(item as ILinePayload);   break;
          }
        }

        setGeometry(next);
      },
    });

    return () => unregister();
  }, [setGeometry]);

  return (
    <>
      <MeshView  meshes={meshes}  />
      <PointView points={points} />
      <CurveView curves={curves} />
      <LineView  lines={lines}   />
    </>
  );
};

export default GeometryView;
