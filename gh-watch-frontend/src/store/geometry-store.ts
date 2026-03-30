import { create } from "zustand";
import type { IMeshPayload } from "../props/payload-props/mesh-props";
import type { IPointPayload } from "../props/payload-props/point-props";
import type { ICurvePayload } from "../props/payload-props/curve-props";
import type { ILinePayload } from "../props/payload-props/line-props";

interface GeometryStoreState {
  meshes: IMeshPayload[];
  points: IPointPayload[];
  curves: ICurvePayload[];
  lines: ILinePayload[];
  setGeometry: (state: {
    meshes: IMeshPayload[];
    points: IPointPayload[];
    curves: ICurvePayload[];
    lines: ILinePayload[];
  }) => void;
}

export const useGeometryStore = create<GeometryStoreState>()((set) => ({
  meshes: [],
  points: [],
  curves: [],
  lines: [],
  setGeometry: (state) => set(state),
}));
