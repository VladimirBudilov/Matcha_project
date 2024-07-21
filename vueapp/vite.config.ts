import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import Components from 'unplugin-vue-components/vite';
import { AntDesignVueResolver } from 'unplugin-vue-components/resolvers';
import fs from 'fs'


// https://vitejs.dev/config/
export default defineConfig({
  server: {
    host: process.env.FRONT,
    port:8080,
    https: {
      key: fs.readFileSync(process.env.CERT!.toString()),
      cert: fs.readFileSync(process.env.CERT_KEY!.toString()),
    } ,
    hmr: {
      host: process.env.FRONT,
      protocol: "wss"
    },
  },
  plugins: [
    vue({
        template: {
  compilerOptions: {
    isCustomElement: tagName => {
      return tagName === 'vue-advanced-chat' || tagName === 'emoji-picker'
    }}}}),
    Components({
      resolvers: [
        AntDesignVueResolver({
          importStyle: false, // css in js
        }),
      ],
    }),
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
