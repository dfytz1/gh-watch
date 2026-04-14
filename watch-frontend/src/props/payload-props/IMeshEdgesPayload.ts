// Same buffer format as IBrepEdgesPayload — flat segment pairs from mesh topology.
import type { IType } from "./IType";

export interface IMeshEdgesPayload extends IType {
  type: "meshEdges";
  buffer: number[];
}
