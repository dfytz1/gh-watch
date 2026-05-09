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

  useEffect(() => {
    const unregister = registerWebViewMessageHandlers({
      // Grasshopper sends this before it starts serializing, so the overlay
      // appears immediately — before any geometry data arrives.
      geometries_loading: () => {
        setLoading(true);
      },
      /**for curve use file based approach */
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
        // loading is cleared by RhinoFileView once the file finishes loading
      },
      mesh: async (payload: IGenericPayload[]) => {
        if (payload.length > 0) {
          const geo = await processDirectGeometry(payload);
          if (geo) setBrepGeometry(geo);
        } else {
          setBrepGeometry(null);
        }
        // loading is not cleared here — file_geometry always follows and
        // RhinoFileView will clear it once the heavier file load completes.
      },
    });

    return () => {
      unregister?.();
    };
  }, [setLoading]);

  return (
    <>
      {fileArray && <RhinoFileView byteArray={fileArray} />}
      {brepGeometry && <BrepView geometry={brepGeometry} />}
    </>
  );
};

export default GeometryView;
