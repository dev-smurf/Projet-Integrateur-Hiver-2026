<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-brand-900 sticky top-0 z-50">
      <div class="max-w-7xl mx-auto px-6">
        <div class="flex items-center justify-between h-14">
          <!-- Left: nav links -->
          <div class="flex items-center gap-1">
            <router-link
              :to="{ name: 'dashboard' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('dashboard') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <LayoutDashboard class="w-4 h-4" />
              {{ $t('routes.dashboard.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Member)"
              :to="{ name: 'books' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('books') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <BookOpen class="w-4 h-4" />
              {{ $t('routes.books.name') }}
            </router-link>
          </div>

          <!-- Right: language, admin, profile, logout -->
          <div class="flex items-center gap-3">
            <!-- Language dropdown -->
            <div class="relative">
              <button
                @click="langOpen = !langOpen"
                class="p-1.5 text-gray-400 hover:text-white rounded-lg hover:bg-white/5 transition cursor-pointer"
              >
                <Languages class="w-4 h-4" />
              </button>
              <div
                v-if="langOpen"
                class="absolute right-0 top-full mt-2 bg-brand-800 rounded-lg shadow-lg py-1 z-50 min-w-[120px]"
              >
                <button
                  v-for="loc in LOCALES"
                  :key="loc.value"
                  @click="switchLanguage(loc.value)"
                  class="w-full text-left px-3 py-2 text-sm transition"
                  :class="currentLocale === loc.value ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
                >
                  {{ loc.caption }}
                </button>
              </div>
            </div>

            <!-- Admin panel -->
            <router-link
              v-if="userStore.hasRole(Role.Admin)"
              :to="{ name: 'admin.children.members.index' }"
              class="p-1.5 text-gray-400 hover:text-white rounded-lg hover:bg-white/5 transition cursor-pointer"
              :class="isActive('admin') ? 'text-white bg-white/10' : ''"
            >
              <Shield class="w-4 h-4" />
            </router-link>

            <!-- Profile -->
            <router-link
              :to="{ name: 'account' }"
              class="flex items-center gap-2 text-sm text-gray-400 hover:text-white transition"
            >
              <div class="w-7 h-7 rounded-full bg-brand-600 text-white flex items-center justify-center text-xs font-semibold">
                {{ initials }}
              </div>
              <span class="hidden sm:inline">{{ personStore.person.firstName }} {{ personStore.person.lastName }}</span>
            </router-link>

            <!-- Logout -->
            <button
              @click="handleLogout"
              class="flex items-center gap-1 text-sm text-gray-500 hover:text-brand-400 transition cursor-pointer"
            >
              <LogOut class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </nav>

    <main class="max-w-7xl mx-auto px-6 py-8">
      <router-view />
    </main>

    <notifications position="bottom right" :duration="4000" :speed="300" width="360">
      <template #body="{ item, close }">
        <div
          class="mb-3 mr-3 rounded-xl shadow-lg overflow-hidden backdrop-blur-sm"
          :class="item.type === 'success' ? 'bg-white border border-green-200' : 'bg-white border border-red-200'"
        >
          <div class="flex items-start gap-3 px-4 py-3">
            <div
              class="mt-0.5 w-5 h-5 rounded-full flex items-center justify-center shrink-0"
              :class="item.type === 'success' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'"
            >
              <CheckCircle2 v-if="item.type === 'success'" class="w-3.5 h-3.5" />
              <XCircle v-else class="w-3.5 h-3.5" />
            </div>
            <p class="text-sm text-gray-700 flex-1 leading-snug">{{ item.text }}</p>
            <button @click="close" class="text-gray-300 hover:text-gray-500 transition shrink-0 mt-0.5">
              <X class="w-3.5 h-3.5" />
            </button>
          </div>
          <div class="h-0.5 w-full" :class="item.type === 'success' ? 'bg-green-100' : 'bg-red-100'">
            <div
              class="h-full toast-progress"
              :class="item.type === 'success' ? 'bg-green-500' : 'bg-red-500'"
            />
          </div>
        </div>
      </template>
    </notifications>

    <ChatBubble />
  </div>
</template>

<script lang="ts" setup>
import {computed, ref, onMounted, onUnmounted} from "vue";
import {useRouter} from "vue-router";
import {useI18n} from "vue3-i18n";
import Cookies from "universal-cookie";
import {LayoutDashboard, BookOpen, Shield, LogOut, Languages, CheckCircle2, XCircle, X} from "lucide-vue-next";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import ChatBubble from "@/components/chat/ChatBubble.vue";
import {useMemberService, useAdministratorService, useAuthenticationService, useConversationService} from "@/inversify.config";
import {useChatStore} from "@/stores/chatStore";
import {useSignalR} from "@/composables/useSignalR";
import {Role} from "@/types/enums";
import {LOCALES} from "@/locales";

const router = useRouter();
const i18nInstance = useI18n();
const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const adminService = useAdministratorService();
const authService = useAuthenticationService();
const conversationService = useConversationService();
const chatStore = useChatStore();
const {connect: connectSignalR, disconnect: disconnectSignalR} = useSignalR();

const langOpen = ref(false);
const currentLocale = ref(i18nInstance.getLocale());

const initials = computed(() => {
  const first = personStore.person.firstName || "";
  const last = personStore.person.lastName || "";
  return ((first[0] || "") + (last[0] || "")).toUpperCase();
});

function isActive(routePrefix: string): boolean {
  const name = router.currentRoute.value.name as string || "";
  return name === routePrefix || name.startsWith(routePrefix + ".");
}

function switchLanguage(lang: string) {
  i18nInstance.setLocale(lang);
  currentLocale.value = lang;
  const cookies = new Cookies();
  cookies.set("lang", lang, {path: "/"});
  langOpen.value = false;
}

function handleClickOutside(e: MouseEvent) {
  if (langOpen.value && !(e.target as HTMLElement).closest(".relative")) {
    langOpen.value = false;
  }
}

async function handleLogout() {
  chatStore.reset();
  await disconnectSignalR();
  await authService.logout();
  userStore.reset();
  personStore.reset();
  await router.push({name: "login"});
}

onMounted(async () => {
  document.addEventListener("click", handleClickOutside);
  try {
    if (userStore.hasRole(Role.Admin)) {
      const admin = await adminService.getAuthenticated();
      if (admin) personStore.setPerson(admin);
    } else if (userStore.hasRole(Role.Member)) {
      const member = await memberService.getAuthenticated();
      if (member) personStore.setPerson(member);
    }
  } catch {
    // API failed — personStore already has persisted data from login
  }

  // Initialize chat
  await connectSignalR();
  try {
    const unreadCount = await conversationService.getUnreadCount();
    chatStore.setUnreadCount(unreadCount);
  } catch {
    // Chat unavailable — non-blocking
  }
});

onUnmounted(async () => {
  document.removeEventListener("click", handleClickOutside);
  await disconnectSignalR();
});
</script>
