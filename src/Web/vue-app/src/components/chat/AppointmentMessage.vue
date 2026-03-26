<template>
  <div class="w-full flex" :class="msg.senderId === currentUserId ? 'justify-end' : 'justify-start'">
    <div class="max-w-[85%] rounded-2xl overflow-hidden border" :class="borderClass">
      <!-- Header -->
      <div class="px-3 py-2 flex items-center gap-2" :class="headerClass">
        <Calendar class="w-4 h-4" />
        <span class="text-xs font-semibold">
          {{ msg.type === MessageType.AppointmentRequest ? $t('appointment.requestMsg') : $t('appointment.responseMsg') }}
        </span>
      </div>

      <!-- Body -->
      <div class="px-3 py-2.5 space-y-1.5 bg-white">
        <!-- Date -->
        <div class="flex items-center gap-2 text-sm">
          <Clock class="w-3.5 h-3.5 text-gray-400" />
          <span class="text-gray-700">{{ formatAppointmentDate(msg.appointmentDate) }}</span>
        </div>

        <!-- Motif -->
        <div v-if="msg.appointmentMotif" class="text-sm text-gray-600">
          <span class="font-medium text-gray-500">{{ $t('appointment.motifLabel') }}:</span> {{ msg.appointmentMotif }}
        </div>

        <!-- Status badge -->
        <div class="flex items-center gap-1.5">
          <span class="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium" :class="statusClass">
            <component :is="statusIcon" class="w-3 h-3" />
            {{ statusText }}
          </span>
        </div>

        <!-- Refusal reason -->
        <div v-if="msg.appointmentStatus === AppointmentStatus.Refused && msg.text" class="text-xs text-red-600 italic mt-1">
          {{ msg.text }}
        </div>

        <!-- Admin actions (accept/refuse) -->
        <div v-if="showActions" class="flex gap-2 mt-2 pt-2 border-t border-gray-100">
          <button
            @click="respond(true)"
            :disabled="responding"
            class="flex-1 flex items-center justify-center gap-1 px-3 py-1.5 rounded-lg text-xs font-medium text-white bg-green-500 hover:bg-green-600 disabled:opacity-40 transition cursor-pointer"
          >
            <Check class="w-3.5 h-3.5" />
            {{ $t('appointment.accept') }}
          </button>
          <button
            @click="showRefuseForm = !showRefuseForm"
            :disabled="responding"
            class="flex-1 flex items-center justify-center gap-1 px-3 py-1.5 rounded-lg text-xs font-medium text-white bg-red-500 hover:bg-red-600 disabled:opacity-40 transition cursor-pointer"
          >
            <XIcon class="w-3.5 h-3.5" />
            {{ $t('appointment.refuse') }}
          </button>
        </div>

        <!-- Refuse form -->
        <div v-if="showRefuseForm" class="mt-2 space-y-2">
          <textarea
            v-model="refuseReason"
            :placeholder="$t('appointment.refusePlaceholder')"
            rows="2"
            class="w-full px-2 py-1.5 rounded-lg border border-gray-200 text-xs focus:outline-none focus:border-red-300 resize-none"
          />
          <button
            @click="respond(false)"
            :disabled="responding"
            class="w-full px-3 py-1.5 rounded-lg text-xs font-medium text-white bg-red-500 hover:bg-red-600 disabled:opacity-40 transition cursor-pointer"
          >
            {{ $t('appointment.confirmRefuse') }}
          </button>
        </div>
      </div>

      <!-- Timestamp -->
      <div class="px-3 pb-2 bg-white">
        <span class="text-[10px] text-gray-400">{{ formatMsgTime(msg.date) }}</span>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed} from "vue"
import {Calendar, Clock, Check, X as XIcon, CircleDot, CheckCircle, XCircle} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useAppointmentService} from "@/inversify.config"
import {useUserStore} from "@/stores/userStore"
import {useChatStore} from "@/stores/chatStore"
import {Role} from "@/types/enums"
import {MessageType, type ChatMessage} from "@/types/entities"
import {AppointmentStatus} from "@/types/entities"

const props = defineProps<{
  msg: ChatMessage
}>()

const {t} = useI18n()
const userStore = useUserStore()
const chatStore = useChatStore()
const appointmentService = useAppointmentService()

const currentUserId = userStore.getUser.id
const responding = ref(false)
const showRefuseForm = ref(false)
const refuseReason = ref('')

const showActions = computed(() => {
  return userStore.hasRole(Role.Admin)
    && props.msg.type === MessageType.AppointmentRequest
    && props.msg.appointmentStatus === AppointmentStatus.Pending
})

const statusText = computed(() => {
  switch (props.msg.appointmentStatus) {
    case AppointmentStatus.Pending: return t('appointment.pending')
    case AppointmentStatus.Accepted: return t('appointment.accepted')
    case AppointmentStatus.Refused: return t('appointment.refused')
    default: return ''
  }
})

const statusClass = computed(() => {
  switch (props.msg.appointmentStatus) {
    case AppointmentStatus.Pending: return 'bg-yellow-50 text-yellow-700'
    case AppointmentStatus.Accepted: return 'bg-green-50 text-green-700'
    case AppointmentStatus.Refused: return 'bg-red-50 text-red-700'
    default: return 'bg-gray-50 text-gray-700'
  }
})

const statusIcon = computed(() => {
  switch (props.msg.appointmentStatus) {
    case AppointmentStatus.Pending: return CircleDot
    case AppointmentStatus.Accepted: return CheckCircle
    case AppointmentStatus.Refused: return XCircle
    default: return CircleDot
  }
})

const headerClass = computed(() => {
  if (props.msg.type === MessageType.AppointmentResponse) {
    return props.msg.appointmentStatus === AppointmentStatus.Accepted
      ? 'bg-green-500 text-white'
      : 'bg-red-500 text-white'
  }
  return 'bg-brand-100 text-brand-800'
})

const borderClass = computed(() => {
  if (props.msg.type === MessageType.AppointmentResponse) {
    return props.msg.appointmentStatus === AppointmentStatus.Accepted
      ? 'border-green-200'
      : 'border-red-200'
  }
  return 'border-brand-200'
})

function formatAppointmentDate(dateStr: string | null): string {
  if (!dateStr) return ''
  return DateTime.fromISO(dateStr, {zone: 'utc'}).toLocal().toFormat('dd MMMM yyyy, HH:mm')
}

function formatMsgTime(dateStr: string): string {
  return DateTime.fromISO(dateStr, {zone: 'utc'}).toLocal().toFormat('HH:mm')
}

async function respond(accept: boolean) {
  if (!props.msg.appointmentId) return
  responding.value = true
  try {
    const responseMsg = await appointmentService.respondAppointment(
      props.msg.appointmentId,
      accept,
      accept ? undefined : refuseReason.value.trim() || undefined
    )
    chatStore.addMessage(responseMsg.conversationId, responseMsg)

    // Update the original message's appointment status in store
    const messages = chatStore.currentMessages
    const originalMsg = messages.find(m => m.appointmentId === props.msg.appointmentId && m.type === MessageType.AppointmentRequest)
    if (originalMsg) {
      originalMsg.appointmentStatus = accept ? AppointmentStatus.Accepted : AppointmentStatus.Refused
    }

    showRefuseForm.value = false
  } catch {
    alert(t('appointment.respondError'))
  } finally {
    responding.value = false
  }
}
</script>
