<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import AppModal from '../../components/common/AppModal.vue'
import PageHeader from '../../components/common/PageHeader.vue'
import LoadingSpinner from '../../components/common/LoadingSpinner.vue'
import EmptyState from '../../components/common/EmptyState.vue'
import StatusBadge from '../../components/common/StatusBadge.vue'
import { getCases, createCase, updateCase, updateCaseStatus } from '../../services/caseService'
import { getRequests } from '../../services/requestService'
import { getBlocks } from '../../services/blockService'
import { formatDate, formatDateTime, formatTime } from '../../utils/formatters'
import { showToast } from '../../utils/toast'

defineProps({
  embedded: {
    type: Boolean,
    default: false
  }
})

const loading = ref(false)
const saving = ref(false)

const cases = ref([])
const approvedRequests = ref([])
const blocks = ref([])

const statusFilter = ref('')

const selectedCase = ref(null)
const selectedStatusCase = ref(null)

const createForm = ref({
  requestId: '',
  blockId: '',
  scheduledStart: '',
  scheduledEnd: ''
})

const updateForm = ref({
  scheduledStart: '',
  scheduledEnd: ''
})

const statusForm = ref({
  status: 'InProgress',
  actualStart: '',
  actualEnd: '',
  cancellationReason: ''
})

const toDateTimeLocal = value => {
  if (!value) return ''

  const date = new Date(value)
  const offset = date.getTimezoneOffset()
  const localDate = new Date(date.getTime() - offset * 60000)

  return localDate.toISOString().slice(0, 16)
}

const toDatePart = value => {
  if (!value) return ''

  if (typeof value === 'string') {
    return value.substring(0, 10)
  }

  return new Date(value).toISOString().substring(0, 10)
}

const toTimePart = value => {
  if (!value) return ''

  return formatTime(value).substring(0, 5)
}

const toDateTimeLocalFromParts = (dateValue, timeValue) => {
  const datePart = toDatePart(dateValue)
  const timePart = toTimePart(timeValue)

  if (!datePart || !timePart) return ''

  return `${datePart}T${timePart}`
}

const addMinutesToDateTimeLocal = (dateTimeLocal, minutes) => {
  if (!dateTimeLocal || !minutes) return ''

  const date = new Date(dateTimeLocal)
  date.setMinutes(date.getMinutes() + Number(minutes))

  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hour = String(date.getHours()).padStart(2, '0')
  const minute = String(date.getMinutes()).padStart(2, '0')

  return `${year}-${month}-${day}T${hour}:${minute}`
}

const getMinutesBetweenTimes = (startTime, endTime) => {
  const start = toTimePart(startTime)
  const end = toTimePart(endTime)

  if (!start || !end) return 0

  const [startHour, startMinute] = start.split(':').map(Number)
  const [endHour, endMinute] = end.split(':').map(Number)

  return (endHour * 60 + endMinute) - (startHour * 60 + startMinute)
}

const getMinutesBetweenDateTimes = (start, end) => {
  if (!start || !end) return 0

  const startDate = new Date(start)
  const endDate = new Date(end)

  return Math.round((endDate - startDate) / 60000)
}

const getDayMaskFromDate = dateValue => {
  const date = new Date(toDatePart(dateValue))
  const day = date.getDay()

  const map = {
    1: 1,
    2: 2,
    3: 4,
    4: 8,
    5: 16
  }

  return map[day] || 0
}

const isBlockDateAllowedForRequest = (request, block) => {
  if (!request || !block) return false

  if (!request.availableDaysMask) {
    return true
  }

  const blockDayMask = getDayMaskFromDate(block.blockDate)

  if (!blockDayMask) {
    return false
  }

  return (Number(request.availableDaysMask) & blockDayMask) !== 0
}

const selectedRequest = computed(() => {
  if (!createForm.value.requestId) return null

  return approvedRequests.value.find(
    request => Number(request.requestId) === Number(createForm.value.requestId)
  ) || null
})

const selectedBlock = computed(() => {
  if (!createForm.value.blockId) return null

  return blocks.value.find(
    block => Number(block.blockId) === Number(createForm.value.blockId)
  ) || null
})

const blockDurationMinutes = computed(() => {
  if (!selectedBlock.value) return 0

  return getMinutesBetweenTimes(
    selectedBlock.value.startTime,
    selectedBlock.value.endTime
  )
})

