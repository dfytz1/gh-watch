import type { IType } from "./type";

export interface IPointPayload extends IType {
  x: number;
  y: number;
  z: number;
}
