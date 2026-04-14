import { ZoomIn, Box } from "lucide-react";
import { useViewSettings } from "../store/view-settings";

const Toolbar = () => {
  const { requestZoomToFit, cameraMode, toggleCameraMode } = useViewSettings();

  return (
    <div className="absolute top-3 right-3 flex gap-1 rounded-lg bg-black/50 p-1 backdrop-blur-sm">
      <button
        onClick={requestZoomToFit}
        title="Zoom to fit"
        className="rounded-md p-2 cursor-pointer transition-colors text-white/30 hover:text-white/60"
      >
        <ZoomIn size={16} />
      </button>
      <button
        onClick={toggleCameraMode}
        title={cameraMode === "perspective" ? "Switch to orthographic" : "Switch to perspective"}
        className={`rounded-md p-2 cursor-pointer transition-colors ${
          cameraMode === "perspective"
            ? "bg-white/20 text-white"
            : "text-white/30 hover:text-white/60"
        }`}
      >
        <Box size={16} />
      </button>
    </div>
  );
};

export default Toolbar;
