<template>
  <div class="flex-1 overflow-y-auto">
    <!-- Loading state -->
    <div v-if="loading" class="p-4 space-y-3">
      <div v-for="i in 4" :key="i" class="flex items-center gap-3 animate-pulse">
        <div class="w-10 h-10 bg-gray-200 rounded-full shrink-0" />
        <div class="flex-1 space-y-2">
          <div class="h-3 bg-gray-200 rounded w-24" />
          <div class="h-2.5 bg-gray-100 rounded w-40" />
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="chatStore.conversations.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400 px-6">
      <MessageCircle class="w-10 h-10 mb-2 opacity-50" />
      <p class="text-sm text-center">{{ $t('chat.noConversations') }}</p>
    </div>

    <!-- Conversation list -->
    <div v-else>
      <button
        v-for="conv in chatStore.conversations"
        :key="conv.id"
        @click="openConversation(conv.id)"
        class="w-full flex items-center gap-3 px-4 py-3 hover:bg-brand-50 transition text-left cursor-pointer border-b border-gray-100 last:border-b-0"
      >
        <!-- Avatar -->
        <div class="w-10 h-10 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-sm font-semibold shrink-0">
          {{ getInitials(conv.memberName) }}
        </div>

        <!-- Content -->
        <div class="flex-1 min-w-0">
          <div class="flex items-center justify-between">
            <span class="text-sm font-medium text-gray-900 truncate">{{ conv.memberName }}</span>
            <span class="text-[11px] text-gray-400 shrink-0 ml-2">{{ formatTime(conv.lastMessageAt) }}</span>
          </div>
          <p class="text-xs text-gray-500 truncate mt-0.5">{{ conv.lastMessage }}</p>
        </div>

        <!-- Unread badge -->
        <span
          v-if="conv.unreadCount > 0"
          class="min-w-5 h-5 px-1.5 rounded-full bg-brand-500 text-white text-[11px] font-bold flex items-center justify-center shrink-0"
        >
          {{ conv.unreadCount }}
        </span>
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, onMounted} from "vue"
import {MessageCircle} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useChatStore} from "@/stores/chatStore"
import {useConversationService} from "@/inversify.config"

const {t} = useI18n()
const chatStore = useChatStore()
const conversationService = useConversationService()
const loading = ref(true)

function getInitials(name: string): string {
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function formatTime(dateStr: string): string {
  const dt = DateTime.fromISO(dateStr)
  const now = DateTime.now()
  if (dt.hasSame(now, 'day')) return dt.toFormat('HH:mm')
  if (dt.hasSame(now.minus({days: 1}), 'day')) return t('chat.yesterday')
  return dt.toFormat('dd/MM')
}

async function openConversation(conversationId: string) {
  chatStore.openConversation(conversationId)
  const messages = await conversationService.getMessages(conversationId)
  chatStore.setMessages(conversationId, messages)
  await conversationService.markAsRead(conversationId)
  chatStore.markConversationAsRead(conversationId)
}

onMounted(async () => {
  try {
    const conversations = await conversationService.getConversations()
    chatStore.setConversations(conversations)
  } finally {
    loading.value = false
  }
})
</script>
