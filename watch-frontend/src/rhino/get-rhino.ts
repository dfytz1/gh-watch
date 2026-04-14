import type { RhinoModule } from "rhino3dm";
import rhino3dm from "rhino3dm";

let rhino: RhinoModule | null = null;

export const getRhino = async (): Promise<RhinoModule> => {
  if (rhino) return rhino;
  rhino = await rhino3dm();
  return rhino;
};
