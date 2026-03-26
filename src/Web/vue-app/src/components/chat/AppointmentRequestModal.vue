<template>
  <Teleport to="body">
    <div class="fixed inset-0 z-50 flex items-center justify-center">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-black/40" @click="$emit('close')" />

      <!-- Modal -->
      <div class="relative bg-white rounded-2xl shadow-2xl w-[420px] max-h-[85vh] overflow-hidden flex flex-col z-10">
        <!-- Header -->
        <div class="bg-brand-900 px-5 py-4 flex items-center justify-between shrink-0">
          <h3 class="text-white text-sm font-semibold">{{ $t('appointment.modalTitle') }}</h3>
          <button @click="$emit('close')" class="text-gray-400 hover:text-white transition cursor-pointer">
            <X class="w-4 h-4" />
          </button>
        </div>

        <!-- Content -->
        <div class="flex-1 overflow-y-auto p-5 space-y-4">
          <!-- Calendar -->
          <div>
            <!-- Month navigation -->
            <div class="flex items-center justify-between mb-3">
              <button @click="prevMonth" class="p-1 rounded-lg hover:bg-gray-100 transition cursor-pointer">
                <ChevronLeft class="w-4 h-4 text-gray-600" />
              </button>
              <span class="text-sm font-semibold text-gray-800 capitalize">
                {{ currentMonthLabel }}
              </span>
              <button @click="nextMonth" class="p-1 rounded-lg hover:bg-gray-100 transition cursor-pointer">
                <ChevronRight class="w-4 h-4 text-gray-600" />
              </button>
            </div>

            <!-- Day headers -->
            <div class="grid grid-cols-7 gap-1 mb-1">
              <div v-for="d in dayHeaders" :key="d" class="text-center text-[10px] font-medium text-gray-400 uppercase">
                {{ d }}
              </div>
            </div>

            <!-- Calendar grid -->
            <div v-if="loadingMonth" class="flex justify-center py-8">
              <div class="w-5 h-5 border-2 border-brand-200 border-t-brand-500 rounded-full animate-spin" />
            </div>
            <div v-else class="grid grid-cols-7 gap-1">
              <template v-for="cell in calendarCells" :key="cell.key">
                <!-- Empty cell -->
                <div v-if="!cell.date" class="w-full aspect-square" />
                <!-- Day cell -->
                <button
                  v-else
                  @click="selectDate(cell.dateStr)"
                  :disabled="cell.isPast || cell.totalSlots === 0"
                  class="w-full aspect-square rounded-lg flex flex-col items-center justify-center gap-0.5 text-xs transition relative"
                  :class="[
                    selectedDate === cell.dateStr
                      ? 'bg-brand-500 text-white ring-2 ring-brand-300'
                      : cell.isPast || cell.totalSlots === 0
                        ? 'text-gray-300 cursor-not-allowed'
                        : 'text-gray-700 hover:bg-gray-50 cursor-pointer'
                  ]"
                >
                  <span class="font-medium leading-none">{{ cell.day }}</span>
                  <!-- Availability dot -->
                  <span
                    v-if="!cell.isPast && cell.totalSlots > 0"
                    class="w-1.5 h-1.5 rounded-full"
                    :class="dotColor(cell)"
                  />
                </button>
              </template>
            </div>
          </div>

          <!-- Time slots for selected date -->
          <div v-if="selectedDate">
            <label class="block text-xs font-medium text-gray-600 mb-1.5">{{ $t('appointment.selectTime') }}</label>
            <div v-if="filteredSlots.length === 0" class="text-sm text-gray-400 text-center py-3">
              {{ $t('appointment.noSlots') }}
            </div>
            <div v-else class="grid grid-cols-3 gap-2">
              <button
                v-for="slot in filteredSlots"
                :key="slot.date"
                @click="selectedSlot = slot"
                class="px-3 py-2 rounded-lg text-sm font-medium transition cursor-pointer"
                :class="selectedSlot?.date === slot.date
                  ? 'bg-brand-500 text-white'
                  : 'bg-gray-50 text-gray-700 hover:bg-brand-50 hover:text-brand-600 border border-gray-200'"
              >
                {{ slot.startTime }}
              </button>
            </div>
          </div>

          <!-- Motif -->
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1.5">{{ $t('appointment.motif') }}</label>
            <textarea
              v-model="motif"
              :placeholder="$t('appointment.motifPlaceholder')"
              rows="2"
              class="w-full px-3 py-2 rounded-lg border border-gray-200 text-sm focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 resize-none"
            />
          </div>
        </div>

        <!-- Footer -->
        <div class="shrink-0 border-t border-gray-200 px-5 py-3 flex gap-2">
          <button
            @click="$emit('close')"
            class="flex-1 px-4 py-2 rounded-lg text-sm font-medium text-gray-600 bg-gray-100 hover:bg-gray-200 transition cursor-pointer"
          >
            {{ $t('appointment.cancel') }}
          </button>
          <button
            @click="submitRequest"
            :disabled="!selectedSlot || submitting"
            class="flex-1 px-4 py-2 rounded-lg text-sm font-medium text-white bg-brand-600 hover:bg-brand-500 disabled:opacity-40 disabled:cursor-not-allowed transition cursor-pointer"
          >
            <span v-if="submitting" class="flex items-center justify-center gap-2">
              <div class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            </span>
            <span v-else>{{ $t('appointment.submit') }}</span>
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script lang="ts" setup>
import {ref, computed, watch, onMounted} from "vue"
import {X, ChevronLeft, ChevronRight} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useAppointmentService} from "@/inversify.config"
import {useChatStore} from "@/stores/chatStore"
import type {AvailableSlot} from "@/types/entities"

