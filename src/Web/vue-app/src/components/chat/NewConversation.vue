<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Search -->
    <div class="px-4 py-3 border-b border-gray-100">
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          v-model="searchQuery"
          :placeholder="$t('chat.searchMember')"
          class="w-full pl-9 pr-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 transition"
        />
      </div>
    </div>

    <!-- Member list -->
    <div class="flex-1 overflow-y-auto">
      <div v-if="loading" class="p-4 space-y-3">
        <div v-for="i in 5" :key="i" class="flex items-center gap-3 animate-pulse">
          <div class="w-9 h-9 bg-gray-200 rounded-full" />
          <div class="h-3 bg-gray-200 rounded w-32" />
        </div>
      </div>

      <button
        v-for="member in filteredMembers"
        :key="member.id"
        @click="startConversation(member.id)"
        class="w-full flex items-center gap-3 px-4 py-3 hover:bg-brand-50 transition text-left cursor-pointer border-b border-gray-100 last:border-b-0"
      >
        <div class="w-9 h-9 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-xs font-semibold">
          {{ getInitials(member.firstName, member.lastName) }}
        </div>
        <span class="text-sm text-gray-700">{{ member.firstName }} {{ member.lastName }}</span>
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted} from "vue"
import {Search} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import {useConversationService, useMemberService} from "@/inversify.config"
import type {Member} from "@/types/entities"

const chatStore = useChatStore()
const conversationService = useConversationService()
const memberService = useMemberService()

const searchQuery = ref('')
const members = ref<Member[]>([])
const loading = ref(true)

const filteredMembers = computed(() => {
  const q = searchQuery.value.toLowerCase()
  if (!q) return members.value
  return members.value.filter(m =>
    `${m.firstName} ${m.lastName}`.toLowerCase().includes(q)
  )
})

function getInitials(first: string, last: string): string {
  return ((first?.[0] || '') + (last?.[0] || '')).toUpperCase()
}

async function startConversation(memberId: string) {
  const result = await conversationService.createConversation(memberId)
  const conversations = await conversationService.getConversations()
  chatStore.setConversations(conversations)
  chatStore.openConversation(result.id)
}

onMounted(async () => {
  try {
    const result = await memberService.search(0, 100, '')
    members.value = result.items
  } finally {
    loading.value = false
  }
})
</script>