const scheduledDurationMinutes = computed(() => {
  return getMinutesBetweenDateTimes(
    createForm.value.scheduledStart,
    createForm.value.scheduledEnd
  )
})

const compatibleBlocks = computed(() => {
  const request = selectedRequest.value

  if (!request) {
    return blocks.value.filter(block =>
      block.blockStatus !== 'Cancelled' &&
      block.blockStatus !== 'Released'
    )
  }

  return blocks.value.filter(block => {
    if (
      block.blockStatus === 'Cancelled' ||
      block.blockStatus === 'Released'
    ) {
      return false
    }

    if (!isBlockDateAllowedForRequest(request, block)) {
      return false
    }

    if (block.blockType === 'Recurring') {
      return Number(block.surgeonId) === Number(request.surgeonId)
    }

    if (block.blockType === 'Emergency') {
      return request.priority === 'Emergency'
    }

    if (block.blockType === 'AdHoc') {
      return !block.surgeonId ||
        Number(block.surgeonId) === Number(request.surgeonId)
    }

    if (block.blockType === 'Open') {
      return true
    }

    return false
  })
})

const selectedBlockFitsRequest = computed(() => {
  if (!selectedRequest.value || !selectedBlock.value) {
    return true
  }

  return blockDurationMinutes.value >= Number(selectedRequest.value.estimatedDurationMin || 0)
})

const scheduledDurationIsEnough = computed(() => {
  if (!selectedRequest.value || !createForm.value.scheduledStart || !createForm.value.scheduledEnd) {
    return true
  }

  return scheduledDurationMinutes.value >= Number(selectedRequest.value.estimatedDurationMin || 0)
})

const requestSummary = computed(() => selectedRequest.value)
const blockSummary = computed(() => selectedBlock.value)

const autoFillSchedule = () => {
  const request = selectedRequest.value
  const block = selectedBlock.value

  if (!request || !block) return

  const start = toDateTimeLocalFromParts(block.blockDate, block.startTime)

  if (!start) return

  createForm.value.scheduledStart = start

  const estimatedMinutes = Number(request.estimatedDurationMin || 0)
  const proposedEnd = addMinutesToDateTimeLocal(start, estimatedMinutes)

  const blockEnd = toDateTimeLocalFromParts(block.blockDate, block.endTime)

  if (proposedEnd && blockEnd && new Date(proposedEnd) <= new Date(blockEnd)) {
    createForm.value.scheduledEnd = proposedEnd
  } else {
    createForm.value.scheduledEnd = blockEnd
  }
}

watch(
  () => createForm.value.requestId,
  () => {
    createForm.value.blockId = ''
    createForm.value.scheduledStart = ''
    createForm.value.scheduledEnd = ''
  }
)

watch(
  () => createForm.value.blockId,
  () => {
    autoFillSchedule()
  }
)

const loadCases = async () => {
  loading.value = true

  try {
    const params = {}

    if (statusFilter.value) {
      params.status = statusFilter.value
    }

    const response = await getCases(params)
    cases.value = response.data || []
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to load cases.'

    showToast(message, 'error')
  } finally {
    loading.value = false
  }
}

const loadSupportingData = async () => {
  try {
    const [approvedResponse, modifiedResponse, blocksResponse] = await Promise.all([
      getRequests({ status: 'Approved' }),
      getRequests({ status: 'Modified' }),
      getBlocks()
    ])

    approvedRequests.value = [
      ...(approvedResponse.data || []),
      ...(modifiedResponse.data || [])
    ]

    blocks.value = blocksResponse.data || []
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to load scheduling data.'

    showToast(message, 'error')
  }
}

const resetCreateForm = () => {
  createForm.value = {
    requestId: '',
    blockId: '',
    scheduledStart: '',
    scheduledEnd: ''
  }
}

const submitCreateCase = async () => {
  if (!createForm.value.requestId || !createForm.value.blockId) {
    showToast('Request and block are required.', 'warning')
    return
  }

  if (!createForm.value.scheduledStart || !createForm.value.scheduledEnd) {
    showToast('Scheduled start and end are required.', 'warning')
    return
  }

  if (!selectedBlockFitsRequest.value) {
    showToast('Selected block is shorter than the request estimated duration.', 'warning')
    return
  }

  if (!scheduledDurationIsEnough.value) {
    showToast('Scheduled duration is less than request estimated duration.', 'warning')
    return
  }

  saving.value = true

  try {
    await createCase({
      requestId: Number(createForm.value.requestId),
      blockId: Number(createForm.value.blockId),
      scheduledStart: createForm.value.scheduledStart,
      scheduledEnd: createForm.value.scheduledEnd
    })

    showToast('Surgical case created successfully.', 'success')
    resetCreateForm()
    await Promise.all([loadCases(), loadSupportingData()])
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to create surgical case.'

    showToast(message, 'error')
  } finally {
    saving.value = false
  }
}

