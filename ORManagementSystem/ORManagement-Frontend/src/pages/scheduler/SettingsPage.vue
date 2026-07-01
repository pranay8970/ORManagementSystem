<script setup>
import { onMounted, ref } from 'vue'

import AppModal from '../../components/common/AppModal.vue'
import PageHeader from '../../components/common/PageHeader.vue'
import LoadingSpinner from '../../components/common/LoadingSpinner.vue'
import EmptyState from '../../components/common/EmptyState.vue'
import RoomsCalendar from './RoomsCalendar.vue'

import { getSettings, updateSetting } from '../../services/settingsService'
import { showToast } from '../../utils/toast'

const activeSettingsTab = ref('settings')

const loading = ref(false)
const saving = ref(false)

const settings = ref([])
const selectedSetting = ref(null)

const settingForm = ref({
  settingValue: ''
})

const loadSettings = async () => {
  loading.value = true

  try {
    const response = await getSettings()
    settings.value = response.data || []
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to load settings.'

    showToast(message, 'error')
  } finally {
    loading.value = false
  }
}

const openEdit = setting => {
  selectedSetting.value = setting

  settingForm.value = {
    settingValue: setting.settingValue
  }
}

const submitUpdate = async () => {
  if (!selectedSetting.value) return

  if (!settingForm.value.settingValue?.trim()) {
    showToast('Setting value is required.', 'warning')
    return
  }

  saving.value = true

  try {
    await updateSetting(selectedSetting.value.settingKey, {
      settingValue: settingForm.value.settingValue.trim()
    })

    showToast('Setting updated successfully.', 'success')
    selectedSetting.value = null
    await loadSettings()
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.title ||
      'Failed to update setting.'

    showToast(message, 'error')
  } finally {
    saving.value = false
  }
}

onMounted(loadSettings)
</script>

<template>
  <div>
    <PageHeader
      title="Settings"
      subtitle="Manage settings keys and operating room master data"
      icon="bi-gear"
    />

    <!-- Settings Tabs -->
    <div class="settings-tabs mb-4">
      <button
        class="settings-tab"
        :class="{ active: activeSettingsTab === 'settings' }"
        @click="activeSettingsTab = 'settings'"
      >
        <i class="bi bi-sliders me-1"></i>
        Settings Key
      </button>

      <button
        class="settings-tab"
        :class="{ active: activeSettingsTab === 'rooms' }"
        @click="activeSettingsTab = 'rooms'"
      >
        <i class="bi bi-hospital me-1"></i>
        OR Rooms
      </button>
    </div>

    <!-- Settings Key Tab -->
    <div v-if="activeSettingsTab === 'settings'">
      <LoadingSpinner v-if="loading" />

      <div v-else>
        <div class="page-card">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <div>
              <h5 class="mb-0">
                <i class="bi bi-sliders me-2 text-primary"></i>
                Settings Key
              </h5>
              <small class="text-muted">
                Manage configurable scheduling policies and threshold values.
              </small>
            </div>
          </div>

          <EmptyState
            v-if="settings.length === 0"
            title="No settings"
            message="No system settings were found."
            icon="bi-gear"
          />

          <div
            v-else
            class="table-responsive"
          >
            <table class="table table-hover align-middle">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Hospital</th>
                  <th>Key</th>
                  <th>Value</th>
                  <th class="text-end">Actions</th>
                </tr>
              </thead>

              <tbody>
                <tr
                  v-for="setting in settings"
                  :key="setting.settingId"
                >
                  <td>#{{ setting.settingId }}</td>

                  <td>
                    <span v-if="setting.hospitalId">
                      Hospital #{{ setting.hospitalId }}
                    </span>
                    <span
                      v-else
                      class="badge bg-secondary"
                    >
                      Global Default
                    </span>
                  </td>

                  <td>
                    <strong>{{ setting.settingKey }}</strong>
                  </td>

                  <td>{{ setting.settingValue }}</td>

                  <td class="text-end">
                    <button
                      class="btn btn-sm btn-outline-primary"
                      @click="openEdit(setting)"
                    >
                      Edit
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <AppModal
          :show="!!selectedSetting"
          :title="selectedSetting ? `Edit Setting — ${selectedSetting.settingKey}` : 'Edit Setting'"
          size="md"
          @close="selectedSetting = null"
        >
          <div class="mb-3">
            <label class="form-label">Setting Value</label>
            <input
              v-model="settingForm.settingValue"
              class="form-control"
            />
          </div>

          <template #footer>
            <button
              class="btn btn-outline-secondary"
              @click="selectedSetting = null"
            >
              Cancel
            </button>

            <button
              class="btn btn-primary"
              :disabled="saving"
              @click="submitUpdate"
            >
              <span
                v-if="saving"
                class="spinner-border spinner-border-sm me-2"
              ></span>
              Save
            </button>
          </template>
        </AppModal>
      </div>
    </div>

    <!-- OR Rooms Tab -->
    <div v-else-if="activeSettingsTab === 'rooms'">
      <RoomsCalendar embedded />
    </div>
  </div>
</template>

<style scoped>
.settings-tabs {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.settings-tab {
  border: 1px solid #d0d7de;
  background: #ffffff;
  color: #334155;
  padding: 10px 16px;
  border-radius: 999px;
  font-weight: 600;
  transition: all 0.15s ease;
}

.settings-tab:hover {
  border-color: #0d6efd;
  color: #0d6efd;
}

.settings-tab.active {
  background: #0d6efd;
  border-color: #0d6efd;
  color: #ffffff;
}
</style>