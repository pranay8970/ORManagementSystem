<template>
  <router-view />
</template>

<script>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'

export default {
  setup() {
    const auth = useAuthStore()

    onMounted(async () => {
      try {
        // ✅ Always verify user session on app load
        await auth.loadUser()
      } catch (error) {
        console.error('Failed to load user:', error)
      }
    })

    return {}
  }
}
</script>
