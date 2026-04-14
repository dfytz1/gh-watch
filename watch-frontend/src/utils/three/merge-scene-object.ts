import {
  BufferGeometry,
  Float32BufferAttribute,
  Group,
  Line,
  LineBasicMaterial,
  LineSegments,
  Material,
  Mesh,
  MeshStandardMaterial,
  Points,
  PointsMaterial,
  type Object3D,
} from "three";
import { mergeGeometries } from "three/examples/jsm/utils/BufferGeometryUtils.js";

// Converts a Line strip geometry (v0,v1,v2,...) into explicit LineSegments
// pairs (v0,v1, v1,v2, v2,v3, ...) so it can be merged with other segment geos.
function stripToSegments(geo: BufferGeometry): BufferGeometry {
  const src = geo.getAttribute("position");
  const count = src.count;
  if (count < 2) return new BufferGeometry();

  const arr = new Float32Array((count - 1) * 6);
  for (let i = 0; i < count - 1; i++) {
    const o = i * 6;
    arr[o] = src.getX(i);
    arr[o + 1] = src.getY(i);
    arr[o + 2] = src.getZ(i);
    arr[o + 3] = src.getX(i + 1);
    arr[o + 4] = src.getY(i + 1);
    arr[o + 5] = src.getZ(i + 1);
  }

  const result = new BufferGeometry();
  result.setAttribute("position", new Float32BufferAttribute(arr, 3));
  return result;
}

// Recursively dispose all geometries and materials in an Object3D hierarchy.
export function disposeObject3D(obj: Object3D) {
  obj.traverse((child) => {
    if ((child as Mesh).geometry) (child as Mesh).geometry.dispose();
    const mat = (child as Mesh).material;
    if (mat) {
      if (Array.isArray(mat)) mat.forEach((m: Material) => m.dispose());
      else (mat as Material).dispose();
    }
  });
}

// Merges all Mesh / Line / LineSegments / Points children of root into a
// maximum of 3 draw calls (one per geometry type). All geometries are cloned
// and baked into world space so the returned Group sits at the origin.
export function buildSceneGroup(root: Object3D): Group {
  root.updateWorldMatrix(true, true);

  const meshGeos: BufferGeometry[] = [];
  const lineGeos: BufferGeometry[] = [];
  const pointsGeos: BufferGeometry[] = [];

  // ── 1. Traverse & collect ────────────────────────────────────────────────
  const t0 = performance.now();

  root.traverse((child) => {
    if (child instanceof Mesh && child.geometry) {
      const geo = (child.geometry as BufferGeometry).clone();
      geo.applyMatrix4(child.matrixWorld);
      meshGeos.push(geo);
    } else if (child instanceof LineSegments && child.geometry) {
      const geo = (child.geometry as BufferGeometry).clone();
      geo.applyMatrix4(child.matrixWorld);
      lineGeos.push(geo);
    } else if (child instanceof Line && child.geometry) {
      const cloned = (child.geometry as BufferGeometry).clone();
      cloned.applyMatrix4(child.matrixWorld);
      const seg = stripToSegments(cloned);
      cloned.dispose();
      lineGeos.push(seg);
    } else if (child instanceof Points && child.geometry) {
      const geo = (child.geometry as BufferGeometry).clone();
      geo.applyMatrix4(child.matrixWorld);
      pointsGeos.push(geo);
    }
  });

  const t1 = performance.now();

  // ── 2. Merge meshes ───────────────────────────────────────────────────────
  const group = new Group();

  if (meshGeos.length > 0) {
    const merged = mergeGeometries(meshGeos, false);
    meshGeos.forEach((g) => g.dispose());
    if (merged) {
      group.add(
        new Mesh(
          merged,
          new MeshStandardMaterial({ color: "#888888", transparent: true, opacity: 0.5, side: 2 }),
        ),
      );
    }
  }

  const t2 = performance.now();

  // ── 3. Merge lines ────────────────────────────────────────────────────────
  if (lineGeos.length > 0) {
    const merged = mergeGeometries(lineGeos, false);
    lineGeos.forEach((g) => g.dispose());
    if (merged)
      group.add(new LineSegments(merged, new LineBasicMaterial({ color: "#444444" })));
  }

  const t3 = performance.now();

  // ── 4. Merge points ───────────────────────────────────────────────────────
  if (pointsGeos.length > 0) {
    const merged = mergeGeometries(pointsGeos, false);
    pointsGeos.forEach((g) => g.dispose());
    if (merged)
      group.add(new Points(merged, new PointsMaterial({ color: "#ff4444", size: 2, sizeAttenuation: false })));
  }

  const t4 = performance.now();

  console.group("gh-watch | buildSceneGroup");
  console.log(
    "traverse + clone : %.2f ms  (meshes=%d  lines=%d  points=%d)",
    t1 - t0,
    meshGeos.length,
    lineGeos.length,
    pointsGeos.length,
  );
  console.log("merge meshes     : %.2f ms", t2 - t1);
  console.log("merge lines      : %.2f ms", t3 - t2);
  console.log("merge points     : %.2f ms", t4 - t3);
  console.log("total            : %.2f ms", t4 - t0);
  console.groupEnd();

  return group;
}
