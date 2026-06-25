import { createApp } from 'vue'
import App from './App.vue'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import router from './router'

// toast
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

// bootstrap
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import './assets/css/style.css'

// toast options
const toastOptions = {
  position: 'top-right',
  timeout: 3000,
  closeOnClick: true,
  pauseOnHover: true
}

const app = createApp(App)

// ✅ CREATE PINIA INSTANCE
const pinia = createPinia()

// ✅ ADD PERSISTENCE PLUGIN
pinia.use(piniaPluginPersistedstate)

// ✅ USE PINIA
app.use(pinia)

app.use(router)
app.use(Toast, toastOptions)

app.mount('#app')