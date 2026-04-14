import type { IType } from "./IType";

export interface IMeshPayload extends IType {
  type: "mesh";
  vertices: number[];
  normals: number[];
  faces: number[];
}
