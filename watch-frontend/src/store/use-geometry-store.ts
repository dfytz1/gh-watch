import { create } from "zustand";
import type { IMeshPayload } from "../props/payload-props/IMeshPayload";
import type { IPointPayload } from "../props/payload-props/IPointPayload";
import type { ICurvePayload } from "../props/payload-props/ICurvePayload";
import type { ILinePayload } from "../props/payload-props/ILinePayload";
import type { IBrepEdgesPayload } from "../props/payload-props/IBrepEdgesPayload";
import type { IMeshEdgesPayload } from "../props/payload-props/IMeshEdgesPayload";
import type { IGenericPayload } from "../props/payload-props/IGenericPayload";

interface GeometryStoreState {
  breps: IGenericPayload[];
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
    breps: IGenericPayload[];
  }) => void;
}

export const useGeometryStore = create<GeometryStoreState>()((set) => ({
  breps: [],
  meshes: [],
  points: [],
  curves: [],
  lines: [],
  brepEdges: [],
  meshEdges: [],
  setGeometry: (state) => set(state),
}));
