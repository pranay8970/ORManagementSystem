import { defineStore } from 'pinia'

const getDefaultForm = () => ({
  patientId: '',
  surgeryType: '',
  estimatedDurationMin: '',
  priority: 'Elective',
  patientReadiness: 'Ready',
  remarks: ''
})

export const useSurgeonRequestStore = defineStore('surgeonRequest', {
  state: () => ({
    form: getDefaultForm()
  }),

  actions: {
    resetForm() {
      this.form = getDefaultForm()
    }
  },

  // ✅ Persist ONLY the form safely
  persist: {
    key: 'surgeon-request-store',
    storage: localStorage,

    // ✅ important: persist only what you need
    paths: ['form']
  }
})
