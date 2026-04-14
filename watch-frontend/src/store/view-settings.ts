import { create } from "zustand";
import { persist } from "zustand/middleware";

export type CameraMode = "perspective" | "orthographic";

interface ViewSettingsState {
  zoomToFitRequest: number;
  requestZoomToFit: () => void;
  cameraMode: CameraMode;
  toggleCameraMode: () => void;
}

export const useViewSettings = create<ViewSettingsState>()(
  persist(
    (set) => ({
      zoomToFitRequest: 0,
      requestZoomToFit: () =>
        set((state) => ({ zoomToFitRequest: state.zoomToFitRequest + 1 })),
      cameraMode: "perspective",
      toggleCameraMode: () =>
        set((state) => ({
          cameraMode: state.cameraMode === "perspective" ? "orthographic" : "perspective",
        })),
    }),
    {
      name: "gh-watch-view-settings",
      partialize: (state) => ({ cameraMode: state.cameraMode }),
    },
  ),
);
