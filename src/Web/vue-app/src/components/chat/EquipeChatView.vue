<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Messages area -->
    <div ref="messagesContainer" class="flex-1 overflow-y-auto px-4 py-3 space-y-1">
      <!-- Loading -->
      <div v-if="loading" class="flex items-center justify-center h-full">
        <div class="w-6 h-6 border-2 border-brand-200 border-t-brand-500 rounded-full animate-spin" />
      </div>

      <!-- No conversation selected -->
      <div v-else-if="!chatStore.currentEquipeConversationId" class="flex flex-col items-center justify-center h-full text-gray-400">
        <MessageSquare class="w-8 h-8 mb-2 opacity-50" />
        <p class="text-sm text-center px-4">{{ $t('chat.noEquipeConversations') }}</p>
      </div>

      <!-- Empty conversation -->
      <div v-else-if="chatStore.currentEquipeMessages.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400">
        <MessageSquare class="w-8 h-8 mb-2 opacity-50" />
        <p class="text-sm">{{ $t('chat.noMessages') }}</p>
      </div>

      <!-- Messages -->
      <template v-else>
        <template v-for="(msg, index) in chatStore.currentEquipeMessages" :key="msg.id">
          <!-- Date separator -->
          <div v-if="showDateSeparator(index)" class="flex items-center gap-3 py-2">
            <div class="flex-1 h-px bg-gray-200" />
            <span class="text-[11px] text-gray-400 font-medium">{{ formatDate(msg.date) }}</span>
            <div class="flex-1 h-px bg-gray-200" />
          </div>

          <!-- Regular message bubble -->
          <div :class="msg.senderId === currentUserId ? 'flex justify-end' : 'flex justify-start'">
            <div class="flex flex-col max-w-[75%]" :class="msg.senderId === currentUserId ? 'items-end' : 'items-start'">
              <!-- Sender name (always shown, above the bubble) -->
              <span
                class="text-[11px] font-semibold mb-0.5 px-1"
                :class="msg.senderId === currentUserId ? 'text-brand-700' : 'text-gray-600'"
              >
                {{ msg.senderId === currentUserId ? $t('chat.you') : (msg.senderName || '—') }}
              </span>

              <div
                class="px-3 py-2 rounded-2xl text-sm leading-relaxed"
                :class="msg.senderId === currentUserId
                  ? 'bg-brand-500 text-white rounded-br-md'
                  : 'bg-gray-100 text-gray-800 rounded-bl-md'"
              >
                <!-- Attachment: image -->
                <img
                  v-if="msg.attachmentUrl && isImage(msg.attachmentContentType)"
                  :src="getAttachmentFullUrl(msg.attachmentUrl)"
                  :alt="msg.attachmentFileName || 'image'"
                  class="rounded-lg max-w-full max-h-48 object-cover mb-1 cursor-pointer"
                  @click="openUrl(getAttachmentFullUrl(msg.attachmentUrl))"
                />

                <!-- Attachment: PDF / other file -->
                <a
                  v-else-if="msg.attachmentUrl"
                  :href="getAttachmentFullUrl(msg.attachmentUrl)"
                  target="_blank"
                  class="flex items-center gap-2 px-3 py-2 rounded-lg mb-1 transition"
                  :class="msg.senderId === currentUserId
                    ? 'bg-brand-600/30 hover:bg-brand-600/50 text-white'
                    : 'bg-gray-200 hover:bg-gray-300 text-gray-700'"
                >
                  <FileText class="w-4 h-4 shrink-0" />
                  <span class="text-xs truncate">{{ msg.attachmentFileName }}</span>
                </a>

                <!-- Text -->
                <p v-if="msg.text" class="whitespace-pre-wrap break-words">{{ msg.text }}</p>

                <span
                  class="block text-[10px] mt-1 text-right"
                  :class="msg.senderId === currentUserId ? 'text-white/60' : 'text-gray-400'"
                >
                  {{ formatMsgTime(msg.date) }}
                </span>
              </div>
            </div>
          </div>
        </template>
      </template>
    </div>

    <!-- Input bar -->
    <div v-if="chatStore.currentEquipeConversationId" class="shrink-0 border-t border-gray-200 px-3 py-2 bg-white">
      <!-- File preview -->
      <div v-if="selectedFile" class="flex items-center gap-2 px-3 py-2 mb-2 bg-gray-50 rounded-lg border border-gray-200">
        <FileText class="w-4 h-4 text-gray-500 shrink-0" />
        <span class="text-xs text-gray-600 truncate flex-1">{{ selectedFile.name }}</span>
        <button @click="clearFile" class="text-gray-400 hover:text-gray-600 cursor-pointer">
          <X class="w-4 h-4" />
        </button>
      </div>

      <div class="flex items-end gap-2">
        <button
          @click="fileInput?.click()"
          class="w-9 h-9 rounded-full text-gray-400 hover:text-gray-600 flex items-center justify-center transition shrink-0 cursor-pointer"
        >
          <Paperclip class="w-4 h-4" />
        </button>
        <input
          ref="fileInput"
          type="file"
          accept="image/jpeg,image/jpg,image/png,image/gif,image/webp,application/pdf"
          class="hidden"
          @change="handleFileSelect"
        />
        <textarea
          ref="inputEl"
          v-model="newMessage"
          @keydown="handleKeydown"
          @paste="handlePaste"
          :placeholder="$t('chat.typePlaceholder')"
          rows="1"
          class="flex-1 resize-none rounded-xl border border-gray-200 px-3 py-2 text-sm focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 max-h-20 transition"
        />
        <button
          @click="sendMessage"
          :disabled="!canSend"
          class="w-9 h-9 rounded-full bg-brand-600 text-white flex items-center justify-center hover:bg-brand-500 disabled:opacity-40 disabled:cursor-not-allowed transition shrink-0 cursor-pointer"
        >
          <Send class="w-4 h-4" />
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted, onActivated, nextTick, watch} from "vue"
import {MessageSquare, Send, Paperclip, FileText, X} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useChatStore} from "@/stores/chatStore"
import {useUserStore} from "@/stores/userStore"
import {useEquipeConversationService} from "@/inversify.config"

