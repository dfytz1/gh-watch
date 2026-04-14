import { create } from "zustand";
import type { Object3D } from "three";

interface SceneStoreState {
  sceneObject: Object3D | null;
  setSceneObject: (obj: Object3D | null) => void;
}

export const useSceneStore = create<SceneStoreState>()((set) => ({
  sceneObject: null,
  setSceneObject: (obj) => set({ sceneObject: obj }),
}));
