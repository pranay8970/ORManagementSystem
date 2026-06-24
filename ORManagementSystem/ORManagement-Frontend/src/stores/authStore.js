import { defineStore } from 'pinia'
import * as authService from '../services/authService'

const extractUser = response => {
  return (
    response?.data?.user ||
    response?.data?.data?.user ||
    response?.data?.authUser ||
    response?.data?.data ||
    response?.data ||
    null
  )
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    isAuthenticated: false,
    loading: false,
    initialized: false
  }),

  getters: {
    isSurgeon: state => state.user?.roleName === 'Surgeon',
    isScheduler: state => state.user?.roleName === 'ORScheduler'
  },

  actions: {
    async login(data) {
      this.loading = true

      try {
        const loginResponse = await authService.login(data)

        let user = extractUser(loginResponse)

        /*
          If login endpoint only sets cookie and does not return user,
          fetch /auth/me immediately after login.
        */
        if (!user || !user.roleName) {
          const meResponse = await authService.getMe()
          user = extractUser(meResponse)
        }

        if (!user || !user.roleName) {
          throw new Error('Login succeeded but user data was not returned.')
        }

        this.user = user
        this.isAuthenticated = true
        this.initialized = true

        return user
      } finally {
        this.loading = false
      }
    },

    async register(data) {
      this.loading = true

      try {
        return await authService.register(data)
      } finally {
        this.loading = false
      }
    },

    async loadUser() {
      this.loading = true

      try {
        const response = await authService.getMe()
        const user = extractUser(response)

        if (!user || !user.roleName) {
          this.user = null
          this.isAuthenticated = false
          return
        }

        this.user = user
        this.isAuthenticated = true
      } catch {
        this.user = null
        this.isAuthenticated = false
      } finally {
        this.initialized = true
        this.loading = false
      }
    },

    async refreshUser() {
      try {
        await authService.refresh()
        await this.loadUser()
      } catch {
        this.user = null
        this.isAuthenticated = false
        this.initialized = true
      }
    },

    async logout() {
      try {
        await authService.logout()
      } catch {
        // Ignore logout API failure. Frontend should still clear local state.
      } finally {
        this.user = null
        this.isAuthenticated = false
        this.initialized = true
      }
    },

    clearAuth() {
      this.user = null
      this.isAuthenticated = false
      this.initialized = true
    }
  }
})
