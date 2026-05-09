import { Loader2 } from "lucide-react";
import { useLoadingStore } from "../store/loading-store";

const LoadingOverlay = () => {
  const isLoading = useLoadingStore((s) => s.isLoading);

  if (!isLoading) return null;

  return (
    <div
      style={{
        position: "absolute",
        inset: 0,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: "rgba(0, 0, 0, 0.35)",
        backdropFilter: "blur(2px)",
        zIndex: 10,
        pointerEvents: "all",
        cursor: "wait",
      }}
    >
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: "12px",
          color: "white",
        }}
      >
        <Loader2 size={36} className="animate-spin" />
        <span style={{ fontSize: "14px", opacity: 0.9 }}>
          Loading geometry…
        </span>
      </div>
    </div>
  );
};

export default LoadingOverlay;
