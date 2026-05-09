import { useEffect, useState } from "react";
import { registerWebViewMessageHandlers } from "../webview-communication/wv";
import RhinoFileView from "./rhino-file-view";
import type { BufferGeometry } from "three";
import BrepView from "./brep-view";

import { processDirectGeometry } from "../utils/rhino/process-brep-geometry";
import type { IGenericPayload } from "../props/payload-props/IGenericPayload";
import { useLoadingStore } from "../store/loading-store";

interface GeometryBatchPayload {
  brepPayload: IGenericPayload[];
  fileData: string | null;
}

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

      // Single command wrapping all geometry for one GH solve.
      // Loading is cleared here when there is no file data to parse,
      // or by RhinoFileView's finally block when file-based geometry is present.
      geometry_batch: async (payload: GeometryBatchPayload) => {
        const hasBrepData = payload.brepPayload?.length > 0;
        const hasFileData = payload.fileData != null;

        // Kick off brep processing in parallel with setting state
        const brepPromise = hasBrepData
          ? processDirectGeometry(payload.brepPayload)
          : Promise.resolve(null);

        // Set file bytes (triggers RhinoFileView); null clears any previous file scene
        if (hasFileData) {
          const binary = atob(payload.fileData!);
          const bytes = new Uint8Array(binary.length);
          for (let i = 0; i < binary.length; i++)
            bytes[i] = binary.charCodeAt(i);
          setFileArray(bytes);
        } else {
          setFileArray(null);
        }

        const geo = await brepPromise;
        setBrepGeometry(geo);

        // If there is no file data, all geometry is already processed — clear loading now.
        // If there is file data, RhinoFileView clears loading in its finally block once
        // the heavier async 3dm parse completes.
        if (!hasFileData) {
          setLoading(false);
        }
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
