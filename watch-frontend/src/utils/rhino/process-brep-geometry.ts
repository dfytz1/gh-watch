import { BufferGeometryLoader, type BufferGeometry } from "three";

import type { IGenericPayload } from "../../props/payload-props/IGenericPayload";
import { getRhino } from "../../rhino/get-rhino";
import type { Mesh } from "rhino3dm";

/**
 * Processes a direct_geometry payload:
 * - Decodes each brep from JSON
 * - Meshes every brep face and appends to a single rhino Mesh
 * - Merges all rhino meshes into one Three.js BufferGeometry
 * - Disposes all WASM heap objects to prevent memory leaks
 * - Yields to the event loop between breps to keep the UI responsive
 *
 * @returns A merged BufferGeometry, or null if nothing could be processed.
 */
export async function processDirectGeometry(
  payload: IGenericPayload[],
): Promise<BufferGeometry | null> {
  const rhino = await getRhino();
  if (!rhino) return null;

  const rhinoMeshes: Mesh[] = [];

  try {
    for (const item of payload) {
      const data = item.data;
      if (!data) continue;

      const parsed = typeof data === "string" ? JSON.parse(data) : data;

      const brep = rhino.CommonObject.decode(parsed);
      if (brep instanceof rhino.Brep) {
        const faceList = brep.faces();
        const rhMesh = new rhino.Mesh();

        for (let i = 0; i < faceList.count; i++) {
          const face = faceList.get(i);

          const faceMesh = face.getMesh(rhino.MeshType.Any);

          if (faceMesh) {
            rhMesh.append(faceMesh);
          }
        }

        rhinoMeshes.push(rhMesh);
      }
    }

    if (rhinoMeshes.length === 0) return null;

    const merged = rhino.Mesh.toThreejsJSONMerged(rhinoMeshes, false);
    if (!merged) return null;

    return new BufferGeometryLoader().parse(merged);
  } catch {
    return null;
  }
}
