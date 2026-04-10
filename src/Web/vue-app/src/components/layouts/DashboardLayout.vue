
<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="sticky top-0 z-50 border-b border-white/10 bg-brand-900/95 backdrop-blur">
      <div class="mx-auto flex h-16 max-w-7xl items-center justify-between gap-4 px-6">
        <div class="flex min-w-0 items-center gap-2">
          <div class="flex flex-wrap items-center gap-1">
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
              :to="{ name: 'member.modules.index' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('member.modules') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <BookOpen class="w-4 h-4" />
              Mes modules
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Member)"
              :to="{ name: 'equipe' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('equipe') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <UsersRound class="w-4 h-4" />
              {{ $t('routes.equipe.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Member)"
              :to="{ name: 'quiz.list' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('quiz') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <ClipboardCheck class="w-4 h-4" />
              {{ $t('routes.quiz.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Admin)"
              :to="{ name: 'admin.children.members.index' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('admin.children.members') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <Users class="w-4 h-4" />
              {{ $t('routes.admin.children.members.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Admin)"
              :to="{ name: 'admin.children.modules.index' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('admin.children.modules') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <Layers class="w-4 h-4" />
              {{ $t('routes.admin.children.modules.name') }}
            </router-link>
            <router-link
              v-if="userStore.hasRole(Role.Admin)"
              :to="{ name: 'admin.children.equipes.index' }"
              class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
              :class="isActive('admin.children.equipes') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
            >
              <UsersRound class="w-4 h-4" />
              {{ $t("routes.admin.children.equipes.name") }}
            </router-link>
          </div>
        </div>

        <div class="flex shrink-0 items-center gap-3">
          <div class="relative">
            <button
              @click="langOpen = !langOpen"
              class="rounded-lg p-2 text-gray-400 transition hover:bg-white/5 hover:text-white"
            >
              <Languages class="h-4 w-4" />
            </button>
            <div
              v-if="langOpen"
              class="absolute right-0 top-full mt-2 min-w-[120px] rounded-lg bg-brand-800 py-1 shadow-lg"
            >
              <button
                v-for="loc in LOCALES"
                :key="loc.value"
                @click="switchLanguage(loc.value)"
                class="w-full px-3 py-2 text-left text-sm transition"
                :class="currentLocale === loc.value ? 'bg-white/10 text-white' : 'text-gray-400 hover:bg-white/5 hover:text-white'"
              >
                {{ loc.caption }}
              </button>
            </div>
          </div>

          <router-link
            :to="{ name: 'account' }"
            class="flex min-w-0 items-center gap-3 rounded-xl px-3 py-2 transition hover:bg-white/5"
          >
            <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand-600 text-sm font-semibold text-white">
              {{ initials }}
            </div>
            <div class="min-w-0">
              <p class="truncate text-sm font-medium text-white">
                {{ personStore.person.firstName }} {{ personStore.person.lastName }}
              </p>
              <p class="truncate text-xs text-gray-400">{{ $t('routes.account.name') }}</p>
            </div>
          </router-link>

          <button
            @click="handleLogout"
            class="rounded-lg p-2 text-gray-400 transition hover:bg-white/5 hover:text-white"
            :title="$t('global.logout')"
          >
            <LogOut class="h-4 w-4" />
          </button>
        </div>
      </div>
    </nav>

    <main class="mx-auto w-full max-w-7xl px-6 py-8">
      <router-view />
    </main>

    <notifications position="bottom right" :duration="4000" :speed="300" width="360">
      <template #body="{ item, close }">
        <div class="mb-3 mr-3 overflow-hidden rounded-xl shadow-lg backdrop-blur-sm"
             :class="item.type === 'success' ? 'border border-green-200 bg-white' : 'border border-red-200 bg-white'">
          <div class="flex items-start gap-3 px-4 py-3">
            <div class="mt-0.5 flex h-5 w-5 shrink-0 items-center justify-center rounded-full"
                 :class="item.type === 'success' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'">
              <CheckCircle2 v-if="item.type === 'success'" class="h-3.5 w-3.5" />
              <XCircle v-else class="h-3.5 w-3.5" />
            </div>
            <p class="flex-1 text-sm leading-snug text-gray-700">{{ item.text }}</p>
            <button @click="close" class="mt-0.5 shrink-0 text-gray-300 transition hover:text-gray-500">
              <X class="h-3.5 w-3.5" />
            </button>
          </div>
          <div class="h-0.5 w-full" :class="item.type === 'success' ? 'bg-green-100' : 'bg-red-100'">
            <div class="toast-progress h-full"
                 :class="item.type === 'success' ? 'bg-green-500' : 'bg-red-500'" />
          </div>
        </div>
      </template>
    </notifications>

    <ChatBubble />
  </div>
</template>

<script lang="ts" setup>
    import { computed, ref, onMounted, onUnmounted } from "vue";
    import { useRouter } from "vue-router";
    import { useI18n } from "vue3-i18n";
    import Cookies from "universal-cookie";
    import {
        LayoutDashboard, BookOpen, Shield, LogOut, Languages,
        CheckCircle2, XCircle, X, Users, Layers, UsersRound,
        ClipboardCheck
    } from "lucide-vue-next";
    import { useUserStore } from "@/stores/userStore";
    import { usePersonStore } from "@/stores/personStore";
    import ChatBubble from "@/components/chat/ChatBubble.vue";
    import { useMemberService, useAdministratorService, useAuthenticationService, useConversationService } from "@/inversify.config";
    import { useChatStore } from "@/stores/chatStore";
    import { useSignalR } from "@/composables/useSignalR";
    import { Role } from "@/types/enums";
    import { LOCALES } from "@/locales";

    const router = useRouter();
    const i18nInstance = useI18n();
    const userStore = useUserStore();
    const personStore = usePersonStore();
    const memberService = useMemberService();
    const adminService = useAdministratorService();
    const authService = useAuthenticationService();
    const conversationService = useConversationService();
    const chatStore = useChatStore();
    const { connect: connectSignalR, disconnect: disconnectSignalR } = useSignalR();

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
        cookies.set("lang", lang, { path: "/" });
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
        await router.push({ name: "login" });
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