const {t} = useI18n()
const chatStore = useChatStore()
const userStore = useUserStore()
const equipeConversationService = useEquipeConversationService()

const messagesContainer = ref<HTMLElement>()
const inputEl = ref<HTMLTextAreaElement>()
const fileInput = ref<HTMLInputElement>()
const newMessage = ref('')
const selectedFile = ref<File | null>(null)
const loading = ref(false)

const currentUserId = userStore.getUser.id

const canSend = computed(() => {
  return newMessage.value.trim().length > 0 || selectedFile.value !== null
})

function isImage(contentType: string | null): boolean {
  return !!contentType && contentType.startsWith('image/')
}

function getAttachmentFullUrl(url: string): string {
  return `${import.meta.env.VITE_API_BASE_URL?.replace('/api', '')}${url}`
}

function openUrl(url: string) {
  window.open(url, '_blank')
}

function handleFileSelect(e: Event) {
  const input = e.target as HTMLInputElement
  if (input.files && input.files[0]) {
    const file = input.files[0]
    if (file.size > 10 * 1024 * 1024) {
      alert(t('chat.fileTooLarge'))
      return
    }
    selectedFile.value = file
  }
  input.value = ''
}

function clearFile() {
  selectedFile.value = null
}

function handlePaste(e: ClipboardEvent) {
  const items = e.clipboardData?.items
  if (!items) return
  for (const item of items) {
    if (item.type.startsWith('image/')) {
      e.preventDefault()
      const file = item.getAsFile()
      if (file) {
        selectedFile.value = new File([file], `screenshot-${Date.now()}.png`, { type: file.type })
      }
      return
    }
  }
}

async function loadMessages() {
  if (!chatStore.currentEquipeConversationId) return
  loading.value = true
  try {
    const messages = await equipeConversationService.getMessages(chatStore.currentEquipeConversationId)
    chatStore.setEquipeMessages(chatStore.currentEquipeConversationId, messages)
    await equipeConversationService.markAsRead(chatStore.currentEquipeConversationId)
    chatStore.markEquipeConversationAsRead(chatStore.currentEquipeConversationId)
  } finally {
    loading.value = false
  }
  await nextTick()
  scrollToBottom()
}

onMounted(async () => {
  if (chatStore.currentEquipeConversationId) {
    await loadMessages()
  }
})

function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

onActivated(async () => {
  if (chatStore.currentEquipeConversationId) {
    await equipeConversationService.markAsRead(chatStore.currentEquipeConversationId).catch(() => {})
    chatStore.markEquipeConversationAsRead(chatStore.currentEquipeConversationId)
    await nextTick()
    scrollToBottom()
  }
})

watch(() => chatStore.currentEquipeMessages.length, () => {
  nextTick(() => scrollToBottom())
  if (chatStore.currentEquipeConversationId && chatStore.isOpen) {
    equipeConversationService.markAsRead(chatStore.currentEquipeConversationId).catch(() => {})
    chatStore.markEquipeConversationAsRead(chatStore.currentEquipeConversationId)
  }
})

async function sendMessage() {
  const text = newMessage.value.trim()
  const file = selectedFile.value

  if (!text && !file) return
  if (!chatStore.currentEquipeConversationId) return

  newMessage.value = ''
  selectedFile.value = null

  const msg = await equipeConversationService.sendMessage(chatStore.currentEquipeConversationId, text, file || undefined)
  chatStore.addEquipeMessage(chatStore.currentEquipeConversationId, msg)
  await nextTick()
  scrollToBottom()

  if (inputEl.value) inputEl.value.style.height = 'auto'
}

function handleKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    sendMessage()
  }
}

function showDateSeparator(index: number): boolean {
  if (index === 0) return true
  const msgs = chatStore.currentEquipeMessages
  const current = DateTime.fromISO(msgs[index].date, {zone: 'utc'}).toLocal()
  const prev = DateTime.fromISO(msgs[index - 1].date, {zone: 'utc'}).toLocal()
  return !current.hasSame(prev, 'day')
}

function formatDate(dateStr: string): string {
  const dt = DateTime.fromISO(dateStr, {zone: 'utc'}).toLocal()
  const now = DateTime.now()
  if (dt.hasSame(now, 'day')) return t('chat.today')
  if (dt.hasSame(now.minus({days: 1}), 'day')) return t('chat.yesterday')
  return dt.toFormat('dd MMMM yyyy')
}

function formatMsgTime(dateStr: string): string {
  return DateTime.fromISO(dateStr, {zone: 'utc'}).toLocal().toFormat('HH:mm')
}
</script>
