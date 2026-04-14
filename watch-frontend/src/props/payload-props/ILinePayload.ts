import type { IType } from "./IType";

export interface ILinePayload extends IType {
  type: "line";
  start: [number, number, number];
  end: [number, number, number];
}
