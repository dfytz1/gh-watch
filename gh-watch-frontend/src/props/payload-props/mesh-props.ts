import type { IType } from "./type";

export interface IMeshPayload extends IType {
  vertices: number[];
  normals: number[];
  faces: number[];
}
