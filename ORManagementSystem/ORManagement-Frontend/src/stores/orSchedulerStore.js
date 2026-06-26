import { defineStore } from 'pinia'

export const useOrSchedulerStore = defineStore('orSchedulerStore', {
  state: () => ({
    activeTab: 'blocks',

    // Block filters
    blockFilters: {
      fromDate: '2026-06-22',
      toDate: '2026-06-26',
      surgeonId: '',
      roomId: '',
      status: ''
    },

    // Room filters
    roomFilters: {
      fromDate: '2026-06-22',
      toDate: '2026-06-22',
      roomId: '',
      status: ''
    },

    // Block calculation form
    calculateForm: {
      blockId: '',
      fromDate: '2026-06-22',
      toDate: '2026-06-26'
    },

    // Room calculation form
    roomCalculateForm: {
      weekStartDate: '2026-06-22',
      orRoomId: ''
    },

    // Weekly report form
    reportForm: {
      weekStartDate: '2026-06-22'
    },

    // Persisted report
    weeklyReport: null
  }),

  persist: {
    key: 'orSchedulerStore',
    storage: localStorage,
    paths: [
      'activeTab',
      'blockFilters',
      'roomFilters',
      'calculateForm',
      'roomCalculateForm',
      'reportForm',
      'weeklyReport'
    ]
  }
})
