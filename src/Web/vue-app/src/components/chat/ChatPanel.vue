<template>
  <div class="w-[360px] h-[480px] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col border border-gray-200">
    <!-- Header -->
    <div class="bg-brand-900 px-4 py-3 flex items-center justify-between shrink-0">
      <div class="flex items-center gap-2">
        <button
          v-if="showBackButton"
          @click="goBack"
          class="text-gray-400 hover:text-white transition cursor-pointer"
        >
          <ArrowLeft class="w-4 h-4" />
        </button>
        <h3 class="text-white text-sm font-semibold">
          <template v-if="headerTitle">{{ headerTitle }}</template>
        </h3>
      </div>
    </div>

    <!-- Tabs -->
    <div
      v-if="showTabs"
      class="flex items-center border-b border-gray-200 shrink-0"
    >
      <button
        @click="chatStore.setChatTab('membres')"
        class="flex-1 py-2.5 text-sm font-medium transition cursor-pointer"
        :class="chatStore.chatTab === 'membres'
          ? 'text-brand-600 border-b-2 border-brand-500'
          : 'text-gray-500 hover:text-gray-700 border-b-2 border-transparent'"
      >
        {{ memberTabLabel }}
      </button>
      <button
        @click="chatStore.setChatTab('equipes')"
        class="flex-1 py-2.5 text-sm font-medium transition cursor-pointer"
        :class="chatStore.chatTab === 'equipes'
          ? 'text-brand-600 border-b-2 border-brand-500'
          : 'text-gray-500 hover:text-gray-700 border-b-2 border-transparent'"
      >
        {{ equipeTabLabel }}
      </button>
    </div>

    <!-- Content -->
    <!-- Admin -->
    <template v-if="userStore.hasRole(Role.Admin)">
      <template v-if="chatStore.view === 'list'">
        <ConversationList v-if="chatStore.chatTab === 'membres'" />
        <EquipeConversationList v-else />
      </template>
      <ChatView v-else-if="chatStore.view === 'chat'" />
      <EquipeChatView v-else-if="chatStore.view === 'equipe-chat'" />
    </template>

    <!-- Member with team access: tabs control the content -->
    <template v-else-if="userStore.hasRole(Role.Member) && hasEquipeAccess">
      <ChatView v-if="chatStore.chatTab === 'membres'" />
      <template v-else>
        <EquipeChatView v-if="chatStore.view === 'equipe-chat'" />
        <EquipeConversationList v-else />
      </template>
    </template>

    <!-- Member without team: original single chat view -->
    <template v-else-if="userStore.hasRole(Role.Member)">
      <ChatView />
    </template>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted} from "vue"
import {ArrowLeft} from "lucide-vue-next"
import {useI18n} from "vue3-i18n"
import {useChatStore} from "@/stores/chatStore"
import {useUserStore} from "@/stores/userStore"
import {useConversationService, useEquipeConversationService} from "@/inversify.config"
import {Role} from "@/types/enums"
import ConversationList from "./ConversationList.vue"
import ChatView from "./ChatView.vue"
import EquipeConversationList from "./EquipeConversationList.vue"
import EquipeChatView from "./EquipeChatView.vue"

const {t} = useI18n()
const chatStore = useChatStore()
const userStore = useUserStore()
const equipeConversationService = useEquipeConversationService()
const conversationService = useConversationService()

const hasEquipeAccess = computed(() => chatStore.equipeConversations.length > 0)

const showTabs = computed(() => {
  if (userStore.hasRole(Role.Admin)) {
    return chatStore.view === 'list'
  }
  if (userStore.hasRole(Role.Member)) {
    // Member with teams: tabs are always visible (except when inside an equipe-chat)
    return hasEquipeAccess.value && chatStore.view !== 'equipe-chat'
  }
  return false
})

const memberTabLabel = computed(() =>
  userStore.hasRole(Role.Admin) ? t('chat.tabs.members') : t('chat.tabs.admin')
)

const equipeTabLabel = computed(() =>
  userStore.hasRole(Role.Admin) ? t('chat.tabs.equipes') : t('chat.tabs.myEquipe')
)

const showBackButton = computed(() => {
  if (userStore.hasRole(Role.Admin)) {
    return chatStore.view === 'chat' || chatStore.view === 'equipe-chat'
  }
  if (userStore.hasRole(Role.Member)) {
    // Only when inside a specific equipe-chat we show back (to return to the equipe list tab)
    return chatStore.view === 'equipe-chat'
  }
  return false
})

const headerTitle = computed(() => {
  if (chatStore.view === 'list') return t('chat.conversations')
  if (chatStore.view === 'chat' && chatStore.currentConversation) {
    return userStore.hasRole(Role.Admin)
      ? chatStore.currentConversation.memberName
      : chatStore.currentConversation.adminName
  }
  if (chatStore.view === 'equipe-chat' && chatStore.currentEquipeConversation) {
    return chatStore.currentEquipeConversation.equipeName
  }
  return t('chat.conversations')
})

function goBack() {
  if (chatStore.view === 'equipe-chat') {
    // Member/admin: back from equipe-chat returns to list (with equipes tab selected)
    chatStore.goToEquipeList()
  } else {
    chatStore.goToList()
  }
}

// Fetch equipe conversations upfront so tabs can show the correct UI.
// For members, also resolve their admin conversation here so we know whether to
// switch to chat view (no teams) or keep list view with tabs (has teams).
onMounted(async () => {
  try {
    const equipeConvs = await equipeConversationService.getConversations()
    chatStore.setEquipeConversations(equipeConvs)
  } catch {
    // silent
  }

  if (userStore.hasRole(Role.Member) && !chatStore.currentConversationId) {
    try {
      const conversations = await conversationService.getConversations()
      chatStore.setConversations(conversations)
      if (conversations.length > 0) {
        const memberHasEquipe = chatStore.equipeConversations.length > 0
        if (memberHasEquipe) {
          // Keep view='list' so tabs remain visible; just set the current conversation id.
          chatStore.setCurrentConversationId(conversations[0].id)
        } else {
          chatStore.openConversation(conversations[0].id)
        }
      }
    } catch {
      // silent
    }
  }
})
</script>
