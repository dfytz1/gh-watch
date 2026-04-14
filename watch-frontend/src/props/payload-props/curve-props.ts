import type { IType } from "./type";

export interface ICurvePayload extends IType {
  buffer: number[]; // flat array of points (x1, y1, z1, x2, y2, z2, ...)
}