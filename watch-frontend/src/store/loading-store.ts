import { create } from "zustand";

interface LoadingStoreState {
  isLoading: boolean;
  setLoading: (loading: boolean) => void;
}

export const useLoadingStore = create<LoadingStoreState>()((set) => ({
  isLoading: false,
  setLoading: (isLoading) => set({ isLoading }),
}));
