import { create } from "zustand";
import type { ViewType } from "../geometry-views/view-types";

interface LoadingStoreState {
  isLoading: boolean;
  pendingViews: Set<string>;
  setLoading: (loading: boolean) => void;
  // Mark a named view as still loading — isLoading stays true until all views are done.
  markViewLoading: (viewType: ViewType) => void;
  // Mark a named view as done — clears isLoading once no views remain pending.
  markViewDone: (viewType: ViewType) => void;
}

export const useLoadingStore = create<LoadingStoreState>()((set) => ({
  isLoading: false,
  pendingViews: new Set<string>(),
  setLoading: (isLoading) => set({ isLoading }),
  markViewLoading: (viewId) =>
    set((state) => {
      const next = new Set(state.pendingViews);
      next.add(viewId);
      return { pendingViews: next, isLoading: true };
    }),
  markViewDone: (viewId) =>
    set((state) => {
      const next = new Set(state.pendingViews);
      next.delete(viewId);
      return { pendingViews: next, isLoading: next.size > 0 };
    }),
}));
