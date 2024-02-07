import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import fs from 'fs'


// https://vitejs.dev/config/
export default defineConfig({
  server: {
    host: process.env.HOST_FRONT,
    port:8080,
    https: {
      key: fs.readFileSync('./certs/dev.local+4-key.pem'),
      cert: fs.readFileSync('./certs/dev.local+4.pem'),
    }
  },
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  define: {
    'process.env': process.env
  },
})
