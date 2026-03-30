import { create } from "zustand";
import { persist } from "zustand/middleware";

interface ViewSettingsState {
  showEdges: boolean;
  toggleEdges: () => void;
}

export const useViewSettings = create<ViewSettingsState>()(
  persist(
    (set) => ({
      showEdges: true,
      toggleEdges: () => set((state) => ({ showEdges: !state.showEdges })),
    }),
    { name: "gh-watch-view-settings" },
  ),
);
