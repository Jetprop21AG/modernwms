import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue()
  ],
  resolve: {
    alias: {
      // 配置别名指向src目录
      '@': resolve(__dirname, 'src'),
      'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js'
    },
    // 使用别名的文件后缀
    extensions: ['.js', '.json', '.ts']
  },
  css: {
    preprocessorOptions: {
      less: {
        javascriptEnabled: true
      }
    }
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
    chunkSizeWarningLimit: 1500,
    rollupOptions: {
      output: {
        manualChunks: {
          'vendor': ['vue', 'vue-router', 'vuex', 'axios'],
          'vuetify': ['vuetify'],
          'excel': ['exceljs', 'xlsx', 'vxe-table']
        }
      }
    }
  },
  server: {
    port: parseInt(process.env.VITE_CLI_PORT || '5173')
  }
})
