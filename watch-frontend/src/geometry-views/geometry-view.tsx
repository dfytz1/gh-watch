import { useEffect, useRef, useState } from "react";
import { registerWebViewMessageHandlers } from "../webview-communication/wv";
import RhinoFileView from "./rhino-file-view";
import type { BufferGeometry } from "three";
import BrepView from "./brep-view";

import { processDirectGeometry } from "../utils/rhino/process-brep-geometry";
import type { IGenericPayload } from "../props/payload-props/IGenericPayload";
import { useLoadingStore } from "../store/loading-store";

const DEBOUNCE_MS = 150;

const GeometryView = () => {
  const [fileArray, setFileArray] = useState<Uint8Array | null>(null);
  const [brepGeometry, setBrepGeometry] = useState<BufferGeometry | null>(null);
  const setLoading = useLoadingStore((s) => s.setLoading);

  const fileDebounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const meshDebounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    const unregister = registerWebViewMessageHandlers({
      /**for curve use file based approach */
      file_geometry: (payload: string) => {
        if (!payload) return;

        setLoading(true);

        if (fileDebounceRef.current) clearTimeout(fileDebounceRef.current);
        fileDebounceRef.current = setTimeout(() => {
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
        }, DEBOUNCE_MS);
      },
      mesh: async (payload: IGenericPayload[]) => {
        setLoading(true);

        if (meshDebounceRef.current) clearTimeout(meshDebounceRef.current);
        meshDebounceRef.current = setTimeout(async () => {
          try {
            if (payload.length > 0) {
              const geo = await processDirectGeometry(payload);
              if (geo) setBrepGeometry(geo);
            } else {
              setBrepGeometry(null);
            }
          } finally {
            setLoading(false);
          }
        }, DEBOUNCE_MS);
      },
    });

    return () => {
      unregister?.();
      if (fileDebounceRef.current) clearTimeout(fileDebounceRef.current);
      if (meshDebounceRef.current) clearTimeout(meshDebounceRef.current);
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