const emit = defineEmits<{
  close: []
  sent: []
}>()

const {t} = useI18n()
const appointmentService = useAppointmentService()
const chatStore = useChatStore()

const currentMonth = ref(DateTime.now().startOf('month'))
const selectedDate = ref('')
const selectedSlot = ref<AvailableSlot | null>(null)
const motif = ref('')
const allSlots = ref<AvailableSlot[]>([])
const loadingMonth = ref(false)
const submitting = ref(false)

const dayHeaders = ['Lu', 'Ma', 'Me', 'Je', 'Ve', 'Sa', 'Di']

const currentMonthLabel = computed(() =>
  currentMonth.value.setLocale('fr').toFormat('MMMM yyyy')
)

interface CalendarCell {
  key: string
  date: DateTime | null
  dateStr: string
  day: number
  isPast: boolean
  totalSlots: number
  availableSlots: number
}

const calendarCells = computed<CalendarCell[]>(() => {
  const start = currentMonth.value
  const daysInMonth = start.daysInMonth || 30
  // Monday=1, so offset is (startDayOfWeek - 1) for Monday-start calendar
  const firstDayOfWeek = start.weekday // 1=Mon, 7=Sun
  const offset = firstDayOfWeek - 1

  const cells: CalendarCell[] = []

  // Empty cells before first day
  for (let i = 0; i < offset; i++) {
    cells.push({key: `empty-${i}`, date: null, dateStr: '', day: 0, isPast: false, totalSlots: 0, availableSlots: 0})
  }

  const today = DateTime.now().startOf('day')

  for (let d = 1; d <= daysInMonth; d++) {
    const dt = start.set({day: d})
    const dateStr = dt.toFormat('yyyy-MM-dd')
    const isPast = dt < today

    // Count slots for this day
    const daySlotsAll = allSlots.value.filter(s => {
      const slotDate = DateTime.fromISO(s.date).toFormat('yyyy-MM-dd')
      return slotDate === dateStr
    })

    cells.push({
      key: dateStr,
      date: dt,
      dateStr,
      day: d,
      isPast,
      totalSlots: daySlotsAll.length,
      availableSlots: daySlotsAll.length
    })
  }

  return cells
})

const filteredSlots = computed(() => {
  if (!selectedDate.value) return []
  return allSlots.value.filter(s => {
    const slotDate = DateTime.fromISO(s.date).toFormat('yyyy-MM-dd')
    return slotDate === selectedDate.value
  })
})

function dotColor(cell: CalendarCell): string {
  if (cell.totalSlots === 0) return ''
  if (cell.totalSlots >= 3) return 'bg-green-500'
  if (cell.totalSlots === 2) return 'bg-yellow-500'
  return 'bg-red-500'
}

function prevMonth() {
  currentMonth.value = currentMonth.value.minus({months: 1})
}

function nextMonth() {
  currentMonth.value = currentMonth.value.plus({months: 1})
}

function selectDate(dateStr: string) {
  selectedDate.value = dateStr
  selectedSlot.value = null
}

async function loadMonthSlots() {
  loadingMonth.value = true
  try {
    const start = currentMonth.value.toFormat('yyyy-MM-dd')
    const end = currentMonth.value.endOf('month').toFormat('yyyy-MM-dd')
    allSlots.value = await appointmentService.getAvailableSlots(start, end)
  } catch {
    allSlots.value = []
  } finally {
    loadingMonth.value = false
  }
}

watch(currentMonth, () => {
  selectedDate.value = ''
  selectedSlot.value = null
  loadMonthSlots()
})

onMounted(() => {
  loadMonthSlots()
})

async function submitRequest() {
  if (!selectedSlot.value) return
  submitting.value = true
  try {
    const msg = await appointmentService.requestAppointment(
      selectedSlot.value.date,
      motif.value.trim() || undefined
    )
    chatStore.addMessage(msg.conversationId, msg)
    emit('sent')
    emit('close')
  } catch (err: any) {
    if (err?.response?.status === 409) {
      alert(t('appointment.conflict'))
      await loadMonthSlots()
      selectedSlot.value = null
    } else {
      alert(t('appointment.requestError'))
    }
  } finally {
    submitting.value = false
  }
}
</script>
