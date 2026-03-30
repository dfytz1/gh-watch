import { create } from "zustand";
import type { IMeshPayload } from "../props/payload-props/mesh-props";
import type { IPointPayload } from "../props/payload-props/point-props";
import type { ICurvePayload } from "../props/payload-props/curve-props";
import type { ILinePayload } from "../props/payload-props/line-props";
import type { IBrepEdgesPayload } from "../props/payload-props/brep-edges-props";
import type { IMeshEdgesPayload } from "../props/payload-props/mesh-edges-props";

interface GeometryStoreState {
  meshes: IMeshPayload[];
  points: IPointPayload[];
  curves: ICurvePayload[];
  lines: ILinePayload[];
  brepEdges: IBrepEdgesPayload[];
  meshEdges: IMeshEdgesPayload[];
  setGeometry: (state: {
    meshes: IMeshPayload[];
    points: IPointPayload[];
    curves: ICurvePayload[];
    lines: ILinePayload[];
    brepEdges: IBrepEdgesPayload[];
    meshEdges: IMeshEdgesPayload[];
  }) => void;
}

export const useGeometryStore = create<GeometryStoreState>()((set) => ({
  meshes: [],
  points: [],
  curves: [],
  lines: [],
  brepEdges: [],
  meshEdges: [],
  setGeometry: (state) => set(state),
}));
