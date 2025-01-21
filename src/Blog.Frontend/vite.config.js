import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react-swc'

console.log(process.env.VITE_API_URL)

// https://vite.dev/config/
export default defineConfig({
    server: {
        host: "0.0.0.0",
        watch: {
            usePolling: true
        },
        strictPort: true,
        proxy: {
            "/api": {
                target: process.env.VITE_API_URL,
                changeOrigin: true,
                secure: false,
            }
        }
    },
    plugins: [react()]
})
