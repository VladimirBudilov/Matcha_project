import {fileURLToPath, URL} from 'node:url'
import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import Components from 'unplugin-vue-components/vite';
import {AntDesignVueResolver} from 'unplugin-vue-components/resolvers';
import dotenv from 'dotenv'
import path from 'path'

dotenv.config({ path: path.resolve(__dirname, '../.env') });

export default defineConfig({
    plugins: [
        vue({
            template: {
                compilerOptions: {
                    isCustomElement: tagName => {
                        return tagName === 'vue-advanced-chat' || tagName === 'emoji-picker'
                    }
                }
            }
        }),
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
    }, server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5000',
                changeOrigin: true,
                ws: true
            }
        }
    }
})