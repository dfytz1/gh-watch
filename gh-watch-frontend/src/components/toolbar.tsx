import { Frame } from "lucide-react";
import { useViewSettings } from "../store/view-settings";

const Toolbar = () => {
  const { showEdges, toggleEdges } = useViewSettings();

  return (
    <div className="absolute top-3 right-3 flex gap-1 rounded-lg bg-black/50 p-1 backdrop-blur-sm">
      <button
        onClick={toggleEdges}
        title={showEdges ? "Hide edges" : "Show edges"}
        className={`rounded-md p-2 transition-colors ${
          showEdges
            ? "bg-white/20 text-white"
            : "text-white/30 hover:text-white/60"
        }`}
      >
        <Frame size={16} />
      </button>
    </div>
  );
};

export default Toolbar;