const openUpdateCase = item => {
  selectedCase.value = item

  updateForm.value = {
    scheduledStart: toDateTimeLocal(item.scheduledStart),
    scheduledEnd: toDateTimeLocal(item.scheduledEnd)
  }
}

const submitUpdateCase = async () => {
  if (!selectedCase.value) return

  if (!updateForm.value.scheduledStart || !updateForm.value.scheduledEnd) {
    showToast('Scheduled start and end are required.', 'warning')
    return
  }

  saving.value = true

  try {
    await updateCase(selectedCase.value.surgeryId, {
      scheduledStart: updateForm.value.scheduledStart,
      scheduledEnd: updateForm.value.scheduledEnd
    })

    showToast('Case schedule updated successfully.', 'success')
    selectedCase.value = null
    await loadCases()
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to update case.'

    showToast(message, 'error')
  } finally {
    saving.value = false
  }
}

const openStatusCase = item => {
  selectedStatusCase.value = item

  statusForm.value = {
    status: item.caseStatus === 'Scheduled' ? 'InProgress' : 'Completed',
    actualStart: '',
    actualEnd: '',
    cancellationReason: ''
  }
}

const submitStatusUpdate = async () => {
  if (!selectedStatusCase.value) return

  if (!statusForm.value.status) {
    showToast('Please select a status.', 'warning')
    return
  }

  if (statusForm.value.status === 'Cancelled' && !statusForm.value.cancellationReason) {
    showToast('Cancellation reason is required.', 'warning')
    return
  }

  saving.value = true

  try {
    const payload = {
      status: statusForm.value.status,
      actualStart: statusForm.value.actualStart || null,
      actualEnd: statusForm.value.actualEnd || null,
      cancellationReason: statusForm.value.cancellationReason || null
    }

    await updateCaseStatus(selectedStatusCase.value.surgeryId, payload)

    showToast('Case status updated successfully.', 'success')
    selectedStatusCase.value = null
    await loadCases()
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to update case status.'

    showToast(message, 'error')
  } finally {
    saving.value = false
  }
}

const canEditCase = item => {
  return item.caseStatus === 'Scheduled'
}

const canChangeStatus = item => {
  return item.caseStatus !== 'Completed' && item.caseStatus !== 'Cancelled'
}

onMounted(async () => {
  await Promise.all([loadCases(), loadSupportingData()])
})
</script>

