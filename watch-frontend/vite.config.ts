import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import path from "path";

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      // rhino3dm's package.json has no "browser" field so Vite picks the
      // Node.js build (rhino3dm.js) which imports fs/path/crypto/ws.
      // Redirect to the browser ESM build instead.
      rhino3dm: path.resolve(
        "./node_modules/rhino3dm/rhino3dm.module.js"
      ),
    },
  },
});
