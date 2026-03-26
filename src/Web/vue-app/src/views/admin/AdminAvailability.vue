<template>
  <div class="space-y-6">
    <!-- Recurring Schedule -->
    <div class="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
        <h2 class="text-sm font-semibold text-gray-800">{{ $t('appointment.recurring') }}</h2>
        <span v-if="savingRecurring" class="w-3 h-3 border-2 border-gray-300 border-t-brand-500 rounded-full animate-spin" />
        <Check v-else-if="savedFeedback" class="w-3.5 h-3.5 text-green-500" />
      </div>
      <div class="p-5 space-y-4">
        <div v-for="day in 7" :key="day" class="space-y-2">
          <div class="flex items-center gap-2">
            <span class="text-sm font-medium text-gray-700 w-24">{{ $t(`appointment.days.${day % 7}`) }}</span>
            <button
              @click="addSlot(day % 7)"
              class="text-xs text-brand-600 hover:text-brand-700 font-medium cursor-pointer"
            >
              + {{ $t('appointment.addSlot') }}
            </button>
          </div>
          <div v-for="(slot, idx) in getSlotsForDay(day % 7)" :key="idx" class="flex items-center gap-2 ml-26">
            <input
              type="time"
              :value="slot.startTime"
              @change="updateSlotStart(day % 7, idx, ($event.target as HTMLInputElement).value)"
              class="px-2 py-1 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300"
            />
            <span class="text-gray-400 text-sm">-</span>
            <input
              type="time"
              :value="slot.endTime"
              @change="updateSlotEnd(day % 7, idx, ($event.target as HTMLInputElement).value)"
              class="px-2 py-1 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300"
            />
            <button
              @click="removeSlot(day % 7, idx)"
              class="text-red-400 hover:text-red-600 cursor-pointer"
            >
              <Trash2 class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Overrides -->
    <div class="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <div class="px-5 py-4 border-b border-gray-100">
        <h2 class="text-sm font-semibold text-gray-800">{{ $t('appointment.overrides') }}</h2>
      </div>
      <div class="p-5 space-y-4">
        <!-- Add override form -->
        <div class="flex items-end gap-3 flex-wrap">
          <div>
            <label class="block text-xs text-gray-500 mb-1">{{ $t('appointment.selectDate') }}</label>
            <input
              type="date"
              v-model="newOverrideDate"
              :min="minDate"
              class="px-3 py-1.5 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300"
            />
          </div>
          <label class="flex items-center gap-2 text-sm text-gray-700 cursor-pointer">
            <input type="checkbox" v-model="newOverrideBlocked" class="rounded" />
            {{ $t('appointment.blockDate') }}
          </label>
          <template v-if="!newOverrideBlocked">
            <div>
              <label class="block text-xs text-gray-500 mb-1">{{ $t('appointment.customHours') }}</label>
              <div class="flex items-center gap-2">
                <input type="time" v-model="newOverrideStart" class="px-2 py-1.5 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300" />
                <span class="text-gray-400">-</span>
                <input type="time" v-model="newOverrideEnd" class="px-2 py-1.5 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300" />
              </div>
            </div>
          </template>
          <button
            @click="addOverride"
            :disabled="!newOverrideDate"
            class="px-4 py-1.5 rounded-lg text-xs font-medium text-white bg-brand-600 hover:bg-brand-500 disabled:opacity-40 transition cursor-pointer"
          >
            {{ $t('appointment.addOverride') }}
          </button>
        </div>

        <!-- Existing overrides list -->
        <div v-if="overrides.length > 0" class="divide-y divide-gray-100">
          <div v-for="override in overrides" :key="override.id" class="flex items-center justify-between py-2">
            <div class="flex items-center gap-3">
              <span class="text-sm font-medium text-gray-700">{{ formatOverrideDate(override.date) }}</span>
              <span v-if="override.isBlocked" class="px-2 py-0.5 rounded-full bg-red-50 text-red-600 text-xs font-medium">
                {{ $t('appointment.blockDate') }}
              </span>
              <span v-else class="text-sm text-gray-500">
                {{ override.startTime }} - {{ override.endTime }}
              </span>
            </div>
            <button
              @click="removeOverride(override.id!)"
              class="text-red-400 hover:text-red-600 cursor-pointer"
            >
              <Trash2 class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>
        <p v-else class="text-sm text-gray-400">{{ $t('appointment.noSlots') }}</p>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted, onUnmounted} from "vue"
