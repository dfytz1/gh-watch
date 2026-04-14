import type { Object3D } from "three";
import { Rhino3dmLoader } from "three/examples/jsm/Addons.js";

// Create a fresh loader (= fresh Web Worker) for every parse so that
// two back-to-back large files never share the same worker memory.
export const loadRhinoFileFromByteArray = (
  fileData: Uint8Array,
): Promise<Object3D | undefined> => {
  const loader = new Rhino3dmLoader();
  loader.setLibraryPath("/");
  return new Promise((resolve, reject) => {
    loader.parse(
      fileData.buffer,
      (object: Object3D) => {
        loader.dispose();
        resolve(object);
      },
      (onerror) => {
        loader.dispose();
        reject(onerror);
      },
    );
  });
};
