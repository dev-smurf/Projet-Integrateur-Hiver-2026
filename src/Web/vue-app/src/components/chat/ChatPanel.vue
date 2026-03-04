<template>
  <div class="w-[360px] h-[480px] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col border border-gray-200">
    <!-- Header -->
    <div class="bg-brand-900 px-4 py-3 flex items-center justify-between shrink-0">
      <div class="flex items-center gap-2">
        <button
          v-if="chatStore.view === 'chat' && userStore.hasRole(Role.Admin)"
          @click="chatStore.goToList()"
          class="text-gray-400 hover:text-white transition cursor-pointer"
        >
          <ArrowLeft class="w-4 h-4" />
        </button>
        <h3 class="text-white text-sm font-semibold">
          <template v-if="chatStore.view === 'list'">{{ $t('chat.conversations') }}</template>
          <template v-else-if="chatStore.currentConversation">
            {{ userStore.hasRole(Role.Admin) ? chatStore.currentConversation.memberName : chatStore.currentConversation.adminName }}
          </template>
        </h3>
      </div>
      <!-- Search is built into ConversationList now -->
    </div>

    <!-- Content -->
    <ConversationList v-if="chatStore.view === 'list' && userStore.hasRole(Role.Admin)" />
    <ChatView v-else-if="chatStore.view === 'chat' || userStore.hasRole(Role.Member)" />
  </div>
</template>

<script lang="ts" setup>
import {ArrowLeft} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import {useUserStore} from "@/stores/userStore"
import {Role} from "@/types/enums"
import ConversationList from "./ConversationList.vue"
import ChatView from "./ChatView.vue"

const chatStore = useChatStore()
const userStore = useUserStore()
</script>
