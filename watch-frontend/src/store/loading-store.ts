import { create } from "zustand";

interface LoadingStoreState {
  isLoading: boolean;
  pendingViews: Set<string>;
  setLoading: (loading: boolean) => void;
  // Mark a named view as still loading — isLoading stays true until all views are done.
  markViewLoading: (viewId: string) => void;
  // Mark a named view as done — clears isLoading once no views remain pending.
  markViewDone: (viewId: string) => void;
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
