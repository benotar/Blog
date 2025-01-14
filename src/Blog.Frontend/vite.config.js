import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vite.dev/config/
export default defineConfig({
    server: {
        proxy: {
            "/api": {
                target: "https://blog-backend-drhpexgffqbtd8e5.polandcentral-01.azurewebsites.net/",
                changeOrigin: true,
                secure: false,
            }
        }
    },
    plugins: [react()]
})
