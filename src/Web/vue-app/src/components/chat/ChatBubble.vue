<template>
  <div class="fixed bottom-4 right-4 z-40 flex flex-col items-end gap-3">
    <!-- Chat Panel -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0 translate-y-4 scale-95"
      enter-to-class="opacity-100 translate-y-0 scale-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100 translate-y-0 scale-100"
      leave-to-class="opacity-0 translate-y-4 scale-95"
    >
      <ChatPanel v-if="chatStore.isOpen" />
    </Transition>

    <!-- Bubble Button -->
    <button
      @click="chatStore.togglePanel()"
      class="w-14 h-14 rounded-full bg-brand-600 text-white shadow-lg hover:bg-brand-500 hover:scale-105 active:scale-95 transition-all duration-200 flex items-center justify-center relative cursor-pointer"
    >
      <MessageCircle v-if="!chatStore.isOpen" class="w-6 h-6" />
      <X v-else class="w-6 h-6" />

      <!-- Unread badge -->
      <span
        v-if="chatStore.totalUnreadCount > 0 && !chatStore.isOpen"
        class="absolute -top-1 -right-1 min-w-5 h-5 px-1.5 rounded-full bg-red-500 text-white text-xs font-bold flex items-center justify-center animate-bounce-in"
      >
        {{ chatStore.totalUnreadCount > 99 ? '99+' : chatStore.totalUnreadCount }}
      </span>
    </button>
  </div>
</template>

<script lang="ts" setup>
import {MessageCircle, X} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import ChatPanel from "./ChatPanel.vue"

const chatStore = useChatStore()
</script>

<style scoped>
.animate-bounce-in {
  animation: bounceIn 0.3s ease-out;
}

@keyframes bounceIn {
  0% { transform: scale(0); }
  60% { transform: scale(1.2); }
  100% { transform: scale(1); }
}
</style>
