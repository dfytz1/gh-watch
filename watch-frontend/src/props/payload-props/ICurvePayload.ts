// buffer: flat [x0,y0,z0, x1,y1,z1, ...] polyline sample points
import type { IType } from "./IType";

export interface ICurvePayload extends IType {
  type: "curve";
  buffer: number[];
}
