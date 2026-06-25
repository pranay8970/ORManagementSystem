import { defineStore } from 'pinia'

export const useSurgeonRequestStore = defineStore('surgeonRequest', {
  state: () => ({
    form: {
      patientId: '',
      surgeryType: '',
      estimatedDurationMin: '',
      priority: 'Elective',
      patientReadiness: 'Ready',
      remarks: ''
    }
  }),

  actions: {
    resetForm() {
      this.form = {
        patientId: '',
        surgeryType: '',
        estimatedDurationMin: '',
        priority: 'Elective',
        patientReadiness: 'Ready',
        remarks: ''
      }
    }
  },

  // ✅ THIS IS KEY
  persist: true
})