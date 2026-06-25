import { defineStore } from 'pinia'

export const useSchedulerUIStore = defineStore('schedulerUI', {
  state: () => ({

    // ✅ BLOCK MANAGEMENT
    blockTab: 'blocks',
    blockFilters: {
      fromDate: '2026-06-22',
      toDate: '2026-06-26',
      surgeonId: '',
      roomId: ''
    },
    generateForm: {
      fromDate: '2026-06-22',
      toDate: '2026-06-26'
    },

    // ✅ REQUEST APPROVAL
    approvalTab: 'requests',
    requestStatusFilter: 'PendingReview',
    approvalStatusForm: {
      status: 'Approved',
      schedulerRemarks: ''
    },

    // ✅ CASE MANAGEMENT
    caseStatusFilter: '',
    createForm: {
      requestId: '',
      blockId: '',
      scheduledStart: '',
      scheduledEnd: ''
    },
    updateForm: {
      scheduledStart: '',
      scheduledEnd: ''
    },
    caseStatusForm: {
      status: 'InProgress',
      actualStart: '',
      actualEnd: '',
      cancellationReason: ''
    },

    // ✅ WAITLIST & BACKFILL
    waitlistTab: 'slots',
    slotFilters: {
      state: 'Available',
      fromDate: '2026-06-22',
      toDate: '2026-06-26'
    },
    slotStatusForm: {
      slotState: 'Matched'
    },

    // ✅ UTILIZATION
    utilizationTab: 'blocks',

    utilizationBlockFilters: {
      fromDate: '2026-06-22',
      toDate: '2026-06-26',
      surgeonId: '',
      roomId: '',
      status: ''
    },

    utilizationRoomFilters: {
      fromDate: '2026-06-22',
      toDate: '2026-06-22',
      roomId: '',
      status: ''
    },

    calculateBlockForm: {
      blockId: '',
      fromDate: '2026-06-22',
      toDate: '2026-06-26'
    },

    calculateRoomForm: {
      weekStartDate: '2026-06-22',
      orRoomId: ''
    },

    reportForm: {
      weekStartDate: '2026-06-22'
    }
  }),

  actions: {
    // ✅ Block
    setBlockTab(tab) {
      this.blockTab = tab
    },

    // ✅ Approval
    setApprovalTab(tab) {
      this.approvalTab = tab
    },
    setRequestFilter(val) {
      this.requestStatusFilter = val
    },

    // ✅ Case
    setCaseFilter(val) {
      this.caseStatusFilter = val
    },
    setCreateForm(key, val) {
      this.createForm[key] = val
    },
    resetCreateForm() {
      this.createForm = {
        requestId: '',
        blockId: '',
        scheduledStart: '',
        scheduledEnd: ''
      }
    },

    // ✅ Waitlist
    setWaitlistTab(tab) {
      this.waitlistTab = tab
    },

    // ✅ Utilization
    setUtilizationTab(tab) {
      this.utilizationTab = tab
    }
  },

  persist: {
    key: 'scheduler-ui',
    storage: localStorage
  }
})