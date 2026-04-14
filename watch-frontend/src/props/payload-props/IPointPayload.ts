import type { IType } from "./IType";

export interface IPointPayload extends IType {
  type: "point";
  x: number;
  y: number;
  z: number;
}