<template>
  <div>
    <PageHeader
      v-if="!embedded"
      title="Case Management"
      subtitle="Schedule and manage surgical cases"
      icon="bi-clipboard2-pulse"
    />

    <!-- Create case -->
    <div class="page-card mb-4">
      <h5 class="mb-3">
        <i class="bi bi-plus-circle me-2 text-primary"></i>
        Create Surgical Case
      </h5>

      <div class="row g-3">
        <div class="col-md-4">
          <label class="form-label">Approved / Modified Request</label>
          <select v-model="createForm.requestId" class="form-select">
            <option value="">Select request</option>
            <option
              v-for="request in approvedRequests"
              :key="request.requestId"
              :value="request.requestId"
            >
              #{{ request.requestId }}
              -
              {{ request.surgeonName }}
              -
              {{ request.patientName }}
              -
              {{ request.surgeryType }}
              -
              {{ request.estimatedDurationMin }} min
            </option>
          </select>
        </div>

        <div class="col-md-4">
          <label class="form-label">Compatible Block</label>
          <select
            v-model="createForm.blockId"
            class="form-select"
            :disabled="!createForm.requestId"
          >
            <option value="">Select block</option>
            <option
              v-for="block in compatibleBlocks"
              :key="block.blockId"
              :value="block.blockId"
            >
              #{{ block.blockId }}
              -
              {{ block.blockType }}
              -
              {{ block.roomName }}
              -
              {{ block.surgeonName || `${block.blockType} Capacity` }}
              -
              {{ formatDate(block.blockDate) }}
              {{ formatTime(block.startTime) }}-{{ formatTime(block.endTime) }}
            </option>
          </select>
          <small v-if="createForm.requestId && compatibleBlocks.length === 0" class="text-danger">
            No compatible blocks found for this request.
          </small>
        </div>

        <div class="col-md-2">
          <label class="form-label">Scheduled Start</label>
          <input
            v-model="createForm.scheduledStart"
            type="datetime-local"
            class="form-control"
          />
        </div>

        <div class="col-md-2">
          <label class="form-label">Scheduled End</label>
          <input
            v-model="createForm.scheduledEnd"
            type="datetime-local"
            class="form-control"
          />
        </div>
      </div>

      <div v-if="requestSummary || blockSummary" class="row g-3 mt-3">
        <div v-if="requestSummary" class="col-lg-6">
          <div class="summary-card">
            <h6 class="mb-3">Selected Request</h6>

            <div class="d-flex justify-content-between mb-2">
              <span>Surgeon</span>
              <strong>{{ requestSummary.surgeonName }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Patient</span>
              <strong>{{ requestSummary.patientName }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Surgery</span>
              <strong>{{ requestSummary.surgeryType }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Estimated Duration</span>
              <strong>{{ requestSummary.estimatedDurationMin }} min</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Priority</span>
              <strong>{{ requestSummary.priority }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Readiness</span>
              <strong>{{ requestSummary.patientReadiness }}</strong>
            </div>

            <div class="d-flex justify-content-between">
              <span>Availability</span>
              <strong>{{ requestSummary.availableDaysDisplay }}</strong>
            </div>
          </div>
        </div>

        <div v-if="blockSummary" class="col-lg-6">
          <div class="summary-card">
            <h6 class="mb-3">Selected Block</h6>

            <div class="d-flex justify-content-between mb-2">
              <span>Type</span>
              <strong>{{ blockSummary.blockType }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Room</span>
              <strong>{{ blockSummary.roomName }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Capacity Owner</span>
              <strong>
                {{ blockSummary.surgeonName || `${blockSummary.blockType} Capacity` }}
              </strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Date</span>
              <strong>{{ formatDate(blockSummary.blockDate) }}</strong>
            </div>

            <div class="d-flex justify-content-between mb-2">
              <span>Time</span>
              <strong>
                {{ formatTime(blockSummary.startTime) }} -
                {{ formatTime(blockSummary.endTime) }}
              </strong>
            </div>

            <div class="d-flex justify-content-between">
              <span>Block Duration</span>
              <strong>{{ blockDurationMinutes }} min</strong>
            </div>
          </div>
        </div>
      </div>

      <div v-if="selectedRequest && selectedBlock && !selectedBlockFitsRequest" class="alert alert-warning mt-3 mb-0">
        Request duration is {{ selectedRequest.estimatedDurationMin }} min, but selected block capacity is only
        {{ blockDurationMinutes }} min.
      </div>

      <div v-if="selectedRequest && createForm.scheduledStart && createForm.scheduledEnd && !scheduledDurationIsEnough" class="alert alert-warning mt-3 mb-0">
        Scheduled duration is {{ scheduledDurationMinutes }} min, but request estimated duration is
        {{ selectedRequest.estimatedDurationMin }} min.
      </div>

      <div class="text-end mt-3">
        <button
          class="btn btn-primary"
          :disabled="saving"
          @click="submitCreateCase"
        >
          <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
          Create Case
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="page-card mb-4">
      <div class="row g-3 align-items-end">
        <div class="col-md-4">
          <label class="form-label">Filter by Status</label>
          <select v-model="statusFilter" class="form-select">
            <option value="">All Statuses</option>
            <option value="Scheduled">Scheduled</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <div class="col-md-3">
          <button class="btn btn-primary w-100" @click="loadCases">
            <i class="bi bi-search me-1"></i>
            Apply Filter
          </button>
        </div>

        <div class="col-md-3">
          <button
            class="btn btn-outline-secondary w-100"
            @click="statusFilter = ''; loadCases()"
          >
            Clear
          </button>
        </div>
      </div>
    </div>

    <LoadingSpinner v-if="loading" />

    <!-- Cases table -->
    <div v-else class="page-card">
      <EmptyState
        v-if="cases.length === 0"
        title="No cases found"
        message="No surgical cases match the selected filter."
        icon="bi-hospital"
      />

      <div v-else class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Case</th>
              <th>Patient</th>
              <th>Surgeon</th>
              <th>Room</th>
              <th>Surgery</th>
              <th>Scheduled</th>
              <th>Actual</th>
              <th>Status</th>
              <th class="text-end">Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="item in cases" :key="item.surgeryId">
              <td>#{{ item.surgeryId }}</td>

              <td>
                <div>{{ item.patientName }}</div>
                <small class="text-muted">{{ item.patientMrn }}</small>
              </td>

              <td>{{ item.surgeonName }}</td>
              <td>{{ item.roomName }}</td>
              <td>{{ item.surgeryType }}</td>

              <td>
                <div>{{ formatDateTime(item.scheduledStart) }}</div>
                <small class="text-muted">
                  to {{ formatDateTime(item.scheduledEnd) }}
                </small>
              </td>

              <td>
                <div>{{ formatDateTime(item.actualStart) }}</div>
                <small class="text-muted">
                  to {{ formatDateTime(item.actualEnd) }}
                </small>
              </td>

              <td>
                <StatusBadge :status="item.caseStatus" />
              </td>

              <td class="text-end">
                <button
                  class="btn btn-sm btn-outline-primary me-2"
                  :disabled="!canEditCase(item)"
                  @click="openUpdateCase(item)"
                >
                  Edit
                </button>

                <button
                  class="btn btn-sm btn-outline-success"
                  :disabled="!canChangeStatus(item)"
                  @click="openStatusCase(item)"
                >
                  Status
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Update schedule card -->
    <AppModal
      :show="!!selectedCase"
      :title="selectedCase ? `Update Case #${selectedCase.surgeryId}` : 'Update Case'"
      size="lg"
      @close="selectedCase = null"
    >
      <div class="row g-3">
        <div class="col-md-6">
          <label class="form-label">Scheduled Start</label>
          <input
            v-model="updateForm.scheduledStart"
            type="datetime-local"
            class="form-control"
          />
        </div>

        <div class="col-md-6">
          <label class="form-label">Scheduled End</label>
          <input
            v-model="updateForm.scheduledEnd"
            type="datetime-local"
            class="form-control"
          />
        </div>
      </div>

      <template #footer>
        <button class="btn btn-outline-secondary" @click="selectedCase = null">
          Cancel
        </button>

        <button
          class="btn btn-primary"
          :disabled="saving"
          @click="submitUpdateCase"
        >
          <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
          Save Schedule
        </button>
      </template>
    </AppModal>

    <!-- Update status card -->
    <AppModal
      :show="!!selectedStatusCase"
      :title="selectedStatusCase ? `Update Status — Case #${selectedStatusCase.surgeryId}` : 'Update Status'"
      size="lg"
      @close="selectedStatusCase = null"
    >
      <div class="row g-3">
        <div class="col-md-4">
          <label class="form-label">New Status</label>
          <select v-model="statusForm.status" class="form-select">
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <div v-if="statusForm.status === 'InProgress'" class="col-md-4">
          <label class="form-label">Actual Start</label>
          <input
            v-model="statusForm.actualStart"
            type="datetime-local"
            class="form-control"
          />
        </div>

        <div v-if="statusForm.status === 'Completed'" class="col-md-4">
          <label class="form-label">Actual End</label>
          <input
            v-model="statusForm.actualEnd"
            type="datetime-local"
            class="form-control"
          />
        </div>

        <div v-if="statusForm.status === 'Cancelled'" class="col-md-6">
          <label class="form-label">Cancellation Reason</label>
          <select v-model="statusForm.cancellationReason" class="form-select">
            <option value="">Select reason</option>
            <option value="SurgeonCancelled">Surgeon Cancelled</option>
            <option value="PatientNoShow">Patient No Show</option>
            <option value="PatientNotCleared">Patient Not Cleared</option>
            <option value="EmergencyBump">Emergency Bump</option>
            <option value="Other">Other</option>
          </select>
        </div>
      </div>

      <template #footer>
        <button class="btn btn-outline-secondary" @click="selectedStatusCase = null">
          Cancel
        </button>

        <button
          class="btn btn-success"
          :disabled="saving"
          @click="submitStatusUpdate"
        >
          <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
          Update Status
        </button>
      </template>
    </AppModal>
  </div>
</template>

<style scoped>
.summary-card {
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 16px;
  background: #f9fafb;
}
</style>
``