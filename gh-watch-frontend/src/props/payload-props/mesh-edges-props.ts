import type { IType } from "./type";

export interface IMeshEdgesPayload extends IType {
  buffer: number[]; // flat segment-pair array: x0,y0,z0, x1,y1,z1, ... (each 6 floats = 1 segment)
}
