import type { RhinoModule } from "rhino3dm";
import rhino3dm from "rhino3dm";

let rhino: RhinoModule | null = null;

export const getRhino = async (): Promise<RhinoModule> => {
  if (rhino) return rhino;
  // locateFile overrides the default WASM path (which would be relative to
  // the bundled JS in /assets/) and points it at the copy in public/ instead.
  // The type declaration omits this Emscripten option so we cast to allow it.
  type RhinoFactory = (opts: { locateFile: (f: string) => string }) => Promise<RhinoModule>;
  rhino = await (rhino3dm as unknown as RhinoFactory)({ locateFile: (f) => `/${f}` });
  return rhino;
};
