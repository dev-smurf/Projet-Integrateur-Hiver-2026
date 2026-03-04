<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Messages area -->
    <div ref="messagesContainer" class="flex-1 overflow-y-auto px-4 py-3 space-y-1">
      <!-- Loading -->
      <div v-if="loading" class="flex items-center justify-center h-full">
        <div class="w-6 h-6 border-2 border-brand-200 border-t-brand-500 rounded-full animate-spin" />
      </div>

      <!-- No conversation selected -->
      <div v-else-if="!chatStore.currentConversationId" class="flex flex-col items-center justify-center h-full text-gray-400">
        <MessageSquare class="w-8 h-8 mb-2 opacity-50" />
        <p class="text-sm text-center px-4">{{ $t('chat.noConversations') }}</p>
      </div>

      <!-- Empty conversation -->
      <div v-else-if="chatStore.currentMessages.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400">
        <MessageSquare class="w-8 h-8 mb-2 opacity-50" />
        <p class="text-sm">{{ $t('chat.noMessages') }}</p>
      </div>

      <!-- Messages -->
      <template v-else>
        <template v-for="(msg, index) in chatStore.currentMessages" :key="msg.id">
          <!-- Date separator -->
          <div v-if="showDateSeparator(index)" class="flex items-center gap-3 py-2">
            <div class="flex-1 h-px bg-gray-200" />
            <span class="text-[11px] text-gray-400 font-medium">{{ formatDate(msg.date) }}</span>
            <div class="flex-1 h-px bg-gray-200" />
          </div>

          <!-- Message bubble -->
          <div
            :class="msg.senderId === currentUserId ? 'flex justify-end' : 'flex justify-start'"
          >
            <div
              class="max-w-[75%] px-3 py-2 rounded-2xl text-sm leading-relaxed"
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
        </template>

        <!-- Typing indicator -->
        <div v-if="chatStore.isCurrentTyping" class="flex justify-start">
          <div class="bg-gray-100 rounded-2xl rounded-bl-md px-4 py-3">
            <div class="flex items-center gap-1">
              <span class="typing-dot w-2 h-2 bg-gray-400 rounded-full" />
              <span class="typing-dot w-2 h-2 bg-gray-400 rounded-full" style="animation-delay: 0.15s" />
              <span class="typing-dot w-2 h-2 bg-gray-400 rounded-full" style="animation-delay: 0.3s" />
            </div>
          </div>
        </div>
      </template>
    </div>

    <!-- Input bar -->
    <div v-if="chatStore.currentConversationId" class="shrink-0 border-t border-gray-200 px-3 py-2 bg-white">
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
import {useConversationService} from "@/inversify.config"
import {useSignalR} from "@/composables/useSignalR"
import {Role} from "@/types/enums"

const {t} = useI18n()
const chatStore = useChatStore()
const userStore = useUserStore()
const conversationService = useConversationService()
const {sendTyping} = useSignalR()

const messagesContainer = ref<HTMLElement>()
const inputEl = ref<HTMLTextAreaElement>()
const fileInput = ref<HTMLInputElement>()
const newMessage = ref('')
const selectedFile = ref<File | null>(null)
const loading = ref(false)
let typingTimeout: number | null = null

const currentUserId = userStore.getUser.id

const canSend = computed(() => {
  return newMessage.value.trim().length > 0 || selectedFile.value !== null
})

const recipientUserId = computed(() => {
  const conv = chatStore.currentConversation
  if (!conv) return null
  return conv.adminId === currentUserId ? conv.memberId : conv.adminId
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
  // Reset so the same file can be re-selected
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
  if (!chatStore.currentConversationId) return
  loading.value = true
  try {
    const messages = await conversationService.getMessages(chatStore.currentConversationId)
    chatStore.setMessages(chatStore.currentConversationId, messages)
    await conversationService.markAsRead(chatStore.currentConversationId)
    chatStore.markConversationAsRead(chatStore.currentConversationId)
  } finally {
    loading.value = false
  }
  await nextTick()
  scrollToBottom()
}

// For members: auto-load their single conversation
onMounted(async () => {
  if (userStore.hasRole(Role.Member) && !chatStore.currentConversationId) {
    try {
      const conversations = await conversationService.getConversations()
      chatStore.setConversations(conversations)
      if (conversations.length > 0) {
        chatStore.openConversation(conversations[0].id)
      }
    } catch {
      // API failed — show empty state
    }
  }
  if (chatStore.currentConversationId) {
    await loadMessages()
  }
})

function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

onActivated(async () => {
  if (chatStore.currentConversationId) {
    await conversationService.markAsRead(chatStore.currentConversationId).catch(() => {})
    chatStore.markConversationAsRead(chatStore.currentConversationId)
    await nextTick()
    scrollToBottom()
  }
})

watch(() => chatStore.currentMessages.length, () => {
  nextTick(() => scrollToBottom())
  // Auto-mark as read when a new message arrives while viewing this conversation
  if (chatStore.currentConversationId && chatStore.isOpen) {
    conversationService.markAsRead(chatStore.currentConversationId).catch(() => {})
    chatStore.markConversationAsRead(chatStore.currentConversationId)
  }
})

watch(() => chatStore.isCurrentTyping, (typing) => {
  if (typing) nextTick(() => scrollToBottom())
})

async function sendMessage() {
  const text = newMessage.value.trim()
  const file = selectedFile.value

  if (!text && !file) return
  if (!chatStore.currentConversationId) return

  newMessage.value = ''
  selectedFile.value = null

  const msg = await conversationService.sendMessage(chatStore.currentConversationId, text, file || undefined)
  chatStore.addMessage(chatStore.currentConversationId, msg)
  await nextTick()
  scrollToBottom()

  // Auto-resize textarea back
  if (inputEl.value) inputEl.value.style.height = 'auto'
}

function handleKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    sendMessage()
    return
  }
  emitTyping()
}

function emitTyping() {
  if (!chatStore.currentConversationId || !recipientUserId.value) return
  if (typingTimeout) return // Throttle: only emit once per second
  sendTyping(chatStore.currentConversationId, recipientUserId.value)
  typingTimeout = window.setTimeout(() => { typingTimeout = null }, 1000)
}

function showDateSeparator(index: number): boolean {
  if (index === 0) return true
  const msgs = chatStore.currentMessages
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
