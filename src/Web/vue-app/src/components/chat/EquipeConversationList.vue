<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Search -->
    <div v-if="!loading && chatStore.equipeConversations.length > 0" class="px-4 py-3 border-b border-gray-100">
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          v-model="searchQuery"
          :placeholder="$t('chat.searchEquipe')"
          class="w-full pl-9 pr-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 transition"
        />
      </div>
    </div>

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
      <div v-else-if="filteredConversations.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400 px-6">
        <Users class="w-10 h-10 mb-2 opacity-50" />
        <p class="text-sm text-center">{{ $t('chat.noEquipeConversations') }}</p>
      </div>

      <!-- Conversation list -->
      <div v-else>
        <button
          v-for="conv in filteredConversations"
          :key="conv.id"
          @click="openEquipeConversation(conv.id)"
          class="w-full flex items-center gap-3 px-4 py-3 hover:bg-brand-50 transition text-left cursor-pointer border-b border-gray-100 last:border-b-0"
        >
          <!-- Avatar -->
          <div class="w-10 h-10 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-sm font-semibold shrink-0">
            {{ getInitials(conv.equipeName) }}
          </div>

          <!-- Content -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center justify-between">
              <span class="text-sm font-medium text-gray-900 truncate">{{ conv.equipeName }}</span>
              <span v-if="conv.lastMessage" class="text-[11px] text-gray-400 shrink-0 ml-2">{{ formatTime(conv.lastMessageAt) }}</span>
            </div>
            <p class="text-xs text-gray-500 truncate mt-0.5">
              {{ conv.lastMessage || $t('chat.equipeMembersCount', { count: conv.membersCount }) }}
            </p>
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
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted, onActivated} from "vue"
import {Users, Search} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useChatStore} from "@/stores/chatStore"
import {useEquipeConversationService} from "@/inversify.config"

const {t} = useI18n()
const chatStore = useChatStore()
const equipeConversationService = useEquipeConversationService()
const loading = ref(true)
const searchQuery = ref('')

const filteredConversations = computed(() => {
  const q = searchQuery.value.toLowerCase()
  if (!q) return chatStore.equipeConversations
  return chatStore.equipeConversations.filter(c =>
    (c.equipeName || '').toLowerCase().includes(q)
  )
})

function getInitials(name: string): string {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function formatTime(dateStr: string): string {
  const dt = DateTime.fromISO(dateStr, {zone: 'utc'}).toLocal()
  const now = DateTime.now()
  if (dt.hasSame(now, 'day')) return dt.toFormat('HH:mm')
  if (dt.hasSame(now.minus({days: 1}), 'day')) return t('chat.yesterday')
  return dt.toFormat('dd/MM')
}

async function openEquipeConversation(conversationId: string) {
  chatStore.openEquipeConversation(conversationId)
  const messages = await equipeConversationService.getMessages(conversationId)
  chatStore.setEquipeMessages(conversationId, messages)
  await equipeConversationService.markAsRead(conversationId)
  chatStore.markEquipeConversationAsRead(conversationId)
}

async function loadConversations() {
  try {
    const conversations = await equipeConversationService.getConversations()
    chatStore.setEquipeConversations(conversations)
  } catch {
    // silent
  } finally {
    loading.value = false
  }
}

onMounted(loadConversations)
onActivated(loadConversations)
</script>
