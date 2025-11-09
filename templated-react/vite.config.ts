import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  base: "./",
  build: {
    outDir: "../TemplateAPI/wwwroot",
    emptyOutDir: true,
  },
  server: {
    proxy: {
      "/api": "https://localhost:7231",
    },
  },
});
