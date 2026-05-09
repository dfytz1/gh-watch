import { useEffect, useState } from "react";
import { registerWebViewMessageHandlers } from "../webview-communication/wv";
import RhinoFileView from "./rhino-file-view";
import type { BufferGeometry } from "three";
import BrepView from "./brep-view";

import { processDirectGeometry } from "../utils/rhino/process-brep-geometry";
import type { IGenericPayload } from "../props/payload-props/IGenericPayload";
import { useLoadingStore } from "../store/loading-store";

const GeometryView = () => {
  const [fileArray, setFileArray] = useState<Uint8Array | null>(null);
  const [brepGeometry, setBrepGeometry] = useState<BufferGeometry | null>(null);
  const setLoading = useLoadingStore((s) => s.setLoading);
  const markViewLoading = useLoadingStore((s) => s.markViewLoading);
  const markViewDone = useLoadingStore((s) => s.markViewDone);

  useEffect(() => {
    const unregister = registerWebViewMessageHandlers({
      // Grasshopper sends this before it starts serializing — mark both views as
      // pending so the overlay stays visible until every view finishes processing.
      geometries_loading: () => {
        setLoading(true);
        markViewLoading("brep");
        markViewLoading("file");
      },

      // File-based geometry (curves, meshes, etc.) — decoding triggers RhinoFileView
      // which clears the "file" pending flag in its finally block.
      file_geometry: (payload: string) => {
        if (!payload) return;

        console.group("gh-watch | geometry message");
        console.log("base64 payload (%d chars):", payload.length);

        const t0 = performance.now();
        const binary = atob(payload);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++)
          bytes[i] = binary.charCodeAt(i);
        const t1 = performance.now();

        console.log("decode : %.2f ms  (%d bytes)", t1 - t0, bytes.length);
        console.groupEnd();

        setFileArray(bytes);
      },

      // Brep / surface / box geometry — processed directly in JS; marks "brep" done
      // once the async work completes, regardless of what the file view is doing.
      mesh: async (payload: IGenericPayload[]) => {
        try {
          if (payload.length > 0) {
            const geo = await processDirectGeometry(payload);
            setBrepGeometry(geo);
          } else {
            setBrepGeometry(null);
          }
        } finally {
          markViewDone("brep");
        }
      },
    });

    return () => {
      unregister?.();
    };
  }, [setLoading, markViewLoading, markViewDone]);

  return (
    <>
      {fileArray && <RhinoFileView byteArray={fileArray} />}
      {brepGeometry && <BrepView geometry={brepGeometry} />}
    </>
  );
};

export default GeometryView;
