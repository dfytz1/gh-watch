// buffer: flat [x0s,y0s,z0s, x0e,y0e,z0e,  x1s,y1s,z1s, x1e,y1e,z1e, ...]
// Each 6-float group is one line segment (start + end).
import type { IType } from "./IType";

export interface IBrepEdgesPayload extends IType {
  type: "brepEdges";
  buffer: number[];
}