import {Trash2, Check} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useAppointmentService} from "@/inversify.config"
import type {AvailabilitySlot, AvailabilityOverride} from "@/types/entities"

const {t} = useI18n()
const appointmentService = useAppointmentService()

const recurringSlots = ref<AvailabilitySlot[]>([])
const overrides = ref<AvailabilityOverride[]>([])
const savingRecurring = ref(false)
const savedFeedback = ref(false)

const newOverrideDate = ref('')
const newOverrideBlocked = ref(false)
const newOverrideStart = ref('09:00')
const newOverrideEnd = ref('17:00')

let debounceTimer: number | null = null
let feedbackTimer: number | null = null

const minDate = computed(() => DateTime.now().toFormat('yyyy-MM-dd'))

onMounted(async () => {
  try {
    const data = await appointmentService.getAvailability()
    recurringSlots.value = data.recurring
    overrides.value = data.overrides
  } catch {
    // Silently fail
  }
})

onUnmounted(() => {
  if (debounceTimer) clearTimeout(debounceTimer)
  if (feedbackTimer) clearTimeout(feedbackTimer)
})

function scheduleAutoSave() {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = window.setTimeout(() => saveRecurring(), 600)
}

function getSlotsForDay(day: number): AvailabilitySlot[] {
  return recurringSlots.value.filter(s => s.dayOfWeek === day)
}

function addSlot(day: number) {
  recurringSlots.value.push({dayOfWeek: day, startTime: '09:00', endTime: '17:00'})
  scheduleAutoSave()
}

function removeSlot(day: number, idx: number) {
  const daySlots = recurringSlots.value.filter(s => s.dayOfWeek === day)
  const slotToRemove = daySlots[idx]
  const globalIdx = recurringSlots.value.indexOf(slotToRemove)
  if (globalIdx !== -1) recurringSlots.value.splice(globalIdx, 1)
  scheduleAutoSave()
}

function updateSlotStart(day: number, idx: number, value: string) {
  const daySlots = recurringSlots.value.filter(s => s.dayOfWeek === day)
  daySlots[idx].startTime = value
  scheduleAutoSave()
}

function updateSlotEnd(day: number, idx: number, value: string) {
  const daySlots = recurringSlots.value.filter(s => s.dayOfWeek === day)
  daySlots[idx].endTime = value
  scheduleAutoSave()
}

async function saveRecurring() {
  savingRecurring.value = true
  savedFeedback.value = false
  try {
    await appointmentService.saveAvailability(recurringSlots.value)
    savingRecurring.value = false
    savedFeedback.value = true
    if (feedbackTimer) clearTimeout(feedbackTimer)
    feedbackTimer = window.setTimeout(() => { savedFeedback.value = false }, 2000)
  } catch {
    savingRecurring.value = false
  }
}

async function addOverride() {
  if (!newOverrideDate.value) return
  try {
    const override = await appointmentService.createOverride({
      date: newOverrideDate.value,
      startTime: newOverrideBlocked.value ? undefined : newOverrideStart.value,
      endTime: newOverrideBlocked.value ? undefined : newOverrideEnd.value,
      isBlocked: newOverrideBlocked.value
    })
    overrides.value.push(override)
    newOverrideDate.value = ''
    newOverrideBlocked.value = false
  } catch {
    // Silently fail
  }
}

async function removeOverride(id: string) {
  try {
    await appointmentService.deleteOverride(id)
    overrides.value = overrides.value.filter(o => o.id !== id)
  } catch {
    // Silently fail
  }
}

function formatOverrideDate(dateStr: string): string {
  return DateTime.fromISO(dateStr).toFormat('dd MMMM yyyy')
}
</script>
