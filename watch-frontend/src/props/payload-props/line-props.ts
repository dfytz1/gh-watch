import type { IType } from "./type";

export interface ILinePayload extends IType {
  start: [number, number, number];
  end: [number, number, number];
}
